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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Threading;
using His_Pos.PrintDocuments;
using His_Pos.Service;
using System.Windows.Markup;
using His_Pos.Class.Person;

namespace His_Pos.StockTaking
{
    /// <summary>
    /// StockTakingView.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Product> ProductCollection;
        public ListCollectionView ProductTypeCollection;
        public ObservableCollection<Product> takingCollection = new ObservableCollection<Product>();
        public static StockTakingView Instance;
        public static bool DataChanged { get; set; } = false;
        public ObservableCollection<Product> TakingCollection
        {
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

        private int resultFilled;
        public int ResultFilled
        {
            get { return resultFilled; }
            set
            {
                resultFilled = value;
                NotifyPropertyChanged("ResultFilled");
            }
        }

        private int resultNotFilled;
        public int ResultNotFilled
        {
            get { return resultNotFilled; }
            set
            {
                resultNotFilled = value;
                NotifyPropertyChanged("ResultNotFilled");
            }
        }

        private int resultChanged;
        private ObservableCollection<Person> UserAutoCompleteCollection;

        public int ResultChanged
        {
            get { return resultChanged; }
            set
            {
                resultChanged = value;
                NotifyPropertyChanged("ResultChanged");
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
        public StockTakingView()
        {
            InitializeComponent();
            UpdateUi();
            SetOtcTypeUi();
            Instance = this;
            DataContext = this;
            UserAutoCompleteCollection = PersonDb.GetUserCollection();
            TakingEmp.ItemsSource = UserAutoCompleteCollection;
            TakingEmp.ItemFilter = UserFilter;
            InitProduct();
        }
        public AutoCompleteFilterPredicate<object> UserFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as Person).Id is null) ? true : (obj as Person).Id.Contains(searchText)
                    || (obj as Person).Name.Contains(searchText);
            }
        }

        public void SetOtcTypeUi()
        {
            ProductTypeCollection = ProductDb.GetProductType();
            OtcType.ItemsSource = ProductTypeCollection;
            OtcType.SelectedValue = "無";
        }
        public void InitProduct()
        {
            DataChanged = false;
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
            if (!CheckTakingResult(takingCollection))
            {
                MessageWindow messageWindow = new MessageWindow("尚有品項未填寫盤點數量!", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }

            if(ResultChanged == 0)
            {
                ProductDb.SaveStockTaking(takingCollection, true);

                takingCollection.Clear();

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
            stockTakingStatus = StockTakingStatus.ADDPRODUCTS;
            ClearAddCondition();
            UpdateUi();
            InitProduct();
        }

        private bool ChangedFilter( object product )
        {
            return !(product as IStockTaking).IsEqual;
        }

        private void NextStatus()
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
        private bool CheckTakingResult(ObservableCollection<Product> takingCollection) {
            foreach (Product p in takingCollection)
            {
                if (((IStockTaking)p).TakingResult == string.Empty) return false;
            }
            return true;
        }
        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            ProductDb.SaveStockTaking(takingCollection, true);
            takingCollection.Clear();
            CheckItems.Items.Filter = null;
            InitToBegin();
        }
        public bool CaculateValidDate(string validdate, string month)
        {
            if (String.IsNullOrEmpty(month) || String.IsNullOrEmpty(validdate)) return false;
            validdate = validdate.Replace("/", "");
            int compareDate = Int32.Parse(DateTime.Now.AddMonths(Int32.Parse(month)).ToString("yyyyMMdd"));
            if (Int32.Parse(validdate) <= compareDate && Int32.Parse(validdate) > Int32.Parse(DateTime.Now.ToString("yyyyMMdd"))) return true;
            return false;
        }

        private void AddItems_Click(object sender, RoutedEventArgs e)
        {

            var result = ProductCollection.Where(x => 
            (((IStockTaking)x).Location.Contains(Location.Text) || Location.Text == string.Empty)
            && (((Product)x).Id.Contains(ProductId.Text) || ProductId.Text == string.Empty)
            && (((Product)x).Name.Contains(ProductName.Text) || ProductName.Text == string.Empty)
            && (CaculateValidDate(((IStockTaking)x).ValidDate, ValidDate.Text) || ValidDate.Text == string.Empty)
            && ((((IStockTaking)x).Inventory <= ((IStockTaking)x).SafeAmount && (bool)SafeAmount.IsChecked == true) || (bool)SafeAmount.IsChecked == false)
            && ((x is StockTakingOTC && ((StockTakingOTC)x).Category.Contains(OtcType.SelectedValue.ToString())) || OtcType.SelectedValue.ToString() == string.Empty || OtcType.SelectedValue.ToString() == "無")
            && ( (x is StockTakingMedicine && (bool)ControlMed.IsChecked && ((StockTakingMedicine)x).Control )  || !(bool)ControlMed.IsChecked)
            && ((x is StockTakingMedicine && (bool)FreezeMed.IsChecked && ((StockTakingMedicine)x).Frozen) || !(bool)FreezeMed.IsChecked)
            || TakingCollection.Contains(x));

            TakingCollection = new ObservableCollection<Product>(result.ToList());

            AddStockTakingEmp();
            
            ClearAddCondition();
        }

        private void AddStockTakingEmp()
        {
            foreach(var product in takingCollection)
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
            foreach (var product in takingCollection)
            {
                ((IStockTaking)product).EmpId = "";
            }

            TakingCollection.Clear();
        }

        private void OrderMode_OnChecked(object sender, RoutedEventArgs e)
        {
            if (CheckItems is null) return;

            RadioButton radioButton = sender as RadioButton; ;

            CheckItems.Items.SortDescriptions.Clear();
            CheckItems.Items.SortDescriptions.Add(new SortDescription(CheckItems.Columns[13].SortMemberPath, ListSortDirection.Ascending));
            CheckItems.Items.SortDescriptions.Add(new SortDescription(CheckItems.Columns[Int32.Parse(radioButton.Tag.ToString())].SortMemberPath, ListSortDirection.Ascending));
        }

        private void Print_OnClick(object sender, RoutedEventArgs e)
        {
            var pageSize = new Size(8.26 * 96, 11.69 * 96);
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;

            Collection<FixedPage> stockTakingDocuments = ConvertDataToDoc(pageSize);
            
            foreach( var page in stockTakingDocuments)
            {
                var pageContent = new PageContent();
                pageContent.Child = page;
                document.Pages.Add(pageContent);
            }

            if (NewFunction.DocumentPrinter(document, "盤點單" + DateTime.Now.ToShortDateString()))
            {
                CountFilledResult();
                ProductDb.SaveStockTaking(takingCollection, false);
                NextStatus();
            }   
        }

        private Collection<FixedPage> ConvertDataToDoc(Size pageSize)
        {
            Collection<FixedPage> documents = new Collection<FixedPage>();

            Collection< List < Product >> pages = new BindingList<List<Product>>();

            List<RadioButton> radioButtons = SortType.Children.OfType<RadioButton>().ToList();
            int target = Int16.Parse(radioButtons.Single(r => r.IsChecked == true).Tag.ToString());

            List<Product> temp;

            switch (target)
            {
                case 1:
                    temp = takingCollection.OrderBy(x => ((IStockTaking)x).EmpId).ThenBy(x => x.Id).ToList();
                    break;
                case 2:
                    temp = takingCollection.OrderBy(x => ((IStockTaking)x).EmpId).ThenBy(x => x.Name).ToList();
                    break;
                case 3:
                    temp = takingCollection.OrderBy(x => ((IStockTaking)x).EmpId).ThenBy(x => ((IStockTaking)x).Category).ThenBy(x => x.Id).ToList();
                    break;
                case 9:
                    temp = takingCollection.OrderBy(x => ((IStockTaking)x).EmpId).ThenBy(x => ((IStockTaking)x).Location).ThenBy(x => x.Id).ToList();
                    break;
                default:
                    temp = takingCollection.OrderBy(x => ((IStockTaking)x).EmpId).ToList();
                    break;
            }

            int LENGTH_LIMIT = 25;

            double length = 0;
            int lastIndex = 0;
            for (int x = 0; x < temp.Count; x ++)
            {
                int itemCount = ((IStockTaking)temp[x]).BatchNumbersCollection.Count;
                if (itemCount == 1)
                    length += 0.9;
                else
                {
                    length += (0.8 * (itemCount - 2) + 1.2);
                }

                if (x != 0 && ((IStockTaking)temp[x - 1]).EmpId != ((IStockTaking)temp[x]).EmpId)
                {
                    pages.Add(temp.GetRange(lastIndex, x - lastIndex));
                    lastIndex = x;
                    length = 0;
                }
                else if (length >= LENGTH_LIMIT)
                {
                    pages.Add(temp.GetRange(lastIndex, x - 1 - lastIndex));
                    lastIndex = x - 1;
                    length = 0;
                }
                else if (x == temp.Count - 1)
                {
                    pages.Add(temp.GetRange(lastIndex, x - lastIndex + 1));
                }
            }
            
            int totalPage = pages.Count;

            for (int x = 0; x < pages.Count; x ++)
            {
                var fixedPage = new FixedPage();
                fixedPage.Width = pageSize.Width;
                fixedPage.Height = pageSize.Height;
                if (CheckItems.Items.SortDescriptions.Count > 1)
                    fixedPage.Children.Add(new StockTakingDocument(pages[x], ((IStockTaking)pages[x][0]).EmpId, takingCollection.Count, x + 1, totalPage, CheckItems.Items.SortDescriptions[1]));
                else
                    fixedPage.Children.Add(new StockTakingDocument(pages[x], ((IStockTaking)pages[x][0]).EmpId, takingCollection.Count, x + 1, totalPage, new SortDescription(CheckItems.Columns[1].SortMemberPath, ListSortDirection.Ascending)));
               
                fixedPage.Measure(pageSize);
                fixedPage.Arrange(new Rect(new Point(), pageSize));
                fixedPage.UpdateLayout();

                documents.Add(fixedPage);
            }

            return documents;
        }

        private void AddOneItem_Click(object sender, RoutedEventArgs e)
        {
            StockTakingItemDialog stockTakingItemDialog = new StockTakingItemDialog(ProductCollection,TakingCollection);
            stockTakingItemDialog.ShowDialog();

            if (stockTakingItemDialog.ConfirmButtonClicked)
            {
                TakingCollection.Add(stockTakingItemDialog.SelectedItem as Product);

                TakingEmp.Text = stockTakingItemDialog.SelectedUser;

                AddStockTakingEmp();
                ClearAddCondition();
            }
        }

        private void CountFilledResult()
        {
            ResultFilled = takingCollection.Count(x => ((IStockTaking)x).IsChecked);
            ResultNotFilled = takingCollection.Count(x => !((IStockTaking)x).IsChecked);

            ResultChanged = takingCollection.Count(x =>
                !((IStockTaking) x).Inventory.ToString().Equals(((IStockTaking) x).TakingResult) && !((IStockTaking)x).TakingResult.Equals(String.Empty));
        }

        private void StockTakingResult_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CountFilledResult();
        }

        private void AutoFill_Click(object sender, RoutedEventArgs e)
        {
            foreach (Product p in takingCollection) {
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
            if (!(bool) value)
                return "/Images/Changed.png";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
