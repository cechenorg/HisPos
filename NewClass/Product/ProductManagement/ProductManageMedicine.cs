using System;
using System.Data;
using His_Pos.Service;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageMedicine : Product, ICloneable
    {
        #region ----- Define Variables -----
        public bool Status { get; set; }
        public string Note { get; set; }
        public string Indication { get; set; }
        public string Warnings { get; set; }
        public string SideEffect { get; set; }
        public string BarCode { get; set; }
        public double StockValue { get; set; }
        public int? SafeAmount { get; set; }
        public int? BasicAmount { get; set; }
        public int MinOrderAmount { get; set; }
        public double LastPrice { get; set; }
        public double OnTheWayAmount { get; set; }
        public double MedBagOnTheWayAmount { get; set; }
        public double TotalOnTheWayAmount
        {
            get { return OnTheWayAmount + MedBagOnTheWayAmount; }
        }
        public double ShelfInventory { get; set; }
        public double MedBagInventory { get; set; }
        public double TotalInventory { get; set; }
        #endregion

        public ProductManageMedicine() { }

        public ProductManageMedicine(DataRow row) : base(row)
        {
            Status = row.Field<bool>("Pro_IsEnable");
            StockValue = row.Field<double>("STOCK_VALUE");
            Note = row.Field<string>("Pro_Note");
            Indication = row.Field<string>("Med_Indication");
            SideEffect = row.Field<string>("Med_SideEffect");
            BarCode = row.Field<string>("Pro_BarCode");
            Warnings = row.Field<string>("Med_Warning");
            TotalInventory = row.Field<double>("Inv_Inventory");
            ShelfInventory = row.Field<double>("SHELF_INV");
            MedBagInventory = row.Field<double>("MEDBAG_INV");
            OnTheWayAmount = row.Field<double>("Inv_OnTheWay");
            MedBagOnTheWayAmount = row.Field<double>("Inv_MedBagOnTheWay");
            SafeAmount = row.Field<int?>("Inv_SafeAmount");
            BasicAmount = row.Field<int?>("Inv_BasicAmount");
            MinOrderAmount = row.Field<int>("Pro_MinOrder");
            LastPrice = (double)row.Field<decimal>("Pro_LastPrice");
        }

        #region ----- Define Functions -----
        public object Clone()
        {
            return this.DeepCloneViaJson() as ProductManageMedicine;
        }
        public void Save()
        {
            ProductDetailDB.UpdateMedicineDetailData(this);
        }
        #endregion
    }
}
