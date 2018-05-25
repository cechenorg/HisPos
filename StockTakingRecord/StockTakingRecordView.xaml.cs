using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace His_Pos.StockTakingRecord
{
    /// <summary>
    /// StockTakingRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingRecordView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<StockTakingOrder> stocktakingCollection = new ObservableCollection<StockTakingOrder>();
        public ObservableCollection<StockTakingOrder> StocktakingCollection
        {
            get
            {
                return stocktakingCollection;
            }
            set
            {
                stocktakingCollection = value;
                NotifyPropertyChanged("StocktakingCollection");
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
        public StockTakingRecordView()
        {
            InitializeComponent();
            DataContext = this;
            InitStockTakingOrder();
        }

        private void InitStockTakingOrder()
        {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetStockTakingRecord(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            string a = "";
        }
    }
}
