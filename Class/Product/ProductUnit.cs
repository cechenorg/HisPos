using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class ProductUnit
    {
        public ProductUnit()
        {
        }

        public ProductUnit(int id, string unit, string amount, string price, string vIPPrice, string empPrice)
        {
            Id = id;
            Unit = unit;
            Amount = amount;
            Price = price;
            VIPPrice = vIPPrice;
            EmpPrice = empPrice;
        }

        public int Id { get; set; }
        public string Unit { get; set; }
        public string Amount { get; set; }
        public string Price { get; set; }
        public string VIPPrice { get; set; }
        public string EmpPrice { get; set; }
    }
}
