using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrderSystem.Client.Data;
using FoodOrderSystem.Client.Models;
using FoodOrderSystem.Client.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using FoodOrderSystem.Client.Models.Enums;

namespace FoodOrderSystem.Client.Controllers
{
   
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        protected FoodOrderIdentityContext context;
        protected UserManager<ApplicationUser> userManager;
        protected SignInManager<ApplicationUser> signInManager;

        public AccountController(
            FoodOrderIdentityContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;

        }
       
        // POST: api/account/createuser
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email already exists");
                return BadRequest(ModelState);
            }
           
                await userManager.AddToRoleAsync(user, "Customer");

            return Created("CreateUser",new { message = "Account created!", Username = user.UserName });

        }

        // POST: api/account/login
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "The email is already registered.");
                return BadRequest();
            }

            var passwordSignInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if (!passwordSignInResult.Succeeded)
            {
                await userManager.AccessFailedAsync(user);
                ModelState.AddModelError(string.Empty, "Invalid login");
                return BadRequest();
            }
            return Ok(new { message = "Logged in success!", userEmail = model.Email}); ;      
        }

    
        // GET: api/account/getorders  - Get orders for logged in user.
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            var currentUserId = userManager.GetUserId(User);
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (currentUserId == null)
            {
                ModelState.AddModelError(string.Empty, "PleaseLogin");
                return Unauthorized();
            }
            var orders = context.Orders
                .Where(e => e.ApplicationUserId == currentUserId)
                .Select(e => new
                {
                    OrderId = e.Id,
                    ReservationTime = e.ReservationTime,
                    TakeAway = e.TakeAway,
                    TotalPrice = e.TotalPrice,
                    Status = e.OrderStatus.ToString(),
                    Products = e.OrderProducts.Select(op => new
                    {
                        ProductName = op.Product.Name,
                        ProductQuantity = op.Quantity,
                        ProductPrice = op.Product.Price,
                        DiscountPrice = op.Product.Discount != null ? op.Product.Discount.DiscountedPrice : 0,
                    }).ToList()
                }).ToList();

            return Ok(orders);
        }

        // POST: api/account/changepassword
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordDTO model)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var result = await userManager.ChangePasswordAsync(currentUser, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return Ok(new { message = "Password Changed!" });
        }

        // POST: api/account/logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return Ok(new { message = "Logged out!"});
        }

    }
}