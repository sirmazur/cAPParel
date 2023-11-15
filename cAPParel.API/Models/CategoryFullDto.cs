namespace cAPParel.API.Models
{
    public class CategoryFullDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public ICollection<CategoryFullDto> ChildCategories { get; set; }
    }
}
