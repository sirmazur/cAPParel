using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class CategoryForCreationDto
    {
        [Required]
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
