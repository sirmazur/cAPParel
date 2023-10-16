using cAPParel.API.Entities;

namespace cAPParel.API.Models
{
    public class ItemForCreationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
        public ItemType Type { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ICollection<ImageForCreationDto> Images { get; set; }
        public ICollection<FileDataForCreationDto> OtherData { get; set; }
    }
}
