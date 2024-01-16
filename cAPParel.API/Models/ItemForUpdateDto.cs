using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class ItemForUpdateDto
    {
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public double PriceMultiplier { get; set; }
        public Color Color { get; set; }
        
        public ItemType Type { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        public int CategoryId { get; set; }

        public ICollection<PieceForCreationDto> Pieces { get; set; }
        public ICollection<FileDataForCreationDto> FileData { get; set; }
    }
}
