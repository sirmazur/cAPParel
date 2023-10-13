using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class ItemDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public ItemType Type { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<FileData> OtherData { get; set; } = new List<FileData>();
    }
}
