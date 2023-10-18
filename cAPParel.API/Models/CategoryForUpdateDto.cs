using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class CategoryForUpdateDto
    {
        public string? CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
