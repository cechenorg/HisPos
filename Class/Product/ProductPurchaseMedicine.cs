using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using His_Pos.AbstractClass;
using His_Pos.Interface;
using His_Pos.Struct.Product;

namespace His_Pos.Class.Product
{
    class ProductPurchaseMedicine : AbstractClass.Product, IProductPurchase, IDeletable, ITrade, INotifyPropertyChanged, ICloneable
    {
        public ProductPurchaseMedicine(DataRow dataRow, DataSource dataSource) : base(dataRow)
        {
            LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());

            Stock = new InStock(dataRow);

            switch (dataSource)
            {
                case DataSource.GetStoreOrderDetail:
                    Price = dataRow["STOORDDET_PRICE"].ToString();
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    Amount = Int32.Parse(dataRow["STOORDDET_QTY"].ToString());
                    OrderAmount = Int32.Parse(dataRow["STOORDDET_ORDERQTY"].ToString());
                    Note = dataRow["PRO_DESCRIPTION"].ToString();
                    Invoice = dataRow["STOORDDET_INVOICE"].ToString();
                    FreeAmount = Int32.Parse(dataRow["STOORDDET_FREEQTY"].ToString());
                    ValidDate = (dataRow["STOORDDET_VALIDDATE"].ToString().Equals("1900/01/01"))? "" : dataRow["STOORDDET_VALIDDATE"].ToString();
                    BatchNumber = dataRow["STOORDDET_BATCHNUMBER"].ToString();
                    break;
            }

            IsFirstBatch = true;
        }

        private ProductPurchaseMedicine()
        {
        }

        public ProductPurchaseMedicine(PurchaseProduct selectedItem) : base (selectedItem)
        {
            Amount = 0;
            Price = "0";
            TotalPrice = 0;
            Note = "";
            Invoice = "";
            FreeAmount = 0;
            ValidDate = "";
            BatchNumber = "";
            Status = selectedItem.Status;
            LastPrice = selectedItem.LastPrice;
            Stock = new InStock(selectedItem);
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
                NotifyPropertyChanged("Amount");
            }
        }
        private string price;
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                CalculateData("Price");
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

        public void CalculateData(string inputSource)
        {
            double dprice;
            if (!double.TryParse(price, out dprice)) return;
            if (totalPrice == amount * dprice || dprice == totalPrice / amount) return;

            bool isColumnChanged;

            if (FocusColumn.Equals(""))
                isColumnChanged = false;
            else
                isColumnChanged = !FocusColumn.Equals(inputSource);

            if (isColumnChanged) CountStatus = "";

            if (inputSource.Equals("Amount") && amount == 0)
                return;
            else if (inputSource.Equals("Amount") && totalPrice != 0 && amount != 0 && !CountStatus.Equals("*"))
            {
                Price = (totalPrice / amount).ToString();
            }
            else if (!inputSource.Equals("TotalPrice"))
            {
                CountStatus = "*";
                TotalPrice = amount * dprice;
            }
            else if (amount != 0)
            {
                Price = (totalPrice / amount).ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
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
            med.TotalPrice = TotalPrice;
            med.Amount = Amount;
            med.Price = Price;
            med.Note = Note;
            med.FreeAmount = FreeAmount;
            med.Invoice = Invoice;
            med.ValidDate = ValidDate;
            med.BatchNumber = BatchNumber;
            med.Status = Status;
            med.OrderAmount = OrderAmount;

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
            Status = ((IProductPurchase)product).Status;
            LastPrice = ((IProductPurchase)product).LastPrice;
            Stock = ((IProductPurchase)product).Stock;

        }
    }
}
