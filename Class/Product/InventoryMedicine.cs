using System;
using System.Data;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class InventoryMedicine : AbstractClass.Product, IInventory, ICloneable
    {
        private InventoryMedicine() {}

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
            Indication = dataRow["PROWAR_ID"].ToString();
            SideEffect = dataRow["PROWAR_ID"].ToString();
            BarCode = dataRow["PRO_BARCODE"].ToString();
        }
        public InStock Stock { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        public bool Frozen { get; set; }
        public string Control { get; set; }
        public bool Common { get; set; }
        public BitmapImage TypeIcon { get; set; }
        public string StockValue { get; set; }
        public string Note { get; set; }
        public string Ingredient { get; set; }
        public string Indication { get; set; }
        public string SideEffect { get; set; }
        public string WareHouseId { get; set; }
        public string WareHouse { get; set; }
        public string BarCode { get; set; }
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
            newInventoryMedicine.SideEffect = SideEffect;
            newInventoryMedicine.Indication = Indication;
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
