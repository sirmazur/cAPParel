using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class ImageDto
    {
        [Required]
        public byte[] Data { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
