using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
    public class SaleResponseDBO
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public decimal QuantitySold { get; set; }
        public DateTime SaleDate { get; set; }
        public string SoldTo { get; set; } = string.Empty;
    }
}
