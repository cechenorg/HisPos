using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class ProductPurchaseOtc : Otc, IProductPurchase, IDeletable, ITrade
    {
        public ProductPurchaseOtc(DataRow dataRow, DataSource dataSource) : base(dataRow)
        {
            switch (dataSource)
            {
                case DataSource.STOORDLIST:
                    LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
                    Price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    break;
                case DataSource.PRODUCTBASICORSAFE:
                    LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
                    Amount = Int16.Parse(dataRow["PRO_BASICQTY"].ToString()) -
                             Int16.Parse(dataRow["PRO_INVENTORY"].ToString());
                    break;
            }
        }

        public double LastPrice { get; set; }
        public string Source { get; set; }
        public double Cost { get; set; }
        public double TotalPrice { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}
