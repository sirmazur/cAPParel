using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities.Hierarchy
{
    public abstract class Apparel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
    }
}
