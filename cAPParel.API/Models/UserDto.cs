using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class UserDto
    {

        public int Id { get; set; }
        public string Username { get; set; }
        public string? Address { get; set; }

    }
}
