using System;
using System.ComponentModel;
using System.Data;
using His_Pos.Interface;
using His_Pos.Struct.Product;

namespace His_Pos.Class.Product
{
    class ProductPurchaseMedicine : AbstractClass.Product, IProductPurchase, IDeletable, ITrade, ICloneable
    {
        public ProductPurchaseMedicine(DataRow dataRow, DataSource dataSource) : base(dataRow)
        {
            LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());

            Stock = new InStock(dataRow);

            switch (dataSource)
            {
                case DataSource.GetStoreOrderDetail:
                    price = Double.Parse(dataRow["STOORDDET_PRICE"].ToString());
                    totalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    amount = Int32.Parse(dataRow["STOORDDET_QTY"].ToString());
                    orderAmount = Int32.Parse(dataRow["STOORDDET_ORDERQTY"].ToString());
                    note = dataRow["PRO_DESCRIPTION"].ToString();
                    invoice = dataRow["STOORDDET_INVOICE"].ToString();
                    freeAmount = Int32.Parse(dataRow["STOORDDET_FREEQTY"].ToString());
                    validDate = (dataRow["STOORDDET_VALIDDATE"].ToString().Equals("1900/01/01"))? "" : dataRow["STOORDDET_VALIDDATE"].ToString();
                    batchNumber = dataRow["STOORDDET_BATCHNUMBER"].ToString();

                    PackageAmount = Double.Parse(dataRow["PROSIN_PACKAGEQTY"].ToString());
                    PackagePrice = Double.Parse(dataRow["PROSIN_PACKAGEPRICE"].ToString());
                    SingdePrice = Double.Parse(dataRow["PROSIN_PRICE"].ToString());

                    IsSingde = Boolean.Parse(dataRow["IS_SINGDE"].ToString());
                    IsCommon = Boolean.Parse(dataRow["HISMED_COMMON"].ToString());
                    IsControl = Boolean.Parse(dataRow["HISMED_CONTROL"].ToString());
                    break;
            }

            IsFirstBatch = true;
        }

        private ProductPurchaseMedicine()
        {
        }

        public ProductPurchaseMedicine(PurchaseProduct selectedItem, bool isSingde) : base (selectedItem)
        {
            Amount = 0;
            Price = 0;
            TotalPrice = 0;
            Note = "";
            Invoice = "";
            FreeAmount = 0;
            ValidDate = "";
            BatchNumber = "";
            Status = selectedItem.Status;
            LastPrice = selectedItem.LastPrice;
            Stock = new InStock(selectedItem);
            IsFirstBatch = true;

            IsCommon = selectedItem.IsCommon;
            IsControl = selectedItem.IsControl;

            PackageAmount = selectedItem.PackageAmount;
            PackagePrice = selectedItem.PackagePrice;
            SingdePrice = selectedItem.SingdePrice;

            IsSingde = isSingde;
        }

        public bool IsFirstBatch { get; set; }
        public bool InvertIsFirstBatch { get { return !IsFirstBatch; } }
        public bool Status { get; set; } = false;
        public string CountStatus { get; set; } = "";
        public string FocusColumn { get; set; } = "";
        public InStock Stock { get; set; }
        public double orderAmount;
        public double OrderAmount
        {
            get { return orderAmount; }
            set
            {
                orderAmount = value;
                CalculatePackagePrice();
                CalculateData("OrderAmount");
                NotifyPropertyChanged("OrderAmount");
            }
        }
        private string note;
        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                NotifyPropertyChanged("Note");
            }
        }
        public double LastPrice { get; set; }
        private string source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }
        public double Cost { get; set; }
        private double totalPrice;
        public double TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                CalculateData("TotalPrice");
                FocusColumn = "TotalPrice";
                NotifyPropertyChanged("TotalPrice");
            }
        }
        private double amount;
        public double Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                CalculateData("Amount");
                FocusColumn = "Amount";
                NotifyPropertyChanged("Amount");
                NotifyPropertyChanged("IsEnough");
            }
        }
        private double price;
        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                CalculateData("Price");
                FocusColumn = "Price";
                NotifyPropertyChanged("Price");
            }
        }

        private double freeAmount;

        public double FreeAmount
        {
            get { return freeAmount; }
            set
            {
                freeAmount = value;
                NotifyPropertyChanged("FreeAmount");
            }
        }

        private string invoice;

        public string Invoice
        {
            get { return invoice; }
            set
            {
                invoice = value;
                NotifyPropertyChanged("Invoice");
            }
        }

        private string validDate;

        public string ValidDate
        {
            get { return validDate; }
            set
            {
                validDate = value;
                NotifyPropertyChanged("ValidDate");
            }
        }

        private string batchNumber;

        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                batchNumber = value;
                NotifyPropertyChanged("BatchNumber");
            }
        }

        public double PackageAmount { get; }

        public double PackagePrice { get; }

        public double SingdePrice { get; }

        public bool IsSingde { get; set; }
        public bool IsCommon { get; set; }
        public bool IsControl { get; set; }
        public bool IsEnough
        {
            get { return Amount > OrderAmount; }
        }

        public void CalculateData(string inputSource)
        {
            if(IsSingde) return;

            double tempAmount = 0;

            if (inputSource.Equals("OrderAmount"))
            {
                tempAmount = OrderAmount;
            }
            else
            {
                tempAmount = Amount;
            }

            double dprice = price;
            if (totalPrice == tempAmount * dprice || dprice == totalPrice / tempAmount) return;

            bool isColumnChanged;

            if (FocusColumn.Equals(""))
                isColumnChanged = false;
            else
                isColumnChanged = !FocusColumn.Equals(inputSource);

            if (isColumnChanged) CountStatus = "";

            if (inputSource.Equals("Amount") && tempAmount == 0)
                return;
            else if (inputSource.Equals("Amount") && totalPrice != 0 && tempAmount != 0 && !CountStatus.Equals("*"))
            {
                Price = totalPrice / tempAmount;
            }
            else if (!inputSource.Equals("TotalPrice"))
            {
                CountStatus = "*";
                TotalPrice = tempAmount * dprice;
            }
            else if (tempAmount != 0)
            {
                Price = totalPrice / tempAmount;
            }
        }

        private void CalculatePackagePrice()
        {
            if (IsSingde)
            {
                if (OrderAmount >= PackageAmount)
                {
                    TotalPrice = PackagePrice * OrderAmount;
                    Price = PackagePrice;
                }
                else
                {
                    TotalPrice = SingdePrice * OrderAmount;
                    Price = SingdePrice;
                }
            }
        }

        public object Clone()
        {
            ProductPurchaseMedicine med = new ProductPurchaseMedicine();

            med.Id = Id;
            med.Name = Name;
            med.ChiName = ChiName;
            med.EngName = EngName;
            med.Stock = Stock;
            med.LastPrice = LastPrice;
            med.Source = Source;
            med.Cost = Cost;
            med.totalPrice = TotalPrice;
            med.amount = Amount;
            med.price = Price;
            med.note = Note;
            med.freeAmount = FreeAmount;
            med.invoice = Invoice;
            med.validDate = ValidDate;
            med.batchNumber = BatchNumber;
            med.Status = Status;
            med.orderAmount = OrderAmount;

            return med;
        }

        public void CopyFilledData(AbstractClass.Product product)
        {
            Amount = ((ITrade)product).Amount;
            Price = ((ITrade)product).Price;
            TotalPrice = ((ITrade)product).TotalPrice;
            Note = ((IProductPurchase)product).Note;
            Invoice = ((IProductPurchase)product).Invoice;
            FreeAmount = ((IProductPurchase)product).FreeAmount;
            ValidDate = ((IProductPurchase)product).ValidDate;
            BatchNumber = ((IProductPurchase)product).BatchNumber;
            IsFirstBatch = true;
        }
    }
}
