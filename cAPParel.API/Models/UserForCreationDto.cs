using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class UserForCreationDto
    {
        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Username { get; set; }
        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
