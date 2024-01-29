using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities
{
    public enum Role
    {
        Admin,
        User
    }
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Username { get; set; }
        [Required]
        [MinLength(7)]
        [MaxLength(20)]
        public string Password { get; set; }
        public string? Address { get; set; }
        [Required]
        public Role Role { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public double Saldo { get; set; } = 10;

    }
}
