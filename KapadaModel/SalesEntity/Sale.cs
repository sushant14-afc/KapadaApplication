using KapadaModel.MainItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.SalesEntity
{
    public class Sale
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        public string? SoldTo { get; set; }  

    }
}
