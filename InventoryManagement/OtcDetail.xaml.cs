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
        public string[] Days { get; set; }
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
            InventoryCollection.Add(GetInventoryLineSeries());
            InventoryCollection.Add(GetSaveAmountLineSeries());
            
            DataFormatter = value => value.ToString();

            AddDays();
        }

        private LineSeries GetInventoryLineSeries()
        {
            return new LineSeries
            {
                Title = "庫存",
                Values = new ChartValues<double> { 4, 5, 5, 5, 4 },
                PointGeometrySize = 10,
                LineSmoothness = 0,
                DataLabels = true
            };
        }

        private LineSeries GetSaveAmountLineSeries()
        {
            ChartValues<double> chartValues = new ChartValues<double>();

            for (int x = 0; x < 30; x++)
            {
                chartValues.Add( double.Parse(otc.SafeAmount) );
            }
            
            return new LineSeries
            {
                Title = "安全量",
                Values = chartValues,
                PointGeometry = null
            };
        }

        private void AddDays()
        {
            DateTime today = DateTime.Today.Date;

            Days = new string[30];
            for (int x = 0; x < 30; x++)
            {
                Days[x] = today.AddDays(x - 29).Date.ToString("dd/MM");
            }
        }

        private void UpdateUi()
        {
            if (otc is null) return;

            OtcName.Content = otc.Name;
            OtcId.Content = otc.Id;

            OtcPrice.Text = otc.Price.ToString();
            OtcInventory.Text = otc.Inventory.ToString();
            OtcCost.Text = otc.Cost.ToString();
            OtcSaveAmount.Text = otc.SafeAmount.ToString();
            OtcManufactory.Text = otc.ManufactoryName;

            CusOrderOverviewCollection = OTCDb.GetOtcCusOrderOverviewByID(otc.Id);
            OtcCusOrder.ItemsSource = CusOrderOverviewCollection;

            UpdateChart();
        }
    }
}
