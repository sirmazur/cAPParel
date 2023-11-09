using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class PieceForCreationDto
    {
        [Required]
        public string Size { get; set; }
        [Required] 
        public Color Color { get; set; }
    }
}
