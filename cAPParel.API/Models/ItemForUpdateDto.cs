using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class ItemForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public ItemType Type { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public ICollection<PieceForCreationDto> Pieces { get; set; }
        public ICollection<FileDataForCreationDto> FileData { get; set; }
    }
}
