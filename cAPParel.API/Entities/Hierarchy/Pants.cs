using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities.Hierarchy
{
    public class Pants : Apparel
    {
        public int W { get; set; }
        public int L { get; set; }
    }
}
