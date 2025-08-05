using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapadaModel.DBOs
{
    public class RoomDBO
    {
        [Required(ErrorMessage = "Room-Name is required")]
        public string RoomName { get; set; } = string.Empty;
    }
}
