using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsService.Data;
using ProductsService.Models;

namespace ProductsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll() =>
            Ok(await _context.Products.ToListAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Product p)
        {
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
            return Ok(p);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Product p)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = p.Name;
            product.Quantity = p.Quantity;
            product.SupplierId = p.SupplierId;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
