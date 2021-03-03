using His_Pos.Interface;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.Product
{
    public class InventoryOtc : AbstractClass.Product, IInventory, INotifyPropertyChanged
    {
        public InventoryOtc(DataRow dataRow) : base(dataRow)
        {
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
            TypeIcon = new BitmapImage(new Uri(@"..\..\Images\OrangeDot.png", UriKind.Relative));
            StockValue = dataRow["TOTAL"].ToString();
            Location = dataRow["PRO_LOCATION"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            Stock = new InStock(dataRow);
            WareHouseId = dataRow["PROWAR_ID"].ToString();
            WareHouse = dataRow["PROWAR_NAME"].ToString();
            BarCode = dataRow["PRO_BARCODE"].ToString();
        }

        public InStock Stock { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        public BitmapImage TypeIcon { get; set; }
        public string Note { get; set; }
        public string WareHouseId { get; set; }
        public string WareHouse { get; set; }
        public string BarCode { get; set; }
        private string stockValue;

        public string StockValue
        {
            get
            {
                return stockValue;
            }
            set
            {
                stockValue = value;
                NotifyPropertyChanged("StockValue");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}