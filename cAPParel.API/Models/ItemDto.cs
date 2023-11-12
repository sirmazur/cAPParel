using cAPParel.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace cAPParel.API.Models
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double PriceMultiplier { get; set; } = 1;
        public DateTime DateCreated { get; set; }
        public ItemType Type { get; set; }
        public Color Color { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
