using KapadaModel.DBOs;
using KapadaModel.SalesEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Api.DataContext;
using MyApp.Api.Service;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PdfService _pdfService;

        public SaleController(AppDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
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

            var totalPrice = saleDto.UnitPrice * inventory.Quantity;

            var sale = new Sale
            {
                InventoryId = saleDto.InventoryId,
                SoldTo = saleDto.SoldTo,
                UnitPrice = saleDto.UnitPrice,
                TotalPrice = totalPrice,
                SaleDate = DateTime.UtcNow
            };

            inventory.IsSold = true;

            _context.Sales.Add(sale);
            _context.Inventory.Update(inventory);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sale recorded successfully." });
        }

        [HttpPost("createBatchSales")]
        public async Task<IActionResult> CreateBatchSales([FromBody] List<SalesCreateDBO> sales)
        {
            if (sales == null || !sales.Any())
                return BadRequest("No sales provided.");

            foreach (var saleDto in sales)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var inventory = await _context.Inventory.FindAsync(saleDto.InventoryId);
                if (inventory == null)
                    return NotFound($"Inventory not found: {saleDto.InventoryId}");

                if (inventory.IsSold)
                    return BadRequest($"Inventory item already sold: {saleDto.InventoryId}");

                var totalPrice = saleDto.UnitPrice * inventory.Quantity;

                var sale = new Sale
                {
                    InventoryId = saleDto.InventoryId,
                    SoldTo = saleDto.SoldTo,
                    UnitPrice = saleDto.UnitPrice,
                    TotalPrice = totalPrice,
                    SaleDate = DateTime.UtcNow
                };

                inventory.IsSold = true;

                _context.Sales.Add(sale);
                _context.Inventory.Update(inventory);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Batch sales recorded successfully." });
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
                    SoldTo = s.SoldTo,
                    UnitPrice = s.UnitPrice,
                    TotalPrice = s.TotalPrice
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
        public async Task<IActionResult> GetAvailableGroupedInventory([FromQuery] string? categoryName)
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

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                inventoryItems = inventoryItems
                    .Where(i => i.CategoryName == categoryName)
                    .ToList();
            }

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
                        CategoryName = i.Category.Name,
                        RoomName = i.Room.RoomName,           
                        Quantity = i.Quantity,
                        CreatedDate = i.CreatedDate,
                    
                    })
                    .ToListAsync();

                return Ok(items);
            }

        [HttpGet("available-quantities-by-category")]
        public async Task<IActionResult> GetAvailableQuantitiesByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category is required.");

            var quantities = await _context.Inventory
                .Where(i => !i.IsSold && i.Category.Name == category)
                .Select(i => i.Quantity)
                .Distinct()
                .OrderBy(q => q)
                .ToListAsync();

            return Ok(quantities);
        }

        [HttpGet("sales-grouped")]
        public async Task<IActionResult> GetSalesGrouped()
        {
            var groupedSales = await _context.Sales
                .Include(s => s.Inventory)
                    .ThenInclude(i => i.Category)
                .Include(s => s.Inventory)
                    .ThenInclude(i => i.Room)
                .GroupBy(s => s.Inventory.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalQuantity = g.Sum(x => x.Inventory.Quantity),
                    TotalSales = g.Sum(x => x.TotalPrice),
                    Sales = g.Select(x => new SaleResponseDBO
                    {
                        Id = x.Id,
                        InventoryId = x.InventoryId,
                        CategoryName = x.Inventory.Category.Name,
                        RoomName = x.Inventory.Room.RoomName,
                        QuantitySold = x.Inventory.Quantity,
                        SaleDate = x.SaleDate,
                        SoldTo = x.SoldTo ?? "",
                        UnitPrice = x.UnitPrice,
                        TotalPrice = x.TotalPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(groupedSales);
        }

        [HttpGet("download-pdf/{buyer}")]
        public async Task<IActionResult> DownloadBuyerPdf(string buyer)
        {
            var sales = await GetSalesByBuyer(buyer);

            if (sales == null || !sales.Any())
                return NotFound("No sales found for this buyer.");

            // Generate PDF safely
            var pdfBytes = await Task.Run(() => _pdfService.GenerateBuyerSalesPdf(buyer, sales));

            return File(pdfBytes, "application/pdf", $"{buyer}_SalesReport.pdf");
        }

        // Helper: fetch sales for a buyer
        private async Task<List<SaleResponseDBO>> GetSalesByBuyer(string buyer)
        {
            // Fetch and map sales
            var sales = await _context.Sales
                .Include(s => s.Inventory)
                    .ThenInclude(i => i.Category)
                .Include(s => s.Inventory)
                    .ThenInclude(i => i.Room)
                .Where(s => s.SoldTo == buyer)
                .ToListAsync(); // first materialize the query

            // Map to DTO after fetching (safe for nulls)
            var result = sales.Select(s => new SaleResponseDBO
            {
                Id = s.Id,
                InventoryId = s.InventoryId,
                CategoryName = s.Inventory?.Category?.Name ?? "N/A",
                RoomName = s.Inventory?.Room?.RoomName ?? "N/A",
                QuantitySold = s.Inventory?.Quantity ?? 0,
                SaleDate = s.SaleDate,
                SoldTo = s.SoldTo ?? string.Empty,
                UnitPrice = s.UnitPrice,
                TotalPrice = s.TotalPrice
            }).ToList();

            return result;
        }











    }
}
