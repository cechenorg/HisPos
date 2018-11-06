using System;
using System.Data;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class InventoryOtc : AbstractClass.Product, IInventory
    {
        public InventoryOtc(DataRow dataRow) : base(dataRow)
        {
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
            TypeIcon = new BitmapImage(new Uri(@"..\..\Images\OrangeDot.png", UriKind.Relative));
            StockValue = dataRow["TOTAL"].ToString();
            Location = dataRow["PRO_LOCATION"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            Stock = new InStock(dataRow);
            ProductType = new ProductType.ProductType(dataRow);
            WareHouseId = dataRow["PROWAR_ID"].ToString();
            WareHouse = dataRow["PROWAR_NAME"].ToString();
            BarCode = dataRow["PRO_BARCODE"].ToString();
        }
        public ProductType.ProductType ProductType { get; set; }
        public InStock Stock { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        public BitmapImage TypeIcon { get; set; }
        public string StockValue { get; set; }
        public string Note { get; set; }
        public string WareHouseId { get; set; }
        public string WareHouse { get; set; }
        public string BarCode { get; set; }
    }
}
