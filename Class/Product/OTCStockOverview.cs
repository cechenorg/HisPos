using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class OTCStockOverview
    {
        public OTCStockOverview(DataRow row)
        {
            warId = row["PROWAR_ID"].ToString();
            ValidDate = row["VALIDDATE"].ToString();
            Price = row["PRICE"].ToString();
            Amount = row["STOCK"].ToString();
        }
        public string warId { get; set; }
        public string ValidDate { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
    }
}
