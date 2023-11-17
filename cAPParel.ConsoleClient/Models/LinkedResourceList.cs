using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Models
{
    public class LinkedResourceList<T>
    {
        public ICollection<T>? Value { get; set; }
        public ICollection<Link>? Links { get; set; }
    }
}
