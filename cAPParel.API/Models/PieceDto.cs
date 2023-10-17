using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class PieceDto
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public Item Item { get; set; }
        public int ItemId { get; set; }
    }
}
