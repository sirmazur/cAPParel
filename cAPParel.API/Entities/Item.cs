using cAPParel.API.Entities.Hierarchy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities
{
    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public Color Color { get; set; } 
        [Required]
        public ItemType Type { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ICollection<Piece> Pieces { get; set; } = new List<Piece>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<FileData> OtherData { get; set; } = new List<FileData>();
    }
    public enum ItemType
    {
        Men,
        Women,
        Kids
    }

    public enum Color
    {
        Red,
        Blue,
        Green,
        Yellow,
        Black,
        White,
        Gray,
        Brown,
        Pink,
        Purple,
        Orange
    }
}
