using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    class ProductPurchaseMedicine : Medicine, IProductPurchase, IDeletable, ITrade
    {
        public ProductPurchaseMedicine(DataRow dataRow, DataSource dataSource) : base(dataRow)
        {
            Stock = new InStock()
            {
                Inventory = double.Parse(dataRow["PRO_INVENTORY"].ToString()),
                SafeAmount = dataRow["PRO_SAFEQTY"].ToString(),
                BasicAmount = dataRow["PRO_BASICQTY"].ToString()
            };

            switch (dataSource)
            {
                case DataSource.PRODUCTBASICORSAFE:
                    LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
                    Amount = Int16.Parse(dataRow["PRO_BASICQTY"].ToString()) -
                             Int16.Parse(dataRow["PRO_INVENTORY"].ToString());
                    break;
                case DataSource.STOORDLIST:
                    LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
                    Price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    break;
            }
        }

        public InStock Stock { get; set; }
        public string Note { get; set; }
        public double LastPrice { get; set; }
        public string Source { get; set; }
        public double Cost { get; set; }
        public double TotalPrice { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}
