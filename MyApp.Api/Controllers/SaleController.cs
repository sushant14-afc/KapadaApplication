using KapadaModel.DBOs;
using KapadaModel.SalesEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Api.DataContext;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SaleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("createSale")]
        public async Task<IActionResult> CreateSale([FromBody] SalesCreateDBO saleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = await _context.Inventory.FindAsync(saleDto.InventoryId);
            if (inventory == null)
                return NotFound("Inventory not found.");

            if (inventory.IsSold)
                return BadRequest("This inventory item has already been sold.");

            var sale = new Sale
            {
                InventoryId = saleDto.InventoryId,
                SoldTo = saleDto.SoldTo,
                SaleDate = DateTime.UtcNow
            };

            inventory.IsSold = true;

            _context.Sales.Add(sale);
            _context.Inventory.Update(inventory);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sale recorded successfully." });
        }


        [HttpGet("getAllSale")]
        public async Task<ActionResult<List<SaleResponseDBO>>> GetAllSales()
        {
            var sales = await _context.Sales
                .Include(s => s.Inventory)
                    .ThenInclude(i => i.Category)
                .Include(s => s.Inventory)
                    .ThenInclude(i => i.Room)
                .Select(s => new SaleResponseDBO
                {
                    Id = s.Id,
                    InventoryId = s.InventoryId,
                    CategoryName = s.Inventory.Category.Name,
                    RoomName = s.Inventory.Room.RoomName,
                    QuantitySold = s.Inventory.Quantity,
                    SaleDate = s.SaleDate,
                    SoldTo = s.SoldTo
                })
                .ToListAsync();

            return Ok(sales);
        }

        [HttpGet("available")]
        public async Task<ActionResult<List<InventoryResponse>>> GetAvailableInventory()
        {
            var inventories = await _context.Inventory
                .Where(i => !i.IsSold)
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

            return Ok(inventories);
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

        [HttpGet("available-grouped")]
        public async Task<IActionResult> GetAvailableGroupedInventory()
        {
            var inventoryItems = await _context.Inventory
                .Where(i => !i.IsSold)
                .Include(i => i.Category)
                .Include(i => i.Room)
                .Select(i => new
                {
                    CategoryName = i.Category.Name,
                    RoomName = i.Room.RoomName,
                    Quantity = i.Quantity
                })
                .ToListAsync();

            var groupedInventory = inventoryItems
                .GroupBy(item => item.CategoryName)
                .Select(group => new
                {
                    Category = group.Key,
                    Rooms = group
                        .GroupBy(g => g.RoomName)
                        .Select(rg => new
                        {
                            RoomName = rg.Key,
                            TotalQuantity = rg.Sum(x => x.Quantity)
                        })
                        .ToList()
                })
                .ToList();

            return Ok(groupedInventory);
        }

        //[HttpGet("category-items")]
        //public async Task<IActionResult> GetItemsByCategory(string category)
        //{
        //    var items = await _context.Inventory
        //        .Include(i => i.Category)  // Include related category
        //        .Where(i => i.Category.Name == category && !i.IsSold)
        //        .ToListAsync();

        //    return Ok(items);
        //}

        [HttpGet("category-items")]
        public async Task<IActionResult> GetItemsByCategory(string category)
        {
            var items = await _context.Inventory
                .Include(i => i.Category)
                .Include(i => i.Room)  
                .Where(i => i.Category.Name == category && !i.IsSold)
                .Select(i => new InventoryResponse   
                {
                    Id = i.Id,
                    RoomName = i.Room.RoomName,           
                    Quantity = i.Quantity,
                    CreatedDate = i.CreatedDate,
                    
                })
                .ToListAsync();

            return Ok(items);
        }






    }
}
