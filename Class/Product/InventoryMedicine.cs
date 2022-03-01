using His_Pos.Interface;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Media.Imaging;

namespace His_Pos.Class.Product
{
    public class InventoryMedicine : AbstractClass.Product, IInventory, ICloneable, INotifyPropertyChanged
    {
        private InventoryMedicine()
        {
        }

        public InventoryMedicine(DataRow dataRow) : base(dataRow)
        {
            Status = dataRow["PRO_STATUS"].ToString().Equals("1");
            TypeIcon = new BitmapImage(new Uri(@"..\..\Images\BlueDot.png", UriKind.Relative));
            StockValue = dataRow["TOTAL"].ToString();
            Location = dataRow["PRO_LOCATION"].ToString();
            Note = dataRow["PRO_DESCRIPTION"].ToString();
            Stock = new InStock(dataRow);
            Ingredient = dataRow["HISMED_INGREDIENT"].ToString();
            Frozen = dataRow["HISMED_FROZ"].ToString().Equals("True");
            Control = dataRow["HISMED_CONTROL"].ToString();
            Common = dataRow["HISMED_COMMON"].ToString().Equals("True");
            WareHouse = dataRow["PROWAR_NAME"].ToString();
            WareHouseId = dataRow["PROWAR_ID"].ToString();
            Indication = dataRow["HISMED_INDICATION"].ToString();
            SideEffect = dataRow["HISMED_SIDEFFECT"].ToString();
            BarCode = dataRow["PRO_BARCODE"].ToString();
            Warnings = dataRow["HISMED_NOTE"].ToString();
        }

        public InStock Stock { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        public bool Frozen { get; set; }
        public string Control { get; set; }
        public bool Common { get; set; }
        public BitmapImage TypeIcon { get; set; }
        public string Note { get; set; }
        public string Ingredient { get; set; }
        public string Indication { get; set; }
        public string Warnings { get; set; }
        public string SideEffect { get; set; }
        public string WareHouseId { get; set; }
        public string WareHouse { get; set; }
        public string BarCode { get; set; }

        public bool IsControl
        {
            get { return !(Control.Equals("0") || Control.Equals("")); }
        }

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

        public object Clone()
        {
            InventoryMedicine newInventoryMedicine = new InventoryMedicine();

            newInventoryMedicine.Id = Id;
            newInventoryMedicine.Name = Name;
            newInventoryMedicine.ChiName = ChiName;
            newInventoryMedicine.EngName = EngName;
            newInventoryMedicine.Status = Status;
            newInventoryMedicine.WareHouse = WareHouse;
            newInventoryMedicine.WareHouseId = WareHouseId;
            newInventoryMedicine.BarCode = BarCode;
            newInventoryMedicine.SideEffect = SideEffect;
            newInventoryMedicine.Indication = Indication;
            newInventoryMedicine.Warnings = Warnings;
            newInventoryMedicine.Note = Note;
            newInventoryMedicine.StockValue = StockValue;
            newInventoryMedicine.TypeIcon = TypeIcon;
            newInventoryMedicine.Common = Common;
            newInventoryMedicine.Control = Control;
            newInventoryMedicine.Frozen = Frozen;
            newInventoryMedicine.Location = Location;
            newInventoryMedicine.Stock = ((ICloneable)Stock).Clone() as InStock;

            return newInventoryMedicine;
        }
    }
}