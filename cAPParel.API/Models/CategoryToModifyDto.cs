using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public abstract class CategoryToModifyDto : IValidatableObject
    {  
        /// <summary>
        /// Category name has to start with a capital letter
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CategoryName[0] != char.ToUpper(CategoryName[0]))
            {
                yield return new ValidationResult("Category name has to start with a capital letter.", new[] { $"{this.GetType().Name}" });
            }
        }
    }
}
