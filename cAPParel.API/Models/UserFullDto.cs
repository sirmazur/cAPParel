using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class UserFullDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public Role Role { get; set; }

        public double Saldo { get; set; }

        public ICollection<OrderDto> Orders { get; set; }
		public string? Address { get; set; }
	}
}
