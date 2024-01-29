using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class OrderFullDto
    {
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int UserId { get; set; }
        public State State { get; set;}
        public ICollection<PieceDto> Pieces { get; set; }
        public UserDto User { get; set; }
    }
}
