using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class Copayment : Selection
    {
        public Copayment(string id,string name)
        {
            Id = id;
            Name = name;
        }

        public double Point { get; set; }
    }
}
