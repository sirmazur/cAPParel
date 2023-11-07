using cAPParel.API.Entities;

namespace cAPParel.API.Models
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public ItemType Type { get; set; }
        public Color Color { get; set;}
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
