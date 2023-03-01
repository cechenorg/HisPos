using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProductInventoryDetail : ObservableObject
    {
        #region ----- Define Variables -----
        public int ID
        {
            get { return id; }
            set { Set(() => ID, ref id, value); }
        }
        private int id;
        public string BatchNumber
        {
            get { return batchNumber; }
            set { Set(() => BatchNumber, ref batchNumber, value); }
        }
        private string batchNumber;
        public DateTime? ValidDate
        {
            get { return validDate; }
            set { Set(() => ValidDate, ref validDate, value); }
        }
        private DateTime? validDate;
        public double Price
        {
            get { return price; }
            set { Set(() => Price, ref price, value); }
        }
        private double price;
        public double ReceiveAmount
        {
            get { return receiveAmount; }
            set { Set(() => ReceiveAmount, ref receiveAmount, value); }
        }
        private double receiveAmount;
        public double Inventory
        {
            get { return inventory; }
            set { Set(() => Inventory, ref inventory, value); }
        }
        private double inventory;
        public int TypeOTC
        {
            get { return typeOTC; }
            set { Set(() => TypeOTC, ref typeOTC, value); }
        }
        private int typeOTC;
        public double ReturnStockValue
        {
            get { return returnStockValue; }
            set { Set(() => ReturnStockValue, ref returnStockValue, value); }
        }
        private double returnStockValue;
        public double ReturnAmount
        {
            get { return returnAmount; }
            set
            {
                Set(() => ReturnAmount, ref returnAmount, value);
                CalculateStockValue();
            }
        }
        private double returnAmount;

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
                returnAmount = Math.Abs(Convert.ToInt32(row["Record_Qty"]));
            if (row.Table.Columns.Contains("Record_Amt") && row["Record_Amt"] != DBNull.Value)
                returnStockValue = Convert.ToDouble(row["Record_Amt"]) * ReturnAmount;
            if(ReturnAmount != 0)
                receiveAmount = ReturnStockValue / ReturnAmount;
        }

        #region ----- Define Functions -----

        private void CalculateStockValue()
        {
            if (ReturnAmount > Inventory)
                returnAmount = Inventory;

            ReturnStockValue = Price * ReturnAmount;
        }

        #endregion ----- Define Functions -----
    }
}