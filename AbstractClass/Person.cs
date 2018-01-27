using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.AbstractClass
{
    public abstract class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IcNumber { get; set; }
        public string Birthday { get; set; }
    }
}
