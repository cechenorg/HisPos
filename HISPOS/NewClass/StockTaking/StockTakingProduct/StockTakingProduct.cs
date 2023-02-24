using His_Pos.NewClass.Person.Employee;
using System;
using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTakingProduct
{
    public class StockTakingProduct : Product.Product
    {
        #region ----- Define Variables -----

        private double inventory;

        public double Inventory
        {
            get { return inventory; }
            set
            {
                Set(() => Inventory, ref inventory, value);
                ValueDiff = NewInventory - OnTheFrame;
            }
        }

        private double newInventory;

        public double NewInventory
        {
            get { return newInventory; }
            set
            {
                Set(() => NewInventory, ref newInventory, value);
                ValueDiff = NewInventory - OnTheFrame;
                IsTakingPriceEditable = ValueDiff > 0;
            }
        }
        private string batchNum;
        public string BatchNum
        {
            get { return batchNum; }
            set
            {
                Set(() => BatchNum, ref batchNum, value);
            }
        }

        private double valueDiff;

        public double ValueDiff
        {
            get { return valueDiff; }
            set { Set(() => ValueDiff, ref valueDiff, value); }
        }

        private string note;

        public string Note
        {
            get { return note; }
            set { Set(() => Note, ref note, value); }
        }

        private bool isUpdate;

        public bool IsUpdate
        {
            get { return isUpdate; }
            set { Set(() => IsUpdate, ref isUpdate, value); }
        }

        public double OnTheFrame { get; set; }
        public bool IsFrozen { get; set; }
        public byte? IsControl { get; set; }
        public double TotalPrice { get; set; }
        public double AveragePrice { get; set; }
        public double MedBagAmount { get; set; }
        public int InvID { get; set; }
        private double newInventoryTotalPrice;

        public double NewInventoryTotalPrice
        {
            get { return newInventoryTotalPrice; }
            set
            {
                Set(() => NewInventoryTotalPrice, ref newInventoryTotalPrice, value);
            }
        }

        private double priceValueDiff;

        public double PriceValueDiff
        {
            get { return priceValueDiff; }
            set
            {
                Set(() => PriceValueDiff, ref priceValueDiff, value);
            }
        }

        private double takingPrice;

        public double TakingPrice
        {
            get { return takingPrice; }
            set
            {
                Set(() => TakingPrice, ref takingPrice, value);
            }
        }

        private bool isTakingPriceEditable;

        public bool IsTakingPriceEditable
        {
            get { return isTakingPriceEditable; }
            set
            {
                Set(() => IsTakingPriceEditable, ref isTakingPriceEditable, value);
            }
        }

        public Employee Employee { get; set; }

        #endregion ----- Define Variables -----

        public StockTakingProduct()
        {
            Employee = ChromeTabViewModel.ViewModelMainWindow.CurrentUser;
        }

        public StockTakingProduct(DataRow row) : base(row)
        {
            InvID = row.Field<int>("Inv_ID");
            Inventory = row.Field<double>("StoTakDet_OldValue");
            OnTheFrame = row.Field<double>("OnTheFrame");
            NewInventory = row.Field<double>("StoTakDet_NewValue");
            Note = row.Field<string>("StoTakDet_Note");
            IsFrozen = row.Field<bool>("Med_IsFrozen");
            IsControl = row.Field<byte?>("Med_Control");
            TotalPrice = row.Field<double>("TotalPrice");
            MedBagAmount = row.Field<double>("MedBagAmount");
            PriceValueDiff = row.Field<double>("ValueDiff");
            if (Inventory == 0)
            {
                AveragePrice = 0;
            }
            else
            {
                AveragePrice = Math.Round(TotalPrice / Inventory, 4, MidpointRounding.AwayFromZero);
            }
            TakingPrice = Math.Round(AveragePrice, 4, MidpointRounding.AwayFromZero);
            IsUpdate = false;
            NewInventoryTotalPrice = (OnTheFrame + MedBagAmount - Inventory) * AveragePrice;
        }

        #region ----- Define Functions -----

        public void GetStockTakingTotalPrice(string warID)
        {
            //NewInventoryTotalPrice = StockTakingDB.GetStockTakingTotalPrice(this, warID).Rows[0].Field<double>("TotalPrice");
            //if ((NewInventory + MedBagAmount - Inventory) > 0)
            //    NewInventoryTotalPrice += (NewInventory + MedBagAmount - Inventory) * AveragePrice;

            NewInventoryTotalPrice = Math.Round((NewInventory + MedBagAmount) * AveragePrice, 0);
        }

        #endregion ----- Define Functions -----
    }
}