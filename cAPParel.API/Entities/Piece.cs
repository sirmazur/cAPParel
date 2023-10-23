using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities
{
    public class Piece
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public Item Item { get; set; }
        [Required]
        public int ItemId { get; set; }
    }
    
}
