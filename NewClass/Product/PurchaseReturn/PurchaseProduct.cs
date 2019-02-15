using System;
using System.Data;
using His_Pos.Interface;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProduct : Product, IDeletableProduct
    {
        #region ----- Define Variables -----
        private bool isSelected = false;

        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(() => IsSelected, ref isSelected, value); }
        }
        public double Inventory { get; }
        public string UnitName { get; set; }
        public double UnitAmount { get; set; }
        public int SafeAmount { get; }
        public int BasicAmount { get; }
        public double OnTheWayAmount { get; }
        public double LastPrice { get; }
        public double OrderAmount { get; set; }
        public double RealAmount { get; set; }
        public double FreeAmount { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        public string Invoice { get; set; }
        public DateTime? ValidDate { get; set; }
        public string BatchNumber { get; set; }
        public string Note { get; set; }

        public bool IsFirstBatch { get; set; } = true;
        public int SingdePackageAmount { get; } 
        public double SingdePackagePrice { get; }
        public double SingdePrice { get; }
        #endregion

        public PurchaseProduct() : base() {}

        public PurchaseProduct(DataRow dataRow) : base(dataRow)
        {
            Inventory = dataRow.Field<double>("Inv_Inventory");
            SafeAmount = dataRow.Field<int>("Inv_SafeAmount");
            BasicAmount = dataRow.Field<int>("Inv_BasicAmount");
            OnTheWayAmount = dataRow.Field<double>("Inv_OnTheWay");
            LastPrice = (double)dataRow.Field<decimal>("Pro_LastPrice");
            UnitName = dataRow.Field<string>("StoOrdDet_UnitName");
            UnitAmount = dataRow.Field<double>("StoOrdDet_UnitAmount");
            OrderAmount = dataRow.Field<double> ("StoOrdDet_OrderAmount");
            RealAmount = dataRow.Field<double>("StoOrdDet_RealAmount");
            FreeAmount = dataRow.Field<double>("StoOrdDet_FreeAmount");
            Price = (double)dataRow.Field<decimal>("StoOrdDet_Price");
            SubTotal = (double)dataRow.Field<decimal>("StoOrdDet_SubTotal");
            Invoice = dataRow.Field<string>("StoOrdDet_Invoice");
            ValidDate = dataRow.Field<DateTime?>("StoOrdDet_ValidDate");
            BatchNumber = dataRow.Field<string>("StoOrdDet_BatchNumber");
            Note = dataRow.Field<string>("StoOrdDet_Note");

            SingdePackageAmount = dataRow.Field<int>("SinData_PackageAmount");
            SingdePackagePrice = (double)dataRow.Field<decimal>("SinData_PackagePrice");
            SingdePrice = (double)dataRow.Field<decimal>("SinData_SinglePrice");
        }

        public void CopyOldProductData(PurchaseProduct purchaseProduct)
        {
            OrderAmount = purchaseProduct.OrderAmount;
            RealAmount = purchaseProduct.RealAmount;
            FreeAmount = purchaseProduct.FreeAmount;
            Price = purchaseProduct.Price;
            SubTotal = purchaseProduct.SubTotal;
            Invoice = purchaseProduct.Invoice;
            ValidDate = purchaseProduct.ValidDate;
            BatchNumber = purchaseProduct.BatchNumber;
            Note = purchaseProduct.Note;
        }
    }
}
