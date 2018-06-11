using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class InventoryOtc : AbstractClass.Product, IInventory
    {
        public InventoryOtc(DataRow dataRow) : base(dataRow)
        {
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
            TypeIcon = new BitmapImage(new Uri(@"..\..\Images\PosDot.png", UriKind.Relative));
            StockValue = dataRow["TOTAL"].ToString();
            Location = dataRow["PRO_LOCATION"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            Stock = new InStock()
            {
                Inventory = double.Parse(dataRow["PRO_INVENTORY"].ToString()),
                SafeAmount = dataRow["PRO_SAFEQTY"].ToString(),
                BasicAmount = dataRow["PRO_BASICQTY"].ToString()
            };
            ProductType = new ProductType.ProductType(dataRow);
        }
        public ProductType.ProductType ProductType { get; set; }
        public InStock Stock { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        public BitmapImage TypeIcon { get; set; }
        public string StockValue { get; set; }
        public string Note { get; set; }
    }
}
