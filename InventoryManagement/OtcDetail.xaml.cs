using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using His_Pos.Class.Product;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;

namespace His_Pos.InventoryManagement
{
    /// <summary>
    /// OtcDetail.xaml 的互動邏輯
    /// </summary>
    public partial class OtcDetail : Window
    {
        public SeriesCollection SalesCollection { get; set; }
        public string[] Months { get; set; }

        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;
        public ObservableCollection<OTCStoreOrderOverview> StoreOrderOverviewCollection;
        public ObservableCollection<OTCStockOverview> OTCStockOverviewCollection;
        public ObservableCollection<OTCUnit> OTCUnitCollection;

        private Otc otc;

        public OtcDetail(Otc o)
        {
            InitializeComponent();

            otc = o;
            
            UpdateUi();
            CheckAuth();

            DataContext = this;
        }

        private void CheckAuth()
        {
            
        }

        private void UpdateChart()
        {
            SalesCollection = new SeriesCollection();
            SalesCollection.Add(GetSalesLineSeries());
            AddMonths();
        }

        private LineSeries GetSalesLineSeries()
        {
            ChartValues<double> chartValues = OTCDb.GetOtcSalesByID(otc.Id);

            return new LineSeries
            {
                Title = "銷售量",
                Values = chartValues,
                PointGeometrySize = 10,
                LineSmoothness = 0,
                DataLabels = true
            };
        }

        private void AddMonths()
        {
            DateTime today = DateTime.Today.Date;

            Months = new string[12];
            for (int x = 0; x < 12; x++)
            {
                Months[x] = today.AddMonths(-11 + x).Date.ToString("yyyy/MM"); 
            }
        }

        private void UpdateUi()
        {
            if (otc is null) return;

            OtcName.Content = otc.Name;
            OtcId.Content = otc.Id;
            
            OtcSaveAmount.Text = otc.SafeAmount;
            OtcManufactory.Text = otc.ManufactoryName;

            CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(otc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            StoreOrderOverviewCollection = OTCDb.GetOtcStoOrderByID(otc.Id);
            OtcStoOrder.ItemsSource = StoreOrderOverviewCollection;

            OTCStockOverviewCollection = OTCDb.GetOtcStockOverviewById(otc.Id);
            OtcStock.ItemsSource = OTCStockOverviewCollection;
            UpdateStockOverviewInfo();

            OTCUnitCollection = OTCDb.GetOtcUnitById(otc.Id);
            OtcUnit.ItemsSource = OTCUnitCollection;

            UpdateChart();
        }

        private void UpdateStockOverviewInfo()
        {
            int totalStock = 0;
            double totalPrice = 0;

            foreach (var Otc in OTCStockOverviewCollection)
            {
                totalStock += Int32.Parse(Otc.Amount);
                totalPrice += Double.Parse(Otc.Price) * Int32.Parse(Otc.Amount);
            }

            TotalStock.Content = totalStock.ToString();
            StockTotalPrice.Content = "$" + totalPrice.ToString("0.00");

        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if ( selectedItem is CusOrderOverview )
                OtcCusOrder.SelectedItem = selectedItem;
            else if( selectedItem is OTCStoreOrderOverview)
                OtcStoOrder.SelectedItem = selectedItem;
            else if (selectedItem is OTCStockOverview)
                OtcStock.SelectedItem = selectedItem;
            else if (selectedItem is OTCUnit)
                OtcUnit.SelectedItem = selectedItem;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is CusOrderOverview)
                OtcCusOrder.SelectedItem = null;
            else if (leaveItem is OTCStoreOrderOverview)
                OtcStoOrder.SelectedItem = null;
            else if (leaveItem is OTCStockOverview)
                OtcStock.SelectedItem = null;
            else if (leaveItem is OTCUnit)
                OtcUnit.SelectedItem = null;
        }
    }
}
