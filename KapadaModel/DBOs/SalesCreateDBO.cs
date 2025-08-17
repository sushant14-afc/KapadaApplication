using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
    public class SalesCreateDBO
    {
        [Required]
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "Buyer name (SoldTo) is required.")]
        public string SoldTo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
        public decimal UnitPrice { get; set; }
    }
}
