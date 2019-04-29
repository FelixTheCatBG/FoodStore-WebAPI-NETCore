using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderSystem.Client.Data;
using FoodOrderSystem.Client.Models;
using Microsoft.AspNetCore.Authorization;

namespace FoodOrderSystem.Client.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly FoodOrderIdentityContext _context;

        public DiscountController(FoodOrderIdentityContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: api/Discount
        [HttpGet]
        public IActionResult GetDiscounts()
        {
            var discountedProducts = _context.Discounts
                .Select(d => new {
                    Id = d.Id,
                    ProductId = d.Product.Id,
                    Name = d.Product.Name,
                    Category = d.Product.Category,
                    Price = d.Product.Price,
                    DiscountedPrice = d.DiscountedPrice
                }).ToList();

            return Ok(discountedProducts);
        }

        [AllowAnonymous]
        // GET: api/Discount/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _context.Discounts.FindAsync(id);

            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }

        // PUT: api/Discount/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscount([FromRoute] int id, [FromBody] Discount discount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != discount.Id)
            {
                return BadRequest();
            }

            _context.Entry(discount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountExists(id))
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

        // POST: api/Discount
        [HttpPost]
        public async Task<IActionResult> PostDiscount([FromBody] Discount discount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiscount", new { id = discount.Id }, discount);
        }

        // DELETE: api/Discount/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            return Ok(discount);
        }

        private bool DiscountExists(int id)
        {
            return _context.Discounts.Any(e => e.Id == id);
        }
    }
}