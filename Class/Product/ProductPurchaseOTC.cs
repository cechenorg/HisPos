using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using His_Pos.Interface;

namespace His_Pos.Class.Product
{
    public class ProductPurchaseOtc : AbstractClass.Product, IProductPurchase, IDeletable, ITrade, INotifyPropertyChanged, ICloneable
    {
        public ProductPurchaseOtc(DataRow dataRow, DataSource dataSource) : base(dataRow)
        {
            BatchNumber = "";

            LastPrice = Double.Parse(dataRow["LAST_PRICE"].ToString());

            Stock = new InStock(dataRow);

            switch (dataSource)
            {
                case DataSource.PRODUCTBASICORSAFE:
                    Amount = Int16.Parse(dataRow["PRO_BASICQTY"].ToString()) -
                             Int16.Parse(dataRow["PRO_INVENTORY"].ToString());
                    Price = "0";
                    TotalPrice = 0;
                    Note = "";
                    Invoice = "";
                    FreeAmount = 0;
                    ValidDate = "";
                    BatchNumber = "";
                    break;
                case DataSource.GetStoreOrderDetail:
                    Price = dataRow["STOORDDET_PRICE"].ToString();
                    TotalPrice = Double.Parse(dataRow["STOORDDET_SUBTOTAL"].ToString());
                    Amount = Int32.Parse(dataRow["STOORDDET_QTY"].ToString());
                    Note = dataRow["PRO_DESCRIPTION"].ToString();
                    Invoice = dataRow["STOORDDET_INVOICE"].ToString();
                    FreeAmount = Int32.Parse(dataRow["STOORDDET_FREEQTY"].ToString());
                    ValidDate = (dataRow["STOORDDET_VALIDDATE"].ToString().Equals("1900/01/01")) ? "" : dataRow["STOORDDET_VALIDDATE"].ToString();
                    BatchNumber = dataRow["STOORDDET_BATCHNUMBER"].ToString();
                    break;
                case DataSource.GetItemDialogProduct:
                    Amount = 0;
                    Price = "0";
                    TotalPrice = 0;
                    Note = "";
                    Invoice = "";
                    FreeAmount = 0;
                    ValidDate = "";
                    BatchNumber = "";
                    Status = dataRow["PRO_STATUS"].ToString().Equals("1");
                    break;
            }
        }

        private ProductPurchaseOtc()
        {
        }

        public string CountStatus { get; set; } = "";
        public string FocusColumn { get; set; } = "";
        public bool Status { get; set; } = false;
        public InStock Stock { get; set; }
        public double LastPrice { get; set; }
        public string source;
        public string Source {
            get {
                return source;
            }
            set {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }
        public double Cost { get; set; }
        public double totalPrice;
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
        public double amount;
        public double Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                CalculateData("Amount");
                FocusColumn = "Amount";
                NotifyPropertyChanged("Amount");
            }
        }
        public string price;
        public string Price
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
        private int freeAmount;

        public int FreeAmount
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
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

        public object Clone()
        {
            ProductPurchaseOtc otc = new ProductPurchaseOtc();

            otc.Id = Id;
            otc.Name = Name;
            otc.ChiName = ChiName;
            otc.EngName = EngName;
            otc.Stock=Stock;
            otc.LastPrice=LastPrice;
            otc.Source=Source;
            otc.Cost=Cost;
            otc.TotalPrice=TotalPrice;
            otc.Amount=Amount;
            otc.Price=Price;
            otc.Note=Note;
            otc.FreeAmount=FreeAmount;
            otc.Invoice=Invoice;
            otc.ValidDate=ValidDate;
            otc.BatchNumber=BatchNumber;
            otc.Status = Status;

            return otc;
        }
    }
}
