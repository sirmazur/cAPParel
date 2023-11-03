using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class UserDto
    {
        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Username { get; set; }
        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Password { get; set; }
        [Required]
        public Role Role { get; set; }
        public double Saldo { get; set; }
    }
}
