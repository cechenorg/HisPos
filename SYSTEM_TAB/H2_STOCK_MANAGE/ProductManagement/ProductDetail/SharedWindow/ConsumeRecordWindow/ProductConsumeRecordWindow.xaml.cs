using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
using His_Pos.NewClass.Product;
using His_Pos.NewClass.WareHouse;
using LiveCharts;
using LiveCharts.Wpf;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ConsumeRecordWindow
{
    /// <summary>
    /// ProductConsumeRecordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProductConsumeRecordWindow : Window
    {
        #region ----- Define Variables -----
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public Collection<string> Days { get; set; } = new Collection<string>();
        #endregion

        public ProductConsumeRecordWindow(string productID, WareHouse selectedWareHouse)
        {
            InitializeComponent();
            DataContext = this;
            Title = $"{productID} 耗用折線圖({selectedWareHouse.Name})";
            
            InitChart(productID, selectedWareHouse.ID);
        }

        #region ----- Define Functions -----
        private void InitChart(string productID, string wareID)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductDB.GetProductConsumeRecordByID(productID, wareID, DateTime.Today.AddDays(-90), DateTime.Today.AddDays(90));
            MainWindow.ServerConnection.CloseConnection();

            SeriesCollection.Add(new LineSeries { Title = "耗用", Values = new ChartValues<double>(), LineSmoothness = 0 });
            SeriesCollection.Add(new LineSeries { Title = "預估耗用", Values = new ChartValues<double>(), LineSmoothness = 0, StrokeDashArray = new DoubleCollection {2} });
            SeriesCollection.Add(new LineSeries { Title = "平均耗用", Values = new ChartValues<double>(), PointGeometry = null });

            double totalAmount = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                string day = row.Field<string>("DAYS");

                Days.Add(day);

                if (int.Parse(day.Substring(5, 2)) < DateTime.Today.Month)
                {
                    SeriesCollection[0].Values.Add(row.Field<double>("AMOUNT"));
                    SeriesCollection[1].Values.Add(0.0);
                }
                else if (int.Parse(day.Substring(5, 2)) == DateTime.Today.Month)
                {
                    SeriesCollection[0].Values.Add(row.Field<double>("AMOUNT"));
                    SeriesCollection[1].Values.Add(row.Field<double>("AMOUNT"));
                }
                else
                {
                    SeriesCollection[1].Values.Add(row.Field<double>("AMOUNT"));
                }

                totalAmount += row.Field<double>("AMOUNT");
            }

            for (int x = 0; x < dataTable.Rows.Count; x++)
                SeriesCollection[2].Values.Add(totalAmount / dataTable.Rows.Count); 
        }
        #endregion

        
    }
}
