using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wepapp.Data;
using wepapp.Models;

namespace wepapp.Controllers.Api
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.Products.ToListAsync();
            return Ok(items);
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product model)
        {
            if (model == null)
                return BadRequest("Invalid product.");

            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }
    }
}
