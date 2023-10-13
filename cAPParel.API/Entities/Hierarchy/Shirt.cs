using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities.Hierarchy
{
    public class Shirt : Upper
    {
        public bool LongSleeve { get; set; }
    }
}
