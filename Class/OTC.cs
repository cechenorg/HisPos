using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class Otc : Product
    {
        public Otc(string id, string name, double price, double inventory)
        {
            Id = id;
            Name = name;
            Price = price;
            Inventory = inventory;
        }
    }
}
