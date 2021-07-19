using System;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageStruct
    {
        public ProductManageStruct(DataRow row)
        {
            ProductType = (ProductTypeEnum)row.Field<int>("TYPE");
            ID = row.Field<string>("Pro_ID");
            WareHouseID = row.Field<int>("ProInv_WareHouseID");
            ChineseName = row.Field<string>("Pro_ChineseName");
            EnglishName = row.Field<string>("Pro_EnglishName");
            Inventory = row.Field<double>("Inv_Inventory");
            ShelfAmount = row.Field<double>("SHELF_INV");
            MedBagAmount = row.Field<double>("Inv_MedBagAmount");
            SafeAmount = row.Field<int>("Inv_SafeAmount");
            BasicAmount = row.Field<int>("Inv_BasicAmount");
            OnTheWayAmount = row.Field<double>("Inv_OnTheWay");
            MedBagOnTheWayAmount = row.Field<double>("Inv_MedBagOnTheWay");
            IsCommon = row.Field<bool>("Pro_IsCommon");
            IsFrozen = row.Field<bool>("Med_IsFrozen");
            ControlLevel = row.Field<byte?>("Med_Control");
            StockValue = row.Field<double>("STOCK_VALUE");
            ShelfStockValue = row.Field<double>("SHELF_STOCK_VALUE");
            IsEnable = row.Field<bool>("Pro_IsEnable");
            InventoryError = row.Field<bool>("ERROR_FLAG");
            IsZero = row.Field<decimal>("NHIMED_PRICE");
            DepRec_Amount = row.Field<int>("DepRec_Amount");
            SINGINV = row.Field<int>("inv_qty");
            ProLoc_Name = row.Field<string>("ProLoc_Name");

            IsMerged = row.Field<int?>("Cnt") == null ? false : true;
        }

        public ProductTypeEnum ProductType { get; set; }
        public string ID { get; set; }
        public string ProLoc_Name { get; set; }
        public int WareHouseID { get; set; }
        public int DepRec_Amount { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(EnglishName))
                    return (EnglishName.Contains(" ") ? EnglishName.Substring(0, EnglishName.IndexOf(" ")) : EnglishName) + ChineseName;
                return !string.IsNullOrEmpty(ChineseName) ? ChineseName : string.Empty;
            }
        }

        public int SINGINV { get; set; }
        public double Inventory { get; set; }
        public double ShelfAmount { get; set; }
        public double MedBagAmount { get; set; }
        public int SafeAmount { get; set; }
        public int BasicAmount { get; set; }
        public double OnTheWayAmount { get; set; }
        public double MedBagOnTheWayAmount { get; set; }
        public double AllOnTheWayAmount { get { return OnTheWayAmount + MedBagOnTheWayAmount; } }
        public double StockValue { get; set; }
        public double ShelfStockValue { get; set; }
        public int? ControlLevel { get; set; }
        public bool IsCommon { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsEnable { get; set; }
        public bool InventoryError { get; set; }
        public decimal IsZero { get; set; }

        // 子品項
        public bool IsMerged { get; set; }
    }
}