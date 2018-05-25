using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using His_Pos.Class.StockTakingOrder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        public ObservableCollection<StockTakingOrderProduct> changedtakingCollection;
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
        public ObservableCollection<StockTakingOrderProduct> unchangedtakingCollection;
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
        }

        private void StockTakingRecord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangedtakingCollection =  ((StockTakingOrder)StockTakingRecord.SelectedItem).ChangedtakingCollection;
            UnchangedtakingCollection = ((StockTakingOrder)StockTakingRecord.SelectedItem).UnchangedtakingCollection;
        }

        
    }
    public class ValueDifferentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if( int.Parse(value.ToString()) > 0 )
            return Brushes.Green;
            else
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
