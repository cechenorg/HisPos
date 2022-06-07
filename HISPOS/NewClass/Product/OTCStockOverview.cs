using System.Data;

namespace His_Pos.Class.Product
{
    public class OTCStockOverview
    {
        public OTCStockOverview(DataRow row)
        {
            warId = row["PROWAR_ID"].ToString();
            InstockDate = row["INSTOCK_TIME"].ToString();
            Price = row["PRO_PRICE"].ToString();
            Amount = row["PRO_AMOUNT"].ToString();
        }

        public string warId { get; set; }
        public string InstockDate { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
    }
}