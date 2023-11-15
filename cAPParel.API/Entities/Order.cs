using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities
{
    public enum State
    {
        Accepted,
        Ongoing,
        Completed
    }
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateCompleted { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public State State { get; set; } = State.Accepted;
        public ICollection<Piece> Pieces { get; set; } = new List<Piece>();
    }
}
