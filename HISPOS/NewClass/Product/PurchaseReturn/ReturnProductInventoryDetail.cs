using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProductInventoryDetail : ObservableObject
    {
        #region ----- Define Variables -----

        private double returnAmount;
        private double returnStockValue;

        public int ID { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ValidDate { get; set; }
        public double Price { get; set; }

        public double ReceiveAmount { get; set; }

        public double Inventory { get; set; }

        public int TypeOTC { get; set; }

        public double ReturnStockValue
        {
            get { return returnStockValue; }
            set { Set(() => ReturnStockValue, ref returnStockValue, value); }
        }

        public double ReturnAmount
        {
            get { return returnAmount; }
            set { 
                Set(() => ReturnAmount, ref returnAmount, value);
                CalculateStockValue();
            }
        }

        #endregion ----- Define Variables -----

        public ReturnProductInventoryDetail(DataRow row)
        {
            TypeOTC = row.Field<int>("Pro_TypeID");
            ID = row.Field<int>("InvDet_ID");
            BatchNumber = row.Field<string>("InvDet_BatchNumber");
            ValidDate = row.Field<DateTime?>("InvDet_ValidDate");
            Price = (double)row.Field<decimal>("InvDet_Price");
            Inventory = row.Field<double>("InvDet_Amount");
            if (row.Table.Columns.Contains("Record_Qty") && row["Record_Qty"] != DBNull.Value)
                ReturnAmount = Math.Abs(Convert.ToInt32(row["Record_Qty"]));
            if (row.Table.Columns.Contains("Record_Amt") && row["Record_Amt"] != DBNull.Value)
                ReturnStockValue = Convert.ToDouble(row["Record_Amt"]);
            if(ReturnAmount != 0)
                ReceiveAmount = ReturnStockValue / ReturnAmount;
        }

        #region ----- Define Functions -----

        private void CalculateStockValue()
        {
            ReturnStockValue = Price * ReturnAmount;
        }

        #endregion ----- Define Functions -----
    }
}