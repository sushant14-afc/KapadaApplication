using KapadaModel.DBOs;
using KapadaModel.Item;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Api.DataContext;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody]CategoryItemDBO _item)
        {
            var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.Name == _item.Name);
            if (existingItem != null)
            {
                return BadRequest("Item name already exists");
            }

            var newItem = new ItemCategory
            {
                Name = _item.Name
            };

            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();

            return Ok("Category item created successfully");
        }

        [HttpGet("getCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Items
                .Select(c => new CategoryResponseDBO
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPut("updatecategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryItemDBO updatedCategory)
        {
            var category = await _context.Items.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            category.Name = updatedCategory.Name;
            await _context.SaveChangesAsync();

            return Ok("Category updated successfully.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound("Category not found.");
            }

            _context.Items.Remove(item); 
            await _context.SaveChangesAsync();

            return Ok("Deleted Successfully");
        }

    }
}
