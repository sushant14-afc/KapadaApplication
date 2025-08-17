using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
    public class GroupedInventorySummary
    {
        public string Category { get; set; } = "";
        public int TotalQuantity { get; set; }
    }
}
