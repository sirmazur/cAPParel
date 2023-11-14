using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Models
{
    public class LinkedResourceList<T>
    {
        public IEnumerable<T>? Value { get; set; }
        public IEnumerable<Link>? Links { get; set; }
    }
}
