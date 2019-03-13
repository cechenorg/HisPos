using System;
using System.Data;
using His_Pos.Interface;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public abstract class ReturnProduct : Product, IDeletableProduct, ICloneable
    {
        #region ----- Define Variables -----
        private bool isSelected = false;

        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(() => IsSelected, ref isSelected, value); }
        }
        public double Inventory { get; set; }
        public string UnitName { get; set; }
        public double UnitAmount { get; set; }
        public int SafeAmount { get; set; }
        public string Note { get; set; }
        public string BatchNumber { get; set; }
        public double ReturnAmount { get; set; }
        public double RealAmount { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        public DateTime? ValidDate { get; set; }
        #endregion

        public ReturnProduct() : base() {}

        public ReturnProduct(DataRow row) : base(row)
        {
            Inventory = row.Field<double>("Inv_Inventory");
            UnitName = row.Field<string>("StoOrdDet_UnitName");
            UnitAmount = row.Field<double>("StoOrdDet_UnitAmount");
            SafeAmount = row.Field<int>("Inv_SafeAmount");
            Note = row.Field<string>("StoOrdDet_Note");
            BatchNumber = row.Field<string>("StoOrdDet_BatchNumber");
            ReturnAmount = row.Field<double>("StoOrdDet_OrderAmount");
            RealAmount = row.Field<double>("StoOrdDet_RealAmount");
            Price = (double)row.Field<decimal>("StoOrdDet_Price");
            SubTotal = (double)row.Field<decimal>("StoOrdDet_SubTotal");
            ValidDate = row.Field<DateTime?>("StoOrdDet_ValidDate");
        }

        public void CopyOldProductData(ReturnProduct returnProduct)
        {
            Inventory = returnProduct.Inventory;
            UnitName = returnProduct.UnitName;
            UnitAmount = returnProduct.UnitAmount;
            SafeAmount = returnProduct.SafeAmount;
            Note = returnProduct.Note;
            BatchNumber = returnProduct.BatchNumber;
            ReturnAmount = returnProduct.ReturnAmount;
            RealAmount = returnProduct.RealAmount;
            Price = returnProduct.Price;
            SubTotal = returnProduct.SubTotal;
            ValidDate = returnProduct.ValidDate;
        }

        public abstract object Clone();
    }
}
