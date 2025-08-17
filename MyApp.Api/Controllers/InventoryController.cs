using KapadaModel.DBOs;
using KapadaModel.MainItem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Api.DataContext;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context; 
        }

        [HttpPost("createInventory")]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryCreateDBO _inventory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = new Inventory
            {
                CategoryId = _inventory.CategoryId.Value,
                RoomId = _inventory.RoomId.Value,
                Quantity = _inventory.Quantity.Value,
                IsSold = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            return Ok("Inventory created successfully");
        }

        //[HttpPut("edit/{id}")]
        //public async Task<IActionResult> EditInventory(int id, [FromBody] InventoryCreateDBO _editData)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var inventory = await _context.Inventory.FindAsync(id);
        //    if (inventory == null)
        //        return NotFound("Inventory not found");

            
        //    inventory.CategoryId = _editData.CategoryId.Value;
        //    inventory.RoomId = _editData.RoomId.Value;
        //    inventory.Quantity = _editData.Quantity.Value;
            

        //    await _context.SaveChangesAsync();

        //    return Ok("Inventory updated successfully");
        //}

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditInventory(int id, [FromBody] InventoryCreateDBO _editData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
                return NotFound("Inventory not found");

            inventory.CategoryId = _editData.CategoryId!.Value;
            inventory.RoomId = _editData.RoomId!.Value;
            inventory.Quantity = _editData.Quantity!.Value;
            // Note: CreatedAt remains unchanged

            await _context.SaveChangesAsync();

            return Ok("Inventory updated successfully");
        }


        [HttpGet("getInventories")]
        public async Task<IActionResult> GetInventories()
        {
            var inventoryData = await _context.Inventory
                .Include(i => i.Category)
                .Include(i => i.Room)
                .Select(i => new InventoryResponse
                {
                    Id = i.Id,
                    CategoryName = i.Category.Name,  
                    RoomName = i.Room.RoomName,           
                    Quantity = i.Quantity,
                    CreatedDate = i.CreatedDate,
                    IsSold = i.IsSold
                })
                .ToListAsync();

            return Ok(inventoryData);
        }

        [HttpDelete("deleteInventory/{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound("Inventory record not found.");
            }

            _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();

            return Ok("Inventory deleted successfully.");


        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<InventoryCreateDBO>> GetInventoryById(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
                return NotFound();

            return new InventoryCreateDBO
            {
                CategoryId = inventory.CategoryId,
                RoomId = inventory.RoomId,
                Quantity = inventory.Quantity
            };
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Filter(DateTime? date, string? room, string? category)
        {
            var query = _context.Inventory
                .Include(i => i.Category)
                .Include(i => i.Room)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(i => i.CreatedDate.Date == date.Value.Date);

            if (!string.IsNullOrWhiteSpace(room))
                query = query.Where(i => i.Room.RoomName == room);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(i => i.Category.Name == category);

            var result = await query
                .Select(i => new InventoryResponse
                {
                    Id = i.Id,
                    CategoryName = i.Category.Name,
                    RoomName = i.Room.RoomName,
                    Quantity = i.Quantity,
                    CreatedDate = i.CreatedDate,
                    IsSold = i.IsSold
                })
                .ToListAsync();

            return Ok(result);
        }

        //[HttpGet("available-grouped/summary")]
        //public async Task<IActionResult> GetGroupedInventoryCategorySummary([FromQuery] string? category)
        //{
        //    var query = _context.Inventory
        //        .Where(i => !i.IsSold)
        //        .Include(i => i.Category)
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(category))
        //    {
        //        query = query.Where(i => i.Category.Name == category);
        //    }

        //    var categorySummary = await query
        //        .GroupBy(i => i.Category.Name)
        //        .Select(g => new GroupedInventorySummary
        //        {
        //            Category = g.Key,
        //            TotalQuantity = g.Sum(x => x.Quantity)
        //        })
        //        .ToListAsync();

        //    return Ok(categorySummary);
        //}

        [HttpGet("available-quantities")]
        public async Task<IActionResult> GetAvailableQuantities()
        {
            var quantities = await _context.Inventory
                .Where(i => !i.IsSold)
                .Select(i => i.Quantity)
                .Distinct()
                .OrderBy(q => q)
                .ToListAsync();

            return Ok(quantities);
        }




    }
}
