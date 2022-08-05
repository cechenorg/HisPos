using System;
using System.Data;
using DomainModel.Enum;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public abstract class PurchaseProduct : Product, ICloneable
    {
        #region ----- Define Variables -----

        private bool isSelected = false;
        private double orderAmount;
        private double realAmount;
        private double subTotal;
        private double price;
        private bool isProcessing = false;
        private string onTheWayDetail = "";
        private ProductStartInputVariableEnum startInputVariable = ProductStartInputVariableEnum.INIT;
        private double originPrice;
        public bool isAssigned;

        public bool IsAssigned
        {
            get { return isAssigned; }
            set
            {
                Set(() => IsAssigned, ref isAssigned, value);
            }
        }

        public double OriginPrice
        {
            get { return originPrice; }
            set
            {
                Set(() => OriginPrice, ref originPrice, value);
            }
        }

        public bool IsSingde { get; set; } = false;

        public bool IsProcessing
        {
            get { return isProcessing; }
            set
            {
                Set(() => IsProcessing, ref isProcessing, value);
                CalculateRealPrice();
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(() => IsSelected, ref isSelected, value); }
        }

        public ProductStartInputVariableEnum StartInputVariable
        {
            get { return startInputVariable; }
            set { Set(() => StartInputVariable, ref startInputVariable, value); }
        }

        public double Inventory { get; private set; }
        public int Type { get; set; }
        public int WareHouseID { get; set; }
        public string UnitName { get; set; }
        public double UnitAmount { get; set; }
        public int SafeAmount { get; private set; }
        public int BasicAmount { get; private set; }
        public double OnTheWayAmount { get; private set; }
        public double MedBagOnTheWayAmount { get; private set; }
        public double LastPrice { get; private set; }
        public int IsDone { get; set; }

        public string OnTheWayDetail
        {
            get { return onTheWayDetail; }
            set { Set(() => OnTheWayDetail, ref onTheWayDetail, value); }
        }

        public double OrderAmount
        {
            get { return orderAmount; }
            set
            {
                Set(() => OrderAmount, ref orderAmount, value);
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
                CalcEqualOrder();
            }
        }

        private void CalcEqualOrder()
        {
            if (OrderAmount - RealAmount == 0)
            {
                IsAssigned = true;
            }
            else
            {
                IsAssigned = false;
            }
            RaisePropertyChanged(nameof(IsAssigned));
        }

        private void CalcSubTotal()
        {
            SubTotal = Price * RealAmount;
            RaisePropertyChanged(nameof(SubTotal));
        }

        public double FreeAmount { get; set; }

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

        public string Invoice { get; set; }
        public DateTime? ValidDate { get; set; }
        public string BatchNumber { get; set; }
        public string Note { get; set; }

        public bool IsFirstBatch { get; set; } = true;
        public int SingdePackageAmount { get; }
        public double SingdePackagePrice { get; }
        public double SingdePrice { get; }
        public int SingdeStock { get; }

        public OrderStatusEnum OrderStatus { get; }

        #endregion ----- Define Variables -----

        public PurchaseProduct() : base()
        {
        }

        public PurchaseProduct(DataRow dataRow, OrderStatusEnum orderStatus) : base(dataRow)
        {
            OrderStatus = orderStatus;

            Type = dataRow.Field<int>("Pro_TypeID");
            WareHouseID = dataRow.Field<int>("ProInv_WareHouseID");
            Inventory = dataRow.Field<double>("Inv_Inventory");
            SafeAmount = dataRow.Field<int>("Inv_SafeAmount");
            BasicAmount = dataRow.Field<int>("Inv_BasicAmount");
            OnTheWayAmount = dataRow.Field<double>("Inv_OnTheWay");
            MedBagOnTheWayAmount = dataRow.Field<double>("Inv_MedBagOnTheWay");
            LastPrice = (double)dataRow.Field<decimal>("Inv_LastPrice");
            UnitName = dataRow.Field<string>("StoOrdDet_UnitName");
            UnitAmount = dataRow.Field<double>("StoOrdDet_UnitAmount");
            OrderAmount = dataRow.Field<double>("StoOrdDet_OrderAmount");
            RealAmount = dataRow.Field<double>("StoOrdDet_RealAmount");
            FreeAmount = dataRow.Field<double>("StoOrdDet_FreeAmount");
            SubTotal = (double)dataRow.Field<decimal>("StoOrdDet_SubTotal");
            Price = (double)dataRow.Field<decimal>("StoOrdDet_Price");
            Invoice = dataRow.Field<string>("StoOrdDet_Invoice");
            ValidDate = dataRow.Field<DateTime?>("StoOrdDet_ValidDate");
            BatchNumber = dataRow.Field<string>("StoOrdDet_BatchNumber");
            Note = dataRow.Field<string>("StoOrdDet_Note");
            OriginPrice = (double)dataRow.Field<decimal>("StoOrdDet_OriginPrice");
            SingdePackageAmount = dataRow.Field<int>("SinData_PackageAmount");
            SingdePackagePrice = (double)dataRow.Field<decimal>("SinData_PackagePrice");
            SingdePrice = (double)dataRow.Field<decimal>("SinData_SinglePrice");
            SingdeStock = dataRow.Field<int>("SinData_Stock");
            if(dataRow.Table.Columns.Contains("IsDone"))
            {
                IsDone = dataRow.Field<int>("IsDone");
            }
        }

        #region ----- Define Functions -----

        private void CalculatePrice()
        {
            switch (StartInputVariable)
            {
                case ProductStartInputVariableEnum.INIT:
                    price = 0;
                    break;

                case ProductStartInputVariableEnum.PRICE:
                    subTotal = 0;

                    //key單時 => 小計 = 單價*預定數量
                    if (OrderStatus == OrderStatusEnum.NORMAL_UNPROCESSING || OrderStatus == OrderStatusEnum.SINGDE_UNPROCESSING)
                    {
                        decimal amt = Math.Round(Convert.ToDecimal(Price) * Convert.ToDecimal(OrderAmount), 0, MidpointRounding.AwayFromZero);
                        subTotal = Convert.ToDouble(amt);
                    } 
                    else//收貨時 => 小計 = 單價*實際進貨數量
                    {
                        decimal amt = Math.Round(Convert.ToDecimal(Price) * Convert.ToDecimal(RealAmount), 0, MidpointRounding.AwayFromZero);
                        subTotal = Convert.ToDouble(amt);
                    }
                   
                    break;
                case ProductStartInputVariableEnum.SUBTOTAL:
                    price = RealAmount == 0 ? 0 : Math.Round(subTotal / RealAmount, 8);
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

        private void SetStartInputVariable(ProductStartInputVariableEnum startInputVariable)
        {
            StartInputVariable = startInputVariable;
        }

        public void CopyOldProductData(PurchaseProduct purchaseProduct)
        {
            OrderAmount = 0;
            RealAmount = purchaseProduct.RealAmount;
            FreeAmount = purchaseProduct.FreeAmount;
            SubTotal = purchaseProduct.SubTotal;
            Price = purchaseProduct.Price;
            Invoice = purchaseProduct.Invoice;
            ValidDate = purchaseProduct.ValidDate;
            BatchNumber = purchaseProduct.BatchNumber;
            Note = purchaseProduct.Note;

            IsSingde = purchaseProduct.IsSingde;
            StartInputVariable = purchaseProduct.StartInputVariable;
        }

        public void GetOnTheWayDetail()
        {
            if (OnTheWayAmount == 0.0) return;

            DataTable dataTable = PurchaseReturnProductDB.GetProductOnTheWayDetailByID(ID, WareHouseID);

            if (dataTable?.Rows.Count == 0) return;

            string tempDetail = "";

            for (int x = 0; x < dataTable.Rows.Count; x++)
            {
                tempDetail += dataTable.Rows[x].Field<string>("STO_ID").PadLeft(12) + "  數量: " + dataTable.Rows[x].Field<double>("AMOUNT").ToString("####");

                if (x < dataTable.Rows.Count - 1)
                    tempDetail += "\n";
            }

            OnTheWayDetail = tempDetail;
        }

        public abstract object Clone();

        //protected void CloneBaseData(PurchaseProduct purchaseProduct)
        //{
        //    ID = purchaseProduct.ID;
        //    ChineseName = purchaseProduct.ChineseName;
        //    EnglishName = purchaseProduct.EnglishName;
        //    Inventory = purchaseProduct.Inventory;
        //    UnitName = purchaseProduct.UnitName;
        //    UnitAmount = purchaseProduct.UnitAmount;
        //    SafeAmount = purchaseProduct.SafeAmount;
        //    BasicAmount = purchaseProduct.BasicAmount;
        //    OnTheWayAmount = purchaseProduct.OnTheWayAmount;
        //}

        #endregion ----- Define Functions -----
    }
}