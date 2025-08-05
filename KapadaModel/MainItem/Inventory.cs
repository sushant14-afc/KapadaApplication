using KapadaModel.Item;
using KapadaModel.Room;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.MainItem
{
    public class Inventory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        public ItemCategory Category { get; set; }  
        public int RoomId { get; set; }

        public RoomItem Room { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Quantity { get; set; }

        public bool IsSold { get; set; } = false;
    }
}
