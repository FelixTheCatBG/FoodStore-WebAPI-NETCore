using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrderSystem.Client.Data;
using FoodOrderSystem.Client.DTO;
using FoodOrderSystem.Client.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FoodOrderSystem.Client.Controllers
{
    //This Controller is for Showcase purposes only and will be deleted 

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShowCaseController : Controller
    {
        protected FoodOrderIdentityContext context;
        protected UserManager<ApplicationUser> userManager;
        protected SignInManager<ApplicationUser> signInManager;

        public ShowCaseController(
            FoodOrderIdentityContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;

        }

        //POST: api/appuser/createorder
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]MakeOrderRootObjectDTO model)
        {
            var email = model.Info.Email;
 
            var currentUser = userManager.Users.SingleOrDefault(e => e.Email == email);

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "PleaseLogin");
                return Unauthorized(); 
            }

            var order = new Order
            {
                ReservationTime = model.Info.ReservationTime,            
                TakeAway = false,
                ApplicationUserId = currentUser.Id
            };
            context.Orders.Add(order);

            foreach (var product in model.Products)
            {
                var productId = product.Id;
                var productQ = product.Quantity;

                var currentProduct = context.Products.Where(e => e.Id == productId).SingleOrDefault(); 

                var currentProduct2 = context.Products.Where(e => e.Id == productId)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,       
                    DiscountPrice = p.Discount != null ? p.Discount.DiscountedPrice : 0,
                }).SingleOrDefault();

                var orderProductItem = new OrderProduct()
                {
                    Order = order,
                    ProductId = currentProduct2.Id,
                    NetPrice = currentProduct2.DiscountPrice > 0 ? currentProduct2.DiscountPrice : currentProduct2.Price,
                    Quantity = productQ

                };
                await context.OrderProducts.AddAsync(orderProductItem);
            }

            order.TotalPrice = order.OrderProducts.Sum(op => op.CalculatedPrice);
            context.SaveChanges();


            return Created("GetOrder", new { id = order.Id });
        }

        // GET: api/appuser/getorders       
        [HttpPost]
        public IActionResult GetOrders([FromBody]ShowCaseDTO userEmail)
        {
            var email = userEmail.Email;
            var currentUser = userManager.Users.SingleOrDefault(e => e.Email == email);
          
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Please Login");
                return Unauthorized(); 
            }

            var orders = context.Orders
                .Where(e => e.ApplicationUserId == currentUser.Id)
                .Select(e => new
                {   Id = e.Id,
                    ReservationTime = e.ReservationTime,
                    TakeAway = e.TakeAway,
                    TotalPrice = e.TotalPrice,
                    Status = e.OrderStatus.ToString(),
                    Products = e.OrderProducts.Select(op => new
                    {
                        ProductName = op.Product.Name,
                        ProductQuantity = op.Quantity,
                        ProductPrice = op.Product.Price,
                        DiscountPrice = op.Product.Discount != null ? op.Product.Discount.DiscountedPrice : 0
                    }).ToList()
                }).ToList();

            return Ok(orders);
        }
    }
}