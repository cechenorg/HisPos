using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class OTCStoreOrderOverview
    {
        public string StoreOrderDate { get; }
        public string OrderEmployee { get; }
        public string StoreReceiveDate { get; }
        public string Price { get; }
        public string Amount { get; }
        public string ManufactoryName { get; }

        public OTCStoreOrderOverview(string manufactoryName ,string storeOrderDate, string orderEmployee, string storeReceiveDate, string price, string amount)
        {
            ManufactoryName = manufactoryName;
            StoreOrderDate = storeOrderDate;
            OrderEmployee = orderEmployee;
            StoreReceiveDate = storeReceiveDate;
            Price = price;
            Amount = amount;
        }
    }
}
