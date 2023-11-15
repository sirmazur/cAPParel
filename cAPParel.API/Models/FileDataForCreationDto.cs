using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class FileDataForCreationDto
    {
        [Required]
        public byte[] Data { get; set; }
        [Required]
        public string Description { get; set; }
        public Entities.DataType Type { get; set; }
    }
}
