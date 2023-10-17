using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cAPParel.API.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public ICollection<CategoryDto> ChildCategories { get; set; }
    }
}
