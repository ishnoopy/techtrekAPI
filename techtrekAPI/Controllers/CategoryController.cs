using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using techtrekAPI.Entities;

namespace techtrekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly TechtrekContext _context;

        public CategoryController(TechtrekContext context)
        {
            _context = context;
        }

        [HttpGet("getCategories")]
        public async Task<IActionResult> GetCategories() 
        {
            var list = await _context.categories.ToListAsync();
            return Ok(list);
        }
    }
}
