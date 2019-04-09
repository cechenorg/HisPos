using System;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Interface;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public abstract class ReturnProduct : Product, IDeletableProduct, ICloneable
    {
        #region ----- Define Variables -----
        private bool isSelected = false;
        private string batchNumber;
        private double returnAmount;
        private double realAmount;
        private double subTotal;
        private double price;
        private bool isProcessing = false;
        private ProductStartInputVariableEnum startInputVariable = ProductStartInputVariableEnum.INIT;

        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(() => IsSelected, ref isSelected, value); }
        }
        public bool IsProcessing
        {
            get { return isProcessing; }
            set
            {
                Set(() => IsProcessing, ref isProcessing, value);
                CalculateRealPrice();
            }
        }
        public double Inventory { get; set; }
        public string UnitName { get; set; }
        public double UnitAmount { get; set; }
        public int SafeAmount { get; set; }
        public string Note { get; set; }
        public string BatchNumber
        {
            get { return batchNumber; }
            set { Set(() => BatchNumber, ref batchNumber, value); }
        }
        public ProductStartInputVariableEnum StartInputVariable
        {
            get { return startInputVariable; }
            set { Set(() => StartInputVariable, ref startInputVariable, value); }
        }
        public double ReturnAmount
        {
            get { return returnAmount; }
            set
            {
                Set(() => ReturnAmount, ref returnAmount, value);
                CalculatePrice();
            }
        }
        public double RealAmount
        {
            get { return realAmount; }
            set
            {
                Set(() => RealAmount, ref realAmount, value);
                CalculateRealPrice();
            }
        }
        public double SubTotal
        {
            get { return subTotal; }
            set
            {
                if (value == 0.0)
                    SetStartInputVariable(ProductStartInputVariableEnum.INIT);
                else
                    SetStartInputVariable(ProductStartInputVariableEnum.SUBTOTAL);

                Set(() => SubTotal, ref subTotal, value);

                if (IsProcessing)
                    CalculateRealPrice();
                else
                    CalculatePrice();
            }
        }
        public double Price
        {
            get { return price; }
            set
            {
                if (value == 0.0)
                    SetStartInputVariable(ProductStartInputVariableEnum.INIT);
                else
                    SetStartInputVariable(ProductStartInputVariableEnum.PRICE);

                Set(() => Price, ref price, value);

                if (IsProcessing)
                    CalculateRealPrice();
                else
                    CalculatePrice();
            }
        }
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

        #region ----- Define Variables -----
        private void SetStartInputVariable(ProductStartInputVariableEnum startInputVariable)
        {
            StartInputVariable = startInputVariable;
        }
        private void CalculatePrice()
        {
            switch (StartInputVariable)
            {
                case ProductStartInputVariableEnum.INIT:
                    break;
                case ProductStartInputVariableEnum.PRICE:
                    subTotal = Price * ReturnAmount;
                    break;
                case ProductStartInputVariableEnum.SUBTOTAL:
                    if (ReturnAmount <= 0)
                        price = 0;
                    else
                        price = SubTotal / ReturnAmount;
                    break;
            }

            RaisePropertyChanged(nameof(Price));
            RaisePropertyChanged(nameof(SubTotal));
        }
        private void CalculateRealPrice()
        {
            switch (StartInputVariable)
            {
                case ProductStartInputVariableEnum.INIT:
                    break;
                case ProductStartInputVariableEnum.PRICE:
                    subTotal = Price * RealAmount;
                    break;
                case ProductStartInputVariableEnum.SUBTOTAL:
                    if (RealAmount <= 0)
                        price = 0;
                    else
                        price = SubTotal / RealAmount;
                    break;
            }

            RaisePropertyChanged(nameof(Price));
            RaisePropertyChanged(nameof(SubTotal));
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
        #endregion
    }
}
