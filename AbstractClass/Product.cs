using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Media.Imaging;

namespace His_Pos.AbstractClass
{
    public class Product: INotifyPropertyChanged
    {
        public BitmapImage TypeIcon { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public double LastPrice { get; set; }
        public double Cost { get; set; }
        public double Inventory { get; set; }
        public string SafeAmount { get; set; } 
        public string ManufactoryName { get; set; }
        public string StockValue { get; set; }
        public string Note { get; set; }
        public string BasicAmount { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public string Location { get; set; }

        private double totalPrice;
        public double amount;
        public double price;

        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                NotifyPropertyChanged("Price");
            }
        }
        public double Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                NotifyPropertyChanged("Amount");
            }
        }
        public double TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                NotifyPropertyChanged("TotalPrice");
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
    }
}
