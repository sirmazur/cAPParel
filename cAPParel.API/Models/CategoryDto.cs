using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }

        public ICollection<Category> ChildCategories { get; set; }
    }
}
