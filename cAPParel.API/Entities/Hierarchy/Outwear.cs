using System.ComponentModel.DataAnnotations.Schema;

namespace cAPParel.API.Entities.Hierarchy
{
    public class Outwear : Upper
    {
        public bool HasHood { get; set; }
    }
}
