using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
    public class GroupedSaleDBO
    {
        public string Category { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal TotalSales { get; set; }
        public List<SaleResponseDBO> Sales { get; set; } = new();
    }
}
