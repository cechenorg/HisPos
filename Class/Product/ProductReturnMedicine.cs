using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;
using His_Pos.Struct.Product;

namespace His_Pos.Class.Product
{
    public class ProductReturnMedicine: AbstractClass.Product, IProductReturn, ITrade, IDeletable, INotifyPropertyChanged, ICloneable
    {
        private ProductReturnMedicine() { }
        public ProductReturnMedicine(DataRow row) : base(row)
        {
            Stock = new InStock(row);

            Note = row["PRO_NOTE"].ToString();
            Amount = Double.Parse(row["AMOUNT"].ToString());
            BatchNumber = row["BATCHNUMBER"].ToString();
            BatchLimit = Double.Parse(row["BATCHLIMIT"].ToString());
            Price = Double.Parse(row["PRICE"].ToString());
            TotalPrice = Double.Parse(row["TOTAL"].ToString());
        }

        public ProductReturnMedicine(PurchaseProduct selectedItem)
        {
            Id = selectedItem.Id;
            Name = selectedItem.Name;
            ChiName = selectedItem.ChiName;
            EngName = selectedItem.EngName;

            Stock = new InStock(selectedItem);

            Note = "";
            Amount = 0;
            BatchNumber = "";
            BatchLimit = 0;
            Price = 0;
            TotalPrice = 0;
        }

        public InStock Stock { get; set; }
        public double BatchLimit { get; set; }
        public double Cost { get; set; }
        public string CountStatus { get; set; }
        public string FocusColumn { get; set; }

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
        private string source;
        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                NotifyPropertyChanged("Source");
            }
        }

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
        public double price;
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
            ProductReturnMedicine med = new ProductReturnMedicine();

            med.Id = Id;
            med.Name = Name;
            med.ChiName = ChiName;
            med.EngName = EngName;
            med.Stock = Stock;
            med.Source = Source;
            med.Cost = Cost;
            med.totalPrice = TotalPrice;
            med.amount = Amount;
            med.price = Price;
            med.note = Note;
            med.batchNumber = BatchNumber;
            med.BatchLimit = BatchLimit;

            return med;
        }

        public void CalculateData(string inputSource)
        {
            double dprice = price;
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
                Price = totalPrice / amount;
            }
            else if (!inputSource.Equals("TotalPrice"))
            {
                CountStatus = "*";
                TotalPrice = amount * dprice;
            }
            else if (amount != 0)
            {
                Price = totalPrice / amount;
            }
        }

        public void CopyFilledData(AbstractClass.Product product)
        {
            Amount = ((ITrade)product).Amount;
            Price = ((ITrade)product).Price;
            TotalPrice = ((ITrade)product).TotalPrice;
            Note = ((IProductReturn)product).Note;
        }
    }
}
