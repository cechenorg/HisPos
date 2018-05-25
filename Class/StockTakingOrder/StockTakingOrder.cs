using His_Pos.Class.StockTakingOrder;
using His_Pos.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class StockTakingOrder : INotifyPropertyChanged
    {
        public StockTakingOrder(DataRow row) {
            Id = row["PROCHE_ID"].ToString();
        }
        public StockTakingOrder(string id)
        {
            Id = id;
        }
        public int amount;
        public int Amount {
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
