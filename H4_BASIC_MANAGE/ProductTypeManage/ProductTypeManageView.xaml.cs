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
using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using His_Pos.Class.ProductType;
using His_Pos.Interface;
using His_Pos.InventoryManagement;
using LiveCharts;
using LiveCharts.Wpf;

namespace His_Pos.ProductTypeManage
{
    /// <summary>
    /// ProductTypeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTypeManageView : UserControl, INotifyPropertyChanged
    {
        public string[] Months { get; set; }
        public string[] Days { get; set; }

        private ObservableCollection<ProductTypeManageMaster> typeManageMasters = new ObservableCollection<ProductTypeManageMaster>();

        public ObservableCollection<ProductTypeManageMaster> TypeManageMasters
        {
            get { return typeManageMasters; }
            set
            {
                typeManageMasters = value;
                NotifyPropertyChanged("TypeManageMasters");
            }
        }

        private ObservableCollection<ProductTypeManageDetail> typeManageDetails = new ObservableCollection<ProductTypeManageDetail>();

        public ObservableCollection<ProductTypeManageDetail> TypeManageDetails
        {
            get { return typeManageDetails; }
            set
            {
                typeManageDetails = value;
                NotifyPropertyChanged("TypeManageDetails");
            }
        }

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                NotifyPropertyChanged("Products");
            }
        }

        public SeriesCollection StockValuePieSeries { get; set; } = new SeriesCollection();
        public SeriesCollection SalesPieSeries { get; set; } = new SeriesCollection();

        private SeriesCollection salesLineSeries = new SeriesCollection();
        public SeriesCollection SalesLineSeries
        {
            get { return salesLineSeries; }
            set
            {
                salesLineSeries = value;
                NotifyPropertyChanged("SalesLineSeries");
            }
        }

        public LineSeries YearSalesLineSeries { get; set; }
        public LineSeries LastYearSalesLineSeries { get; set; }

        public LineSeries MonthSalesLineSeries { get; set; }
        public LineSeries LastMonthSalesLineSeries { get; set; }

        public ProductTypeManageView()
        {
            InitializeComponent();

            DataContext = this;

            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.InitProductType(this);
            loadingWindow.Show();
            loadingWindow.Topmost = true;
        }

        public void InitMonthsAndDays()
        {
            DateTime today = DateTime.Today.Date;

            Months = new string[12];
            for (int x = 0; x < 12; x++)
            {
                Months[x] = DateTime.Today.Year.ToString() + "/" + (x + 1).ToString();
            }

            int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);

            Days = new string[daysInMonth];
            for (int x = 0; x < daysInMonth; x++)
            {
                Days[x] = DateTime.Today.Month.ToString() + "/" + (x + 1).ToString();
            }
        }

        public void InitPieCharts()
        {
            foreach (var productType in TypeManageMasters)
            {
                if (productType.StockValue != 0)
                {
                    PieSeries newStockPieSeries = new PieSeries()
                    {
                        Title = productType.Name,
                        Values = new ChartValues<double> { productType.StockValue },
                        DataLabels = true
                    };

                    StockValuePieSeries.Add(newStockPieSeries);
                }

                if (productType.Sales != 0)
                {
                    PieSeries newSalesPieSeries = new PieSeries()
                    {
                        Title = productType.Name,
                        Values = new ChartValues<double> { productType.Sales },
                        DataLabels = true
                    };

                    SalesPieSeries.Add(newSalesPieSeries);
                }
            }
        }

        public void InitTypes()
        {
            TypeManageMasters.Clear();
            TypeManageDetails.Clear();

            ProductDb.GetProductTypeManage(TypeManageMasters, TypeManageDetails);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void TypeMaster_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as DataGrid) is null || (sender as DataGrid).SelectedItem is null) return;

            TypeDetail.Items.Filter = item => ((ProductTypeManageDetail)item).Rank == ((ProductTypeManageMaster)(sender as DataGrid).SelectedItem).Id;

            BigType.Content = ((ProductTypeManageMaster)(sender as DataGrid).SelectedItem).Name;

            PieChartPushOut();
            InitLineChart(((ProductTypeManageMaster)(sender as DataGrid).SelectedItem).Id);

            if (TypeDetail.Items.Count != 0)
                TypeDetail.SelectedIndex = 0;
        }

        private void InitTypeTextBox(string bigTypeName)
        {
            
        }

        private void InitLineChart(string typeId)
        {
            SalesLineSeries.Clear();
            YearSalesLineSeries = new LineSeries() { Title = "今年", Stroke = Brushes.RoyalBlue };
            LastYearSalesLineSeries = new LineSeries() { Title = "去年", Stroke = Brushes.Red };
            MonthSalesLineSeries = new LineSeries() { Title = "本月", Stroke = Brushes.RoyalBlue };
            LastMonthSalesLineSeries = new LineSeries() { Title = "上個月", Stroke = Brushes.Red };

            ProductDb.GetProductTypeLineSeries(YearSalesLineSeries, LastYearSalesLineSeries, MonthSalesLineSeries, LastMonthSalesLineSeries, typeId);

            UpdateLineChartUi();
           
        }

        private void UpdateLineChartUi()
        {
            SalesLineSeries.Clear();

            if ((bool) MonthRadioButton.IsChecked)
            {
                LineChartAxis.Labels = Days;

                if((bool)ThisYearLine.IsChecked)
                    SalesLineSeries.Add(MonthSalesLineSeries);

                if ((bool)LastYearLine.IsChecked)
                    SalesLineSeries.Add(LastMonthSalesLineSeries);
            }
            else
            {
                LineChartAxis.Labels = Months;

                if ((bool)ThisYearLine.IsChecked)
                    SalesLineSeries.Add(YearSalesLineSeries);

                if ((bool)LastYearLine.IsChecked)
                    SalesLineSeries.Add(LastYearSalesLineSeries);
            }
        }

        private void PieChartPushOut()
        {
            foreach (PieSeries pieSeries in StockValuePieSeries)
            {
                if (pieSeries.Title.Equals(((ProductTypeManageMaster) TypeMaster.SelectedItem).Name))
                    pieSeries.PushOut = 20;
                else
                {
                    pieSeries.PushOut = 0;
                }
            }

            foreach (PieSeries pieSeries in SalesPieSeries)
            {
                if (pieSeries.Title.Equals(((ProductTypeManageMaster)TypeMaster.SelectedItem).Name))
                    pieSeries.PushOut = 20;
                else
                {
                    pieSeries.PushOut = 0;
                }
            }
        }

        private void TypeDetail_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid) is null || (sender as DataGrid).SelectedItem is null) return;

            ProductsGrid.Items.Filter = item => ((IProductType)item).TypeId == ((ProductTypeManageDetail)(sender as DataGrid).SelectedItem).Id;

            SmallType.Text = ((ProductTypeManageDetail) (sender as DataGrid).SelectedItem).Name;
        }

        private void LineChartRange_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateLineChartUi();
        }

        private void ShowProductDetail(object sender, MouseButtonEventArgs e)
        {
            Product newProduct = ProductDb.GetProductById(((Product) ProductsGrid.SelectedItem).Id);

            if (ProductDetail.Instance is null)
            {
                ProductDetail newProductDetail = new ProductDetail();
                newProductDetail.Show();
            }

            ProductDetail.Instance.AddNewTab(newProduct);
            ProductDetail.Instance.Focus();
        }

        private void CancelTypeChange(object sender, RoutedEventArgs e)
        {
            if (TypeDetail.SelectedItem != null)
            {
                SmallType.Text = ((ProductTypeManageDetail)TypeDetail.SelectedItem).Name;
            }
        }

        private void ConfirmTypeChange(object sender, RoutedEventArgs e)
        {
            if (TypeDetail.SelectedItem != null)
            {
                ((ProductTypeManageDetail)TypeDetail.SelectedItem).Name = SmallType.Text;
                ProductDb.UpdateProductType(((ProductTypeManageDetail)TypeDetail.SelectedItem).Id, SmallType.Text);
            }
        }
    }
}
