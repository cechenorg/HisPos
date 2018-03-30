using System;
using System.Data;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.Product
{
    public class Otc : AbstractClass.Product
    {
        public Otc(string id, string name, string price, double inventory)
        {
            Id = id;
            Name = name;
            Price = double.Parse(price);
            Inventory = inventory;
        }

        public Otc(DataRow dataRow, DataSource dataSource)
        {
            switch(dataSource)
            {
                case DataSource.OTC:
                    TypeIcon = new BitmapImage(new Uri(@"..\Images\PosDot.png", UriKind.Relative));
                    StockValue = dataRow["TOTAL"].ToString();
                    break;
                case DataSource.STOORDLIST:
                    LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());
                    Price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
                    Total = Int32.Parse(dataRow["STOORDDET_QTY"].ToString());
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    break;
            }
            
            Id = dataRow["PRO_ID"].ToString();
            Name = dataRow["PRO_NAME"].ToString();
            Inventory = double.Parse(dataRow["PRO_INVENTORY"].ToString());
            SafeAmount = dataRow["PRO_SAFEQTY"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            BasicAmount = dataRow["PRO_BASICQTY"].ToString();
            Location = dataRow["PRO_LOCATION"].ToString();
        }

        public int Total { get; set; }//商品數量
        public double TotalPrice { get; set; }//商品總價
    }
}
