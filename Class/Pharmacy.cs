using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class Pharmacy : Institution
    {
        public Pharmacy()
        {
        }

        public Pharmacy(string id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
        }
    }
}
