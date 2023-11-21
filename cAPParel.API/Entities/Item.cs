using cAPParel.API.Entities.Hierarchy;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

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
        public double PriceMultiplier { get; set; } = 1;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        [Required]
        public Color Color { get; set; } 
        [Required]
        public ItemType Type { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public ICollection<Piece> Pieces { get; set; } = new List<Piece>();
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public ICollection<FileData> FileData { get; set; } = new List<FileData>();
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
