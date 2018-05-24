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
        private ObservableCollection<StockTakingOrderProduct> takingCollection = new ObservableCollection<StockTakingOrderProduct>();
        public ObservableCollection<StockTakingOrderProduct> TakingCollection
        {
            get
            {
                return takingCollection;
            }
            set
            {
                takingCollection = value;
                NotifyPropertyChanged("TakingCollection");
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
