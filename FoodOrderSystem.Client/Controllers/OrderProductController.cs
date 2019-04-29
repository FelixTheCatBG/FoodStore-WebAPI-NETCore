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

namespace FoodOrderSystem.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderProductController : ControllerBase
    {
        private readonly FoodOrderIdentityContext _context;
        protected UserManager<ApplicationUser> userManager;
        protected SignInManager<ApplicationUser> signInManager;

        public OrderProductController(
            FoodOrderIdentityContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this._context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // GET: api/OrderProduct
        [HttpGet]
        public IEnumerable<OrderProduct> GetOrderProducts()
        {
            return _context.OrderProducts;
        }

        // GET: api/OrderProduct/5
        [HttpGet("{id}")]
        public IActionResult GetOrderProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var orderProduct = _context.OrderProducts
                  .Where(e => e.OrderId == id)
                  .Select(u => new
                  {
                      Order = u.Order,
                      ProductName = u.Product.Name,
                      ProductPrice = u.Product.Price,
                  }).ToList();

            if (orderProduct == null)
            {
                return NotFound();
            }

            return Ok(orderProduct);
        }

        // PUT: api/OrderProduct/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderProduct([FromRoute] int id, [FromBody] OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderProduct.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(orderProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderProductExists(id))
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

        // POST: api/OrderProduct
        [HttpPost]
        public async Task<IActionResult> PostOrderProduct([FromBody] OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrderProducts.Add(orderProduct);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderProductExists(orderProduct.ProductId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderProduct", new { id = orderProduct.ProductId }, orderProduct);
        }

        // DELETE: api/OrderProduct/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderProduct = await _context.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            _context.OrderProducts.Remove(orderProduct);
            await _context.SaveChangesAsync();

            return Ok(orderProduct);
        }

        private bool OrderProductExists(int id)
        {
            return _context.OrderProducts.Any(e => e.ProductId == id);
        }
    }
}