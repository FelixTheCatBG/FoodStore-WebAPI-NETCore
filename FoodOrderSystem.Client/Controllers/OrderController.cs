using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderSystem.Client.Data;
using FoodOrderSystem.Client.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FoodOrderSystem.Client.DTO;

namespace FoodOrderSystem.Client.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly FoodOrderIdentityContext _context;
        protected UserManager<ApplicationUser> userManager;     
        protected SignInManager<ApplicationUser> signInManager;

        public OrderController(
            FoodOrderIdentityContext context,     
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this._context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // GET: api/Order
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return _context.Orders
                //.Where(o => o.OrderTime.Date == DateTime.Now.Date) // show all orders from today
                .OrderByDescending(o => o.OrderTime);
        }

        // GET: api/Order/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            var result = new
            {
                order.Id,
                order.OrderStatus,
                Products = order.OrderProducts.Select(op => new
                {
                    ProductName = op.Product.Name,
                    ProductQuantity = op.Quantity,
                    ProductPrice = op.Product.Price,
                    DiscountPrice = op.Product.Discount != null ? op.Product.Discount.DiscountedPrice : 0
                }).ToList()
            };

            return Ok(result);
           
        }

        //POST: api/Order
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody]MakeOrderRootObjectDTO model)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "PleaseLogin");
                return Unauthorized();
            }
           
            var currentUserId = userManager.GetUserId(HttpContext.User);

            //Create an order and assign the user to it
            var order = new Order
            {
                ReservationTime = model.Info.ReservationTime,
                TakeAway = false,
                ApplicationUserId = currentUser.Id
            };
            _context.Orders.Add(order);

            var listOfOrderProducts = new List<OrderProduct>();

            //Iteratate selected products by the user, check quantity and discount prices and map them with the order
            foreach (var product in model.Products)
            {
                var productId = product.Id;
                var productQ = product.Quantity;

                var currentProduct = _context.Products.Where(e => e.Id == productId)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPrice = p.Discount != null ? p.Discount.DiscountedPrice : 0
                }).SingleOrDefault();

                if (currentProduct == null)
                {
                    return BadRequest("No product with such Id was found.");
                }

                var orderProductItem = new OrderProduct()
                {
                    Order = order,
                    ProductId = currentProduct.Id,
                    NetPrice = currentProduct.DiscountPrice > 0 ? currentProduct.DiscountPrice : currentProduct.Price,
                    Quantity = productQ

                };
                listOfOrderProducts.Add(orderProductItem);
            }
            await _context.OrderProducts.AddRangeAsync(listOfOrderProducts);

            //Sum the Total Price for the Order
            order.TotalPrice = order.OrderProducts.Sum(op => op.CalculatedPrice);

            _context.SaveChanges();

            return Created("GetOrder", new { id = order.Id });
        }

        // PUT: api/Order/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder([FromRoute] int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Order/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}