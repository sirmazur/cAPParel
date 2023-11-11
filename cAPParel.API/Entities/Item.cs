using cAPParel.API.Entities.Hierarchy;
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
        public Color Color { get; set; } 
        [Required]
        public ItemType Type { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ICollection<Piece> Pieces { get; set; } = new List<Piece>();
        public ICollection<FileData> FileData { get; set; } = new List<FileData>();
    }
    public enum ItemType
    {
        [EnumMember(Value = "Men")]
        Men,
        [EnumMember(Value = "Women")]
        Women,
        [EnumMember(Value = "Kids")]
        Kids
    }

    public enum Color
    {
        [EnumMember(Value = "Red")]
        Red,
        [EnumMember(Value = "Blue")]
        Blue,
        [EnumMember(Value = "Green")]
        Green,
        [EnumMember(Value = "Yellow")]
        Yellow,
        [EnumMember(Value = "Black")]
        Black,
        [EnumMember(Value = "White")]
        White,
        [EnumMember(Value = "Gray")]
        Gray,
        [EnumMember(Value = "Brown")]
        Brown,
        [EnumMember(Value = "Pink")]
        Pink,
        [EnumMember(Value = "Purple")]
        Purple,
        [EnumMember(Value = "Orange")]
        Orange
    }
}
