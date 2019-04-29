using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderSystem.Client.Data;
using FoodOrderSystem.Client.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FoodOrderSystem.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly FoodOrderIdentityContext _context;
        protected UserManager<ApplicationUser> userManager;
        protected SignInManager<ApplicationUser> signInManager;

        public ProductController(
            FoodOrderIdentityContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            this._context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetProducts()
        {
            var productsList = _context.Products             
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,                   
                    Category = p.Category.Name,
                    Price = p.Price,
                    DiscountPrice = p.Discount != null ? p.Discount.DiscountedPrice : 0
                }).ToList();

            return Ok(productsList);
        }

        // GET: api/Product/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
    
            var result = _context.Products
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category.Name,
                    Price = p.Price,
                    Ingredients = p.ProductIngredients.Select(e => new
                    {
                        Ingredient = e.Ingredient.Name
                    }).ToList()
                }).SingleOrDefault();


            if (result == null)
            {
                return NotFound();
            }
           
            return Ok(result);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody]Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}