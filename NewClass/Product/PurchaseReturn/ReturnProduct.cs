using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using System;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public abstract class ReturnProduct : Product, ICloneable
    {
        #region ----- Define Variables -----

        private bool isSelected = false;
        private double returnAmount;
        private double realAmount;
        private double subTotal;
        private double price;
        private double returnStockValue;
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

        public int WareHouseID { get; set; }
        public int InvID { get; set; }
        public double Inventory { get; set; }
        public string UnitName { get; set; }
        public double UnitAmount { get; set; }
        public int SafeAmount { get; set; }
        public string BatchNumber { get; set; }
        public string Note { get; set; }
        public int type;

        public int TypeOTC
        {
            get { return type; }
            set { Set(() => TypeOTC, ref type, value); }
        }

        public double ReturnStockValue
        {
            get { return returnStockValue; }
            set { Set(() => ReturnStockValue, ref returnStockValue, value); }
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
                SetReturnInventoryDetail();
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
                if (RealAmount > ReturnAmount) {
                    MessageWindow.ShowMessage($"實際退貨量不可大於預定量!", MessageType.ERROR);
                    RealAmount = 0;
                }
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

        public ReturnProductInventoryDetails InventoryDetailCollection { get; set; } = new ReturnProductInventoryDetails();

        #endregion ----- Define Variables -----

        public ReturnProduct() : base()
        {
        }

        public ReturnProduct(DataRow row) : base(row)
        {
            TypeOTC = row.Field<int>("Pro_TypeID");
            WareHouseID = row.Field<int>("ProInv_WareHouseID");
            InvID = row.Field<int>("Inv_ID");
            Inventory = row.Field<double>("Inv_Inventory");
            UnitName = row.Field<string>("StoOrdDet_UnitName");
            UnitAmount = row.Field<double>("StoOrdDet_UnitAmount");
            SafeAmount = row.Field<int>("Inv_SafeAmount");
            Note = row.Field<string>("StoOrdDet_Note");
            returnAmount = row.Field<double>("StoOrdDet_OrderAmount");
            realAmount = row.Field<double>("StoOrdDet_RealAmount");
            BatchNumber = row.Field<string>("StoOrdDet_BatchNumber");
            price = (double)row.Field<decimal>("StoOrdDet_Price");
            subTotal = (double)row.Field<decimal>("StoOrdDet_SubTotal");

            InventoryDetailCollection.Add(new ReturnProductInventoryDetail(row));
        }

        #region ----- Define Variables -----

        public void SetReturnInventoryDetail()
        {
            double returnAmountTemp = ReturnAmount;

            if (returnAmountTemp > Inventory)
                returnAmountTemp = Inventory;

            InventoryDetailCollection.ClearReturnValue();

            foreach (var detail in InventoryDetailCollection)
            {
                if (detail.Inventory <= returnAmountTemp)
                {
                    detail.ReturnAmount = detail.Inventory;

                    returnAmountTemp -= detail.Inventory;
                }
                else
                {
                    detail.ReturnAmount = returnAmountTemp;
                    break;
                }
            }

            CalculateReturnAmount();
        }

        public void CalculateReturnAmount()
        {
            returnAmount = InventoryDetailCollection.Sum(d => d.ReturnAmount);
            returnStockValue = InventoryDetailCollection.Sum(d => d.ReturnStockValue);

            RaisePropertyChanged(nameof(ReturnAmount));
            RaisePropertyChanged(nameof(ReturnStockValue));
        }

        internal void AddInventoryDetail(DataRow row)
        {
            InventoryDetailCollection.Add(new ReturnProductInventoryDetail(row));
        }

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
            ReturnAmount = returnProduct.ReturnAmount;
            RealAmount = returnProduct.RealAmount;
            Price = returnProduct.Price;
            SubTotal = returnProduct.SubTotal;
            TypeOTC = returnProduct.TypeOTC;

            InventoryDetailCollection = returnProduct.InventoryDetailCollection;
        }

        public abstract object Clone();

        #endregion ----- Define Variables -----
    }
}