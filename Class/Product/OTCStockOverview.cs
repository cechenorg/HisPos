using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class OTCStockOverview
    {
        public OTCStockOverview(string validDate, string price, string amount)
        {
            ValidDate = validDate;
            Price = price;
            Amount = amount;
        }

        public string ValidDate { get; }
        public string Price { get; }
        public string Amount { get; }
    }
}
