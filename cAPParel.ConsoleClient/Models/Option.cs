using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAPParel.ConsoleClient.Models
{
    public class Option
    {
        public string Name { get; }
        public Func<Task> Selected { get; }

        public Option(string name, Func<Task> selected)
        {
            Name = name;
            Selected = selected;
        }
    }
}
