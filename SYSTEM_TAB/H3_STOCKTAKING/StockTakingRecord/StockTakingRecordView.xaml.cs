using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using His_Pos.Class.StockTakingOrder;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingRecord
{
    /// <summary>
    /// StockTakingRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingRecordView : UserControl, INotifyPropertyChanged
    {
        public string selectEmpName;
             private DateTime startDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }
        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }
     
        public StockTakingOrder stockTakingOrder;
        public StockTakingOrder StockTakingOrder
        {
            get
            {
                return stockTakingOrder;
            }
            set
            {
                stockTakingOrder = value;
                NotifyPropertyChanged("StockTakingOrder");
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
            StockTakingRecord.Items.Filter = Filter;
            if (StocktakingCollection.Count > 0) StockTakingRecord.SelectedIndex = 0;
        }

        private void StockTakingRecord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StockTakingOrder =  (StockTakingOrder)(sender as DataGrid).SelectedItem;
            if (StockTakingOrder is null) return;
            TotalAmount.Content = StockTakingOrder.ChangedtakingCollection.Count + StockTakingOrder.UnchangedtakingCollection.Count;
            ChangedAmount.Content = StockTakingOrder.ChangedtakingCollection.Count;
            StockTakingOrder.EmpName.Clear();
            foreach (var product in StockTakingOrder.ChangedtakingCollection)
            {
                if (!StockTakingOrder.EmpName.Contains(product.EmpName)) StockTakingOrder.EmpName.Add(product.EmpName);
            }
            foreach (var product in StockTakingOrder.UnchangedtakingCollection)
            {
                if (!StockTakingOrder.EmpName.Contains(product.EmpName)) StockTakingOrder.EmpName.Add(product.EmpName);
            }
            UnChanged.Items.Filter = null;
            Changed.Items.Filter = null;
            StockTakingOrder.EmpName.Add("全部");
        }
        private bool Filter(object item)
        {
            int id = Convert.ToInt32(((StockTakingOrder)item).Id.Substring(1,8)); 
            if ((((StockTakingOrder)item).Id.Contains(PROCHE_ID.Text) || PROCHE_ID.Text == string.Empty)
              &&((((((StockTakingOrder)item).ChangedtakingCollection).Count(x => x.Id.Contains(PRO_ID.Text)) > 0) || ((((StockTakingOrder)item).UnchangedtakingCollection).Count(x => x.Id.Contains(PRO_ID.Text)) > 0) && PRO_ID.Text != string.Empty) || PRO_ID.Text ==string.Empty)
              &&((((((StockTakingOrder)item).ChangedtakingCollection).Count(x => x.Name.Contains(PRO_NAME.Text)) > 0) || ((((StockTakingOrder)item).UnchangedtakingCollection).Count(x => x.Name.Contains(PRO_NAME.Text)) > 0) && PRO_NAME.Text != string.Empty) || PRO_NAME.Text == string.Empty)
              && ( id >= Convert.ToInt32(StartDate.ToString("yyyyMMdd")) && id <= Convert.ToInt32(EndDate.ToString("yyyyMMdd")))
               ) return true;
            return false;
        }
      
        private bool ChangedEmpFilter(object item)
        {
            if (((StockTakingOrderProduct)item).EmpName == selectEmpName || selectEmpName == "全部") return true;
            return false;
        }
        private void Header_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmpHeader.SelectedItem == null) return;
            selectEmpName = (EmpHeader.SelectedItem).ToString();
            UnChanged.Items.Filter = ChangedEmpFilter;
            Changed.Items.Filter = ChangedEmpFilter;
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
