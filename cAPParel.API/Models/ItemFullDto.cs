using cAPParel.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class ItemFullDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double PriceMultiplier { get; set; }
        public DateTime DateCreated { get; set; }
        public ItemType Type { get; set; }
        public Color Color { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public ICollection<PieceDto> Pieces { get; set; }
        public ICollection<FileDataDto> FileData { get; set; }
    }
}
