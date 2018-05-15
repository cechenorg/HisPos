using His_Pos.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Threading;
using His_Pos.Service;
using RadioButton = System.Windows.Controls.RadioButton;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.StockTaking
{
    /// <summary>
    /// StockTakingView.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingView : UserControl, INotifyPropertyChanged
    {
        public Collection<Product> ProductCollection;
        public ListCollectionView ProductTypeCollection;
        public ObservableCollection<Product> takingCollection = new ObservableCollection<Product>();
        public ObservableCollection<Product> TakingCollection {
            get
            {
                return takingCollection;
            }
            set
            {
                takingCollection = value;
                NotifyPropertyChanged("TakingCollection");
            }
        }
        private StockTakingStatus stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public StockTakingView()
        {
            InitializeComponent();
            UpdateUi();
            SetOtcTypeUi();
            DataContext = this;
            InitProduct();
        }
      
        public void SetOtcTypeUi()
        {
            ProductTypeCollection = ProductDb.GetProductType();
            OtcType.ItemsSource = ProductTypeCollection;
            OtcType.SelectedValue = "無";
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
                    ClearProduct.Visibility = Visibility.Visible;
                    FinishedAddProduct.Visibility = Visibility.Visible;
                    Print.Visibility = Visibility.Collapsed;
                    Row1Rectangle.Width = 610;
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
                    ClearProduct.Visibility = Visibility.Collapsed;
                    FinishedAddProduct.Visibility = Visibility.Collapsed;
                    Print.Visibility = Visibility.Visible;
                    Row1Rectangle.Width = 780;
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
        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if (takingCollection.Contains(selectedItem))
                {
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                }

                CheckItems.SelectedItem = selectedItem;
                return;
            }

            CheckItems.SelectedIndex = takingCollection.Count;
        }
        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is IDeletable)
            {
                (leaveItem as IDeletable).Source = string.Empty;
            }
        }
        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            takingCollection.RemoveAt(CheckItems.SelectedIndex);
        }
        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
            UpdateUi();
        }
        public bool CaculateValidDate(string validdate, string month) {
            if (String.IsNullOrEmpty(month) || String.IsNullOrEmpty(validdate)) return false;
            validdate = validdate.Replace("/","");
            int compareDate = Int32.Parse(DateTime.Now.AddMonths(Int32.Parse(month)).ToString("yyyyMMdd"));
            if (Int32.Parse(validdate) <= compareDate && Int32.Parse(validdate) > Int32.Parse(DateTime.Now.ToString("yyyyMMdd")) )return true;
            return false;
        }

        private void AddItems_Click(object sender, RoutedEventArgs e)
        {
            
               var result = ProductCollection.Where(x => (
                (((IStockTaking)x).Location.Contains(Location.Text) || Location.Text == string.Empty)
            &&  (((Product)x).Id.Contains(ProductId.Text) || ProductId.Text == string.Empty)
            && (((Product)x).Name.Contains(ProductName.Text) || ProductName.Text == string.Empty)
            && (CaculateValidDate(((IStockTaking)x).ValidDate,ValidDate.Text) || ValidDate.Text == string.Empty)
            && ( (((IStockTaking)x).Inventory <= ((IStockTaking)x).SafeAmount && (bool)SafeAmount.IsChecked == true) || (bool)SafeAmount.IsChecked == false)
            && (( x is StockTakingOTC && ((StockTakingOTC)x).Category.Contains(OtcType.SelectedValue.ToString())) || OtcType.SelectedValue.ToString() == string.Empty || OtcType.SelectedValue.ToString() == "無")
            || (TakingCollection.Contains(x))
            ));
            TakingCollection = new ObservableCollection<Product>(result.ToList());
        }

        private void ClearProduct_Click(object sender, RoutedEventArgs e)
        {
            TakingCollection.Clear();
        }

        private void OrderMode_OnChecked(object sender, RoutedEventArgs e)
        {
            if(CheckItems is null) return;

            RadioButton radioButton = sender as RadioButton;;

            CheckItems.Items.SortDescriptions.Clear();
            CheckItems.Items.SortDescriptions.Add(new SortDescription(CheckItems.Columns[Int32.Parse(radioButton.Tag.ToString())].SortMemberPath, ListSortDirection.Ascending));
        }

        private void Print_OnClick(object sender, RoutedEventArgs e)
        {
            stockTakingStatus++;
            UpdateUi();

            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

            printPreviewDialog.Document = 
        }
    }
}
