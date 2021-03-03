using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;

namespace His_Pos.Class.StockTakingOrder
{
    public class StockTakingOrder : INotifyPropertyChanged
    {
        public StockTakingOrder(DataRow row)
        {
            Id = row["PROCHE_ID"].ToString();
        }

        public StockTakingOrder(string id)
        {
            Id = id;
        }

        public string id;

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public int amount;

        public int Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
                NotifyPropertyChanged("Amount");
            }
        }

        public ObservableCollection<string> empName = new ObservableCollection<string>();

        public ObservableCollection<string> EmpName
        {
            get
            {
                return empName;
            }
            set
            {
                empName = value;
                NotifyPropertyChanged("EmpName");
            }
        }

        public double TotalPriceDiff
        {
            get
            {
                double totalPriceDiff = 0;
                foreach (var product in changedtakingCollection)
                {
                    totalPriceDiff += Convert.ToDouble(product.PriceDiff);
                }
                return totalPriceDiff;
            }
        }

        public string stockTakingTime;
        private ObservableCollection<StockTakingOrderProduct> changedtakingCollection = new ObservableCollection<StockTakingOrderProduct>();

        public ObservableCollection<StockTakingOrderProduct> ChangedtakingCollection
        {
            get
            {
                return changedtakingCollection;
            }
            set
            {
                changedtakingCollection = value;
                NotifyPropertyChanged("ChangedtakingCollection");
            }
        }

        private ObservableCollection<StockTakingOrderProduct> unchangedtakingCollection = new ObservableCollection<StockTakingOrderProduct>();

        public ObservableCollection<StockTakingOrderProduct> UnchangedtakingCollection
        {
            get
            {
                return unchangedtakingCollection;
            }
            set
            {
                unchangedtakingCollection = value;
                NotifyPropertyChanged("UnchangedtakingCollection");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}