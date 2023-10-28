namespace cAPParel.API.Models
{
    public class CategoryFullDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public ICollection<CategoryDto> ChildCategories { get; set; }
    }
}
