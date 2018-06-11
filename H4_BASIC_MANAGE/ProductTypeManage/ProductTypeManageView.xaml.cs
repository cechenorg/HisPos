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
using LiveCharts;
using LiveCharts.Wpf;

namespace His_Pos.ProductTypeManage
{
    /// <summary>
    /// ProductTypeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductTypeManageView : UserControl, INotifyPropertyChanged
    {
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

        public SeriesCollection stockValuePieSeries = new SeriesCollection();

        public SeriesCollection StockValuePieSeries
        {
            get { return stockValuePieSeries; }
            set
            {
                stockValuePieSeries = value;
                NotifyPropertyChanged("StockValuePieSeries");
            }
        }

        public SeriesCollection salesPieSeries = new SeriesCollection();

        public SeriesCollection SalesPieSeries
        {
            get { return salesPieSeries; }
            set
            {
                salesPieSeries = value;
                NotifyPropertyChanged("SalesPieSeries");
            }
        }

        public ProductTypeManageView()
        {
            InitializeComponent();

            DataContext = this;

            InitTypes();
            InitProducts();
            InitPieCharts();

            TypeMaster.ItemsSource = TypeManageMasters;
            TypeDetail.ItemsSource = TypeManageDetails;
            TypeMaster.SelectedIndex = 0;
        }

        private void InitPieCharts()
        {
            foreach (var productType in TypeManageMasters)
            {
                PieSeries newStockPieSeries = new PieSeries()
                {
                    Title = productType.Name,
                    Values = new ChartValues<double> { productType.StockValue },
                    DataLabels = true
                };

                StockValuePieSeries.Add(newStockPieSeries);

                PieSeries newSalesPieSeries = new PieSeries()
                {
                    Title = productType.Name,
                    Values = new ChartValues<double> { productType.Sales },
                    DataLabels = true
                };

                SalesPieSeries.Add(newSalesPieSeries);
            }
        }

        private void InitProducts()
        {
            Products = ProductDb.GetProductTypeManageProducts();
        }

        private void InitTypes()
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

            if (TypeDetail.Items.Count != 0)
                TypeDetail.SelectedIndex = 0;
        }

        private void TypeDetail_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid) is null || (sender as DataGrid).SelectedItem is null) return;

            ProductsGrid.Items.Filter = item => ((IProductType)item).TypeId == ((ProductTypeManageDetail)(sender as DataGrid).SelectedItem).Id;
        }
    }
}
