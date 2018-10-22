using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Interface;
using His_Pos.StockTaking;
using Microsoft.Reporting.WinForms;

namespace His_Pos.H3_STOCKTAKING.StockTaking
{
    /// <summary>
    /// StockTakingView.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Product> ProductCollection;
        private ListCollectionView _productTypeCollection;
        private ObservableCollection<Product> _takingCollection = new ObservableCollection<Product>();
        public static StockTakingView Instance;
        public static bool DataChanged { get; set; }
        public ObservableCollection<Product> TakingCollection
        {
            get => _takingCollection;
            set
            {
                _takingCollection = value;
                NotifyPropertyChanged(nameof(TakingCollection));
            }
        }
        private StockTakingStatus _stockTakingStatus = StockTakingStatus.ADDPRODUCTS;

        private int _resultFilled;
        public int ResultFilled
        {
            get => _resultFilled;
            set
            {
                _resultFilled = value;
                NotifyPropertyChanged(nameof(ResultFilled));
            }
        }

        private int _resultNotFilled;
        public int ResultNotFilled
        {
            get => _resultNotFilled;
            set
            {
                _resultNotFilled = value;
                NotifyPropertyChanged(nameof(ResultNotFilled));
            }
        }

        private int _resultChanged;

        public int ResultChanged
        {
            get => _resultChanged;
            set
            {
                _resultChanged = value;
                NotifyPropertyChanged(nameof(ResultChanged));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public StockTakingView()
        {
            InitializeComponent();
            UpdateUi();
            SetOtcTypeUi();
            Instance = this;
            DataContext = this;
            DataChanged = false;
            var userAutoCompleteCollection = PersonDb.GetUserCollection();
            TakingEmp.ItemsSource = userAutoCompleteCollection;
            TakingEmp.ItemFilter = UserFilter;
            InitProduct();
        }

        private AutoCompleteFilterPredicate<object> UserFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Person)?.Id is null || ((Person) obj).Id.Contains(searchText) || ((Person)obj).Name.Contains(searchText);
            }
        }

        public void SetOtcTypeUi()
        {
            _productTypeCollection = ProductDb.GetProductType();
            OtcType.ItemsSource = _productTypeCollection;
            OtcType.SelectedValue = "無";
        }
        public void InitProduct()
        {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.MergeProductStockTaking(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        }

        private void UpdateUi()
        {
            switch (_stockTakingStatus)
            {
                case StockTakingStatus.ADDPRODUCTS:
                    CheckItems.Columns[0].Visibility = Visibility.Visible;
                    CheckItems.Columns[5].Visibility = Visibility.Visible;
                    CheckItems.Columns[6].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[7].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[8].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[9].Visibility = Visibility.Visible;
                    CheckItems.Columns[10].Visibility = Visibility.Visible;
                    CheckItems.Columns[11].Visibility = Visibility.Visible;
                    CheckItems.Columns[12].Visibility = Visibility.Collapsed;
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
                    CheckItems.Columns[0].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[6].Visibility = Visibility.Visible;
                    CheckItems.Columns[7].Visibility = Visibility.Visible;
                    CheckItems.Columns[8].Visibility = Visibility.Visible;
                    CheckItems.Columns[9].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[10].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[11].Visibility = Visibility.Collapsed;
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
                    CheckItems.Columns[7].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[8].Visibility = Visibility.Collapsed;
                    CheckItems.Columns[12].Visibility = Visibility.Visible;
                    ViewGrid.RowDefinitions[7].Height = new GridLength(0);
                    ViewGrid.RowDefinitions[8].Height = new GridLength(50);
                    CheckItems.Columns[11].Visibility = Visibility.Visible;
                    InputEllipse.Fill = Brushes.DeepSkyBlue;
                    CompleteLine.Stroke = Brushes.DeepSkyBlue;
                    CompleteLine.StrokeDashArray = new DoubleCollection() { 300 };
                    CompleteEllipse.Fill = Brushes.GreenYellow;
                    break;
            }
        }

        private void StockTakingComplete_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckTakingResult(_takingCollection))
            {
                var messageWindow = new MessageWindow("尚有品項未填寫盤點數量!", MessageType.ERROR, true);
                messageWindow.ShowDialog();
                return;
            }

            if (ResultChanged == 0)
            {
                ProductDb.SaveStockTaking(_takingCollection, true);

                _takingCollection.Clear();

                InitToBegin();
            }
            else
            {
                CheckItems.Items.Filter = ChangedFilter;

                NextStatus();
            }

        }

        private void InitToBegin()
        {
            _stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
            ClearAddCondition();
            UpdateUi();
            InitProduct();
        }

        private bool ChangedFilter(object product)
        {
            return !((IStockTaking)product).IsEqual;
        }

        private void NextStatus()
        {
            _stockTakingStatus++;
            UpdateUi();
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;

            if (selectedItem is IDeletable deletable)
            {
                if (_takingCollection.Contains(selectedItem))
                {
                    deletable.Source = "/Images/DeleteDot.png";
                }

                CheckItems.SelectedItem = deletable;
                return;
            }

            CheckItems.SelectedIndex = _takingCollection.Count;
        }
        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow)?.Item;

            if (leaveItem is IDeletable deletable)
            {
                deletable.Source = string.Empty;
            }
        }
        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _takingCollection.RemoveAt(CheckItems.SelectedIndex);
        }
        private bool CheckTakingResult(IEnumerable<Product> taking)
        {
            return taking.All(p => ((IStockTaking)p).TakingResult != string.Empty);
        }
        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            ProductDb.SaveStockTaking(_takingCollection, true);
            _takingCollection.Clear();
            CheckItems.Items.Filter = null;
            InitToBegin();
        }

        private bool CaculateValidDate(string validdate, string month)
        {
            if (string.IsNullOrEmpty(month) || string.IsNullOrEmpty(validdate)) return false;
            validdate = validdate.Replace("/", "");
            var compareDate = int.Parse(DateTime.Now.AddMonths(int.Parse(month)).ToString("yyyyMMdd"));
            return int.Parse(validdate) <= compareDate && int.Parse(validdate) > int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        }

        private void AddItems_Click(object sender, RoutedEventArgs e)
        {

            var result = ProductCollection.Where(x =>
            FreezeMed.IsChecked != null && (ControlMed.IsChecked != null && SafeAmount.IsChecked != null && ((((IStockTaking)x).Location.Contains(Location.Text) || Location.Text == string.Empty)
                                                                                                             && (x.Id.Contains(ProductId.Text) || ProductId.Text == string.Empty)
                                                                                                             && (x.Name.ToLower().Contains(ProductName.Text.ToLower()) || ProductName.Text == string.Empty)
                                                                                                             && (CaculateValidDate(((IStockTaking)x).ValidDate, ValidDate.Text) || ValidDate.Text == string.Empty)
                                                                                                             && (((IStockTaking)x).Inventory <= ((IStockTaking)x).SafeAmount && (bool)SafeAmount.IsChecked || SafeAmount.IsChecked == false)
                                                                                                             && (x is StockTakingOTC otc && otc.Category.Contains(OtcType.SelectedValue.ToString()) || OtcType.SelectedValue.ToString() == string.Empty || OtcType.SelectedValue.ToString().Equals("無"))
                                                                                                             && (x is StockTakingMedicine medicine && (bool)ControlMed.IsChecked && medicine.Control || !(bool)ControlMed.IsChecked)
                                                                                                             && (x is StockTakingMedicine takingMedicine && (bool)FreezeMed.IsChecked && takingMedicine.Frozen || !(bool)FreezeMed.IsChecked)
                                                                                                             || TakingCollection.Contains(x))));

            TakingCollection = new ObservableCollection<Product>(result.ToList());

            AddStockTakingEmp();

            ClearAddCondition();
        }

        private void AddStockTakingEmp()
        {
            foreach (var product in _takingCollection)
            {
                if (((IStockTaking)product).EmpId.Equals(""))
                    ((IStockTaking)product).EmpId = (TakingEmp.Text.Equals("")) ? MainWindow.CurrentUser.Id : TakingEmp.Text;
            }
        }

        private void ClearAddCondition()
        {
            Location.Text = "";
            ProductId.Text = "";
            ProductName.Text = "";
            ValidDate.Text = "";
            TakingEmp.Text = "";
            OtcType.SelectedIndex = 0;

            FreezeMed.IsChecked = false;
            ControlMed.IsChecked = false;
            SafeAmount.IsChecked = false;
        }

        private void ClearProduct_Click(object sender, RoutedEventArgs e)
        {
            foreach (var product in _takingCollection)
            {
                ((IStockTaking)product).EmpId = "";
            }

            TakingCollection.Clear();
        }

        private void OrderMode_OnChecked(object sender, RoutedEventArgs e)
        {
            if (CheckItems is null) return;

            var radioButton = sender as RadioButton;

            CheckItems.Items.SortDescriptions.Clear();
            CheckItems.Items.SortDescriptions.Add(new SortDescription(CheckItems.Columns[13].SortMemberPath, ListSortDirection.Ascending));
            if (radioButton != null)
            {
                CheckItems.Items.SortDescriptions.Add(new SortDescription(
                    CheckItems.Columns[int.Parse(radioButton.Tag.ToString())].SortMemberPath,
                    ListSortDirection.Ascending));
            }
        }

        private void Print_OnClick(object sender, RoutedEventArgs e)
        {
            InventoryChecking.InventoryObjectList.Clear();
            foreach (var product in _takingCollection)
            {
                if (product is StockTakingOTC otc)
                {
                    foreach (var batch in otc.BatchNumbersCollection)
                    {
                        InventoryObject invObj = new InventoryObject(otc.Id, otc.Name, otc.Category, batch.BatchNumber, batch.Amount, otc.Inventory, otc.ValidDate, otc.Location);
                        InventoryChecking.InventoryObjectList.Add(invObj);
                    }
                }
                else if (product is StockTakingMedicine med)
                {
                    foreach (var batch in med.BatchNumbersCollection)
                    {
                        InventoryObject invObj = new InventoryObject(med.Id, med.Name, med.Category, batch.BatchNumber, batch.Amount, med.Inventory, med.ValidDate, med.Location);
                        InventoryChecking.InventoryObjectList.Add(invObj);
                    }
                }
            }
            InventoryChecking.MergeData(InventoryChecking.GetInventoryObjectList());
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.DataSources.Add(new ReportDataSource("InventoryDataSet", InventoryChecking.t));
            var parameters = new ReportParameter[2];
            var recEmp = string.IsNullOrEmpty(TakingEmp.Text) ? MainWindow.CurrentUser.Name : TakingEmp.Text;
            parameters[0] = new ReportParameter("CurrentUser", recEmp);
            parameters[1] = new ReportParameter("ProductCount", TakingCollection.Count.ToString());
            rptViewer.LocalReport.ReportPath = @"..\..\RDLC\InventoryCheckSheet.rdlc";
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.Refresh();
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var loadingWindow = new LoadingWindow();
            loadingWindow.Show();
            loadingWindow.PrintInventoryCheckSheet(rptViewer, Instance);
        }


        private void AddOneItem_Click(object sender, RoutedEventArgs e)
        {
            StockTakingItemDialog stockTakingItemDialog = new StockTakingItemDialog(ProductCollection, TakingCollection);
            stockTakingItemDialog.ShowDialog();

            if (!stockTakingItemDialog.ConfirmButtonClicked) return;
            TakingCollection.Add(stockTakingItemDialog.SelectedItem);

            TakingEmp.Text = stockTakingItemDialog.SelectedUser;

            AddStockTakingEmp();
            ClearAddCondition();
        }

        private void CountFilledResult()
        {
            ResultFilled = _takingCollection.Count(x => ((IStockTaking)x).IsChecked);
            ResultNotFilled = _takingCollection.Count(x => !((IStockTaking)x).IsChecked);

            ResultChanged = _takingCollection.Count(x =>
                !((IStockTaking)x).Inventory.ToString().Equals(((IStockTaking)x).TakingResult) && !((IStockTaking)x).TakingResult.Equals(String.Empty));
        }

        private void StockTakingResult_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CountFilledResult();
        }

        private void AutoFill_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in _takingCollection)
            {
                ((IStockTaking)p).TakingResult = ((IStockTaking)p).TakingResult == string.Empty ? ((IStockTaking)p).Inventory.ToString() : ((IStockTaking)p).TakingResult;
            }
        }

        private void FinishedAddProduct_Click(object sender, RoutedEventArgs e)
        {
            CheckItems.Items.SortDescriptions.Clear();
            CheckItems.Items.SortDescriptions.Add(new SortDescription(CheckItems.Columns[13].SortMemberPath, ListSortDirection.Ascending));

            NextStatus();
        }

    }
    public class IsResultEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !(bool)value)
                return "/Images/Changed.png";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
