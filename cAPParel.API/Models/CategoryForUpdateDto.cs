using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class CategoryForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
