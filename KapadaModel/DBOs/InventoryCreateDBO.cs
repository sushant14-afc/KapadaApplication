using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
  
    public class InventoryCreateDBO
    {
        [Required(ErrorMessage = "Please Select the Item-Name")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Please Select the Room")]
        public int? RoomId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public decimal? Quantity { get; set; }
    }

}
