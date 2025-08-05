using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
    public class GroupedInventoryResponse
    {
        public string Category { get; set; } = "";
        public List<RoomQuantityDBO> Rooms { get; set; } = new();
    }
}
