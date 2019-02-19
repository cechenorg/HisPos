using System;
using System.Data;
using His_Pos.Service;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageMedicine : Product, ICloneable
    {
        #region ----- Define Variables -----
        public bool Status { get; set; }
        public bool IsCommon { get; set; }
        public string Note { get; set; }
        public string Indication { get; set; }
        public string Warnings { get; set; }
        public string SideEffect { get; set; }
        public string BarCode { get; set; }
        public double StockValue { get; set; }
        public double Inventory { get; set; }
        public int? SafeAmount { get; set; }
        public int? BasicAmount { get; set; }
        public int MinOrderAmount { get; set; }
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
            Inventory = row.Field<double>("Inv_Inventory");
            SafeAmount = row.Field<int?>("Inv_SafeAmount");
            BasicAmount = row.Field<int?>("Inv_BasicAmount");
            MinOrderAmount = row.Field<int>("Pro_MinOrder");
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
