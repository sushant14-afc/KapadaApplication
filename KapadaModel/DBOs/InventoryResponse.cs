using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
    public class InventoryResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;    
        public string RoomName { get; set; } = string.Empty ;
        public  decimal  Quantity { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsSold { get; set; }

        public bool IsSelected { get; set; } = false;

    }
}
