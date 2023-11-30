using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cAPParel.API.Models
{
    /// <summary>
    /// Item Category
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        /// <summary>
        /// Id of the parent category
        /// </summary>
        public int? ParentCategoryId { get; set; }
    }
}
