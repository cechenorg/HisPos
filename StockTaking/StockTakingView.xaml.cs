using His_Pos.Class;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.AbstractClass;
using His_Pos.Class.Product;
using static His_Pos.ProductPurchase.ProductPurchaseView;
using His_Pos.Interface;

namespace His_Pos.StockTaking
{
    /// <summary>
    /// StockTakingView.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingView : UserControl
    {
        public Collection<Product> ProductCollection;
        public Collection<Product> TakingCollection = new Collection<Product>();
        private StockTakingStatus stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
        public StockTakingView()
        {
            InitializeComponent();
            UpdateUi();

            InitProduct();
        }

        private void InitProduct()
        {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.MergeProductStockTaking(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        }

        private void UpdateUi()
        {
            switch (stockTakingStatus)
            {
                case StockTakingStatus.ADDPRODUCTS:
                    ViewGrid.RowDefinitions[2].Height = new GridLength(15);
                    ViewGrid.RowDefinitions[3].Height = new GridLength(150);
                    ViewGrid.RowDefinitions[4].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[6].Height = new GridLength(50);
                    ViewGrid.RowDefinitions[7].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[8].Height = new GridLength(0);
                    AddProductTri.Visibility = Visibility.Visible;
                    PrintTri.Visibility = Visibility.Collapsed;
                    AddProductsEllipse.Fill = Brushes.GreenYellow;
                    PrintLine.Stroke = Brushes.LightSlateGray;
                    PrintLine.StrokeDashArray = new DoubleCollection() { 4 };
                    PrintEllipse.Fill = Brushes.LightSlateGray;
                    InputLine.Stroke = Brushes.LightSlateGray;
                    InputLine.StrokeDashArray = new DoubleCollection() { 4 };
                    InputEllipse.Fill = Brushes.LightSlateGray;
                    CompleteLine.Stroke = Brushes.LightSlateGray;
                    CompleteLine.StrokeDashArray = new DoubleCollection() { 4 };
                    CompleteEllipse.Fill = Brushes.LightSlateGray;
                    FinishedAddProduct.Visibility = Visibility.Visible;
                    Print.Visibility = Visibility.Collapsed;
                    break;
                case StockTakingStatus.PRINT:
                    ViewGrid.RowDefinitions[3].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[4].Height = new GridLength(50);
                    AddProductTri.Visibility = Visibility.Collapsed;
                    PrintTri.Visibility = Visibility.Visible;
                    AddProductsEllipse.Fill = Brushes.DeepSkyBlue;
                    PrintLine.Stroke = Brushes.DeepSkyBlue;
                    PrintLine.StrokeDashArray = new DoubleCollection() { 300 };
                    PrintEllipse.Fill = Brushes.GreenYellow;
                    FinishedAddProduct.Visibility = Visibility.Collapsed;
                    Print.Visibility = Visibility.Visible;
                    break;
                case StockTakingStatus.INPUTRESULT:
                    ViewGrid.RowDefinitions[4].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[6].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[7].Height = new GridLength(50);
                    PrintTri.Visibility = Visibility.Collapsed;
                    PrintEllipse.Fill = Brushes.DeepSkyBlue;
                    InputLine.Stroke = Brushes.DeepSkyBlue;
                    InputLine.StrokeDashArray = new DoubleCollection() { 300 };
                    InputEllipse.Fill = Brushes.GreenYellow;
                    break;
                case StockTakingStatus.COMPLETE:
                    ViewGrid.RowDefinitions[7].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[8].Height = new GridLength(50);
                    InputEllipse.Fill = Brushes.DeepSkyBlue;
                    CompleteLine.Stroke = Brushes.DeepSkyBlue;
                    CompleteLine.StrokeDashArray = new DoubleCollection() { 300 };
                    CompleteEllipse.Fill = Brushes.GreenYellow;
                    break;
            }
        }

        private void NextStatus_Click(object sender, RoutedEventArgs e)
        {
            stockTakingStatus++;
            UpdateUi();
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
            UpdateUi();
        }
        private bool CaculateValidDate(string validdate, string month) {
            if (String.IsNullOrEmpty(month) || String.IsNullOrEmpty(validdate)) return false;
            var reply = false;
            int nowDate = Int32.Parse(DateTime.Now.AddMonths().ToString("yyyyMMdd"));

            return reply;
        }

        private void AddItems_Click(object sender, RoutedEventArgs e)
        {

            var result = ProductCollection.Where(x => (
                (((IStockTaking)x).Location.Contains(Location.Text) || Location.Text == string.Empty)
            &&  (((Product)x).Id.Contains(ProductId.Text) || ProductId.Text == string.Empty)
            && (((Product)x).Name.Contains(ProductName.Text) || ProductName.Text == string.Empty)
            && (CaculateValidDate(((IStockTaking)x).ValidDate,ValidDate.Text) || ValidDate.Text == string.Empty)
            ));
            var tempCollection = new Collection<Product>(result.ToList());
            foreach (var product in tempCollection){
                TakingCollection.Add(product);
            }
            

        }
    }
}
