using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Person;
using His_Pos.Class.StoreOrder;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    /// <summary>
    /// ProductPurchaseRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseRecordView : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Variables -----
        public static ProductPurchaseRecordView Instance;
        public ObservableCollection<StoreOrder> storeOrderCollection;
        private StoreOrder storeOrderData;
        public ObservableCollection<Person> UserAutoCompleteCollection;
        private ObservableCollection<Manufactory> ManufactoryAutoCompleteCollection = new ObservableCollection<Manufactory>();
        private int sdate,edate;
        public static string Proid;
        public StoreOrder StoreOrderData
        {
            get { return storeOrderData; }
            set
            {
                storeOrderData = value;
                NotifyPropertyChanged("StoreOrderData");
            }
        }

        private DataGrid CurrentDataGrid { get; set; }

        public static bool DataChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
        public ProductPurchaseRecordView()
        {
            InitializeComponent();
            Instance = this;
            UpdateUi();
            InitUser();
            InitManufactory();
            DataContext = this;
            PassValueSearchData();
        }
    
        #region ----- Init Data -----
        public void UpdateUi()
        {
            DataChanged = false;
            CurrentDataGrid = PStoreOrderDetail;
            storeOrderCollection = StoreOrderDb.GetStoreOrderOverview(Class.OrderType.DONE);
            StoOrderOverview.ItemsSource = storeOrderCollection;
            StoOrderOverview.SelectedIndex = 0;
        }
        private void InitManufactory()
        {
            //foreach (DataRow row in MainWindow.ManufactoryTable.Rows)
            //{
            //    ManufactoryAutoCompleteCollection.Add(new Manufactory(row));
            //}
            Manufactory.ItemsSource = ManufactoryAutoCompleteCollection;
            Manufactory.ItemFilter = ManufactoryFilter;
        }
        private void InitUser()
        {
            UserAutoCompleteCollection = PersonDb.GetUserCollection();
            OrdEmp.ItemsSource = UserAutoCompleteCollection;
            OrdEmp.ItemFilter = UserFilter;
            ReceiveEmp.ItemsSource = UserAutoCompleteCollection;
            ReceiveEmp.ItemFilter = UserFilter;
        }
        public void PassValueSearchData()
        {
            if (Proid != null)
            {
                TextBoxId.Text = Proid;
                SearchData();
                Proid = null;
            }
        }
        #endregion

        #region ----- Filter -----
        public AutoCompleteFilterPredicate<object> ManufactoryFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as Manufactory).Id is null) ? true : (obj as Manufactory).Id.Contains(searchText)
                                                               || (obj as Manufactory).Name.Contains(searchText);
            }
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
        public bool StoOrderOverviewFilter(object item)
        {
            bool reply = false;
            int id = Convert.ToInt32(((StoreOrder)item).Id.Substring(1, 8));
            switch (((StoreOrder)item).Category.CategoryName)
            {
                case "進貨":
                    if (
                        ((bool)RadioButtonPurchase.IsChecked || (bool)RadioButtonAll.IsChecked)
                        && (((StoreOrder)item).Id.Contains(TextBoxId.Text) || TextBoxId.Text == string.Empty)
                        && (((StoreOrder)item).Manufactory.Name.Contains(Manufactory.Text) || Manufactory.Text == string.Empty)
                        && (((StoreOrder)item).OrdEmp.Contains(OrdEmp.Text) || OrdEmp.Text == string.Empty)
                        && (((StoreOrder)item).RecEmp.Contains(ReceiveEmp.Text) || ReceiveEmp.Text == string.Empty)
                        && (id <= edate && id >= sdate)
                        ) reply = true;
                    break;
                case "退貨":
                    if (
                       ((bool)RadioButtonReturns.IsChecked || (bool)RadioButtonAll.IsChecked)
                       && (((StoreOrder)item).Id.Contains(TextBoxId.Text) || TextBoxId.Text == string.Empty)
                       && (((StoreOrder)item).Manufactory.Name.Contains(Manufactory.Text) || Manufactory.Text == string.Empty)
                       && (((StoreOrder)item).OrdEmp.Contains(OrdEmp.Text) || OrdEmp.Text == string.Empty)
                       && (((StoreOrder)item).RecEmp.Contains(ReceiveEmp.Text) || ReceiveEmp.Text == string.Empty)
                       && (id <= edate && id >= sdate)
                       ) reply = true;
                    break;
                    //case "調貨":
                    //        if (
                    //           ((bool)RadioButtonTransfer.IsChecked || (bool)RadioButtonAll.IsChecked)
                    //           && (((StoreOrder)item).Id.Contains(TextBoxId.Text) || TextBoxId.Text == string.Empty)
                    //           && (((StoreOrder)item).Manufactory.Name.Contains(Manufactory.Text) || Manufactory.Text == string.Empty)
                    //           && (((StoreOrder)item).OrdEmp.Contains(OrdEmp.Text) || OrdEmp.Text == string.Empty)
                    //           && (((StoreOrder)item).RecEmp.Contains(ReceiveEmp.Text) || ReceiveEmp.Text == string.Empty)
                    //           && (id <= edate && id >= sdate)
                    //           ) reply = true;
                    break;
            }
            return reply;
        }
        #endregion

        #region ----- Change Order -----
        private void StoOrderOverview_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if(dataGrid is null || dataGrid.SelectedItem is null) return;

            StoreOrder storeOrder = (StoreOrder) dataGrid.SelectedItem;

            CurrentDataGrid = null;

            switch (storeOrder.Category.CategoryName)
            {
                case "進貨":
                    storeOrder.Products = StoreOrderDb.GetOrderPurchaseDetailById(storeOrder.Id);
                    CurrentDataGrid = PStoreOrderDetail;

                    DetailGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                    DetailGrid.RowDefinitions[4].Height = new GridLength(0);
                    break;
                case "退貨":
                    storeOrder.Products = StoreOrderDb.GetOrderReturnDetailById(storeOrder.Id);
                    CurrentDataGrid = RStoreOrderDetail;

                    DetailGrid.RowDefinitions[3].Height = new GridLength(0);
                    DetailGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);
                    break;
            }

            CurrentDataGrid.ItemsSource = storeOrder.Products;
            storeOrder.CalculateTotalPrice();

            StoreOrderData = storeOrder;
        }
        #endregion
        
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SearchData();
        }

        private void SearchData()
        {
            sdate = datestart.SelectedDate.ToString() != "" ?Convert.ToInt32(((DateTime)datestart.SelectedDate).ToString("yyyyMMdd")) : 0;
            edate = dateend.SelectedDate.ToString() != "" ?Convert.ToInt32(((DateTime)dateend.SelectedDate).ToString("yyyyMMdd")) : 99999999;
            StoOrderOverview.Items.Filter = StoOrderOverviewFilter;

            StoOrderOverview.SelectedIndex = 0;
            StoreOrderData = StoOrderOverview.SelectedItem as StoreOrder;

            if (StoreOrderData != null)
                CurrentDataGrid.ItemsSource = StoreOrderData.Products;
            else
                CurrentDataGrid.ItemsSource = null;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(StoOrderOverview != null)
            SearchData();
        }

        
    }
}
