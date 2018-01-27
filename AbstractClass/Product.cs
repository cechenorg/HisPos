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
        public string Price { get; set; }
        public string Cost { get; set; }
        public string Inventory { get; set; }
    }
}
