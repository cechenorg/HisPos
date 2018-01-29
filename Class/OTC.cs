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
        public Otc(string id, string name, string price, string inventory)
        {
            Id = id;
            Name = name;
            Price = double.Parse(price);
            Inventory = inventory;
        }

        public Otc()
        {
        }

        public int Total { get; set; }//商品數量
        public double TotalPrice { get; set; }//商品總價
    }
}
