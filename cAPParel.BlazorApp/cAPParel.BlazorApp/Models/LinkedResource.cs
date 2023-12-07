using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.BlazorApp.Models
{
    public class LinkedResource<T>
    {
        public T? Value { get; set; }
        public ICollection<Link> Links { get; set; } = new List<Link>();
    }
}
