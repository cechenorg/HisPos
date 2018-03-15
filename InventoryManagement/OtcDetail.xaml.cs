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
        public SeriesCollection InventoryCollection { get; set; }
        public string[] Months { get; set; }
        public Func<double, string> DataFormatter { get; set; }
        public ObservableCollection<CusOrderOverview> CusOrderOverviewCollection;

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
            InventoryCollection = new SeriesCollection();
            InventoryCollection.Add(GetSalesLineSeries());
            
            DataFormatter = value => value.ToString();

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

            OtcPrice.Text = otc.Price.ToString();
            OtcInventory.Text = otc.Inventory.ToString();
            OtcSaveAmount.Text = otc.SafeAmount;
            OtcManufactory.Text = otc.ManufactoryName;

            CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(otc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            UpdateChart();
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if ( selectedItem is CusOrderOverview )
                OtcCusOrder.SelectedItem = selectedItem;
            else
                OtcManOrder.SelectedItem = selectedItem;
            
        }
    }
}
