using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class OrderForCreationDto
    {
        public int UserId { get; set; }
        public ICollection<int> PiecesIds { get; set; } = new List<int>();
    }
}
