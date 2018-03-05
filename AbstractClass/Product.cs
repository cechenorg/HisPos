using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.AbstractClass
{
    public abstract class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Cost { get; set; }
        public double Inventory { get; set; }
    }
}
