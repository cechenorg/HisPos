using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Person;
using His_Pos.Class.StoreOrder;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace His_Pos.ProductPurchaseRecord
{
    /// <summary>
    /// ProductPurchaseRecordView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseRecordView : UserControl, INotifyPropertyChanged
    {
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

        public static bool DataChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public ProductPurchaseRecordView()
        {
            InitializeComponent();
            Instance = this;
            Focusable = true;
            UpdateUi();
            InitUser();
            InitManufactory();
            DataContext = this;
            PassValueSearchData();
        }
        private void InitManufactory() {
            //foreach (DataRow row in MainWindow.ManufactoryTable.Rows)
            //{
            //    ManufactoryAutoCompleteCollection.Add(new Manufactory(row));
            //}
            Manufactory.ItemsSource = ManufactoryAutoCompleteCollection;
            Manufactory.ItemFilter = ManufactoryFilter;
        }
        public AutoCompleteFilterPredicate<object> ManufactoryFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as Manufactory).Id is null) ? true : (obj as Manufactory).Id.Contains(searchText)
                                                               || (obj as Manufactory).Name.Contains(searchText);
            }
        }
        private void InitUser()
        {
            UserAutoCompleteCollection = PersonDb.GetUserCollection();
            OrdEmp.ItemsSource = UserAutoCompleteCollection;
            OrdEmp.ItemFilter = UserFilter;
            ReceiveEmp.ItemsSource = UserAutoCompleteCollection;
            ReceiveEmp.ItemFilter = UserFilter;
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
        public void UpdateUi() {
            DataChanged = false;
            storeOrderCollection = StoreOrderDb.GetStoreOrderOverview(Class.OrderType.DONE);
            StoOrderOverview.ItemsSource = storeOrderCollection;
            StoOrderOverview.SelectedIndex = 0;
        }

        private void ShowOrderDetail(object sender, RoutedEventArgs e)
        {
            StoreOrder storeOrder = (StoreOrder)(sender as DataGridCell).DataContext;
            UpdateOrderDetailData(storeOrder);
        }
        private void UpdateOrderDetailData(StoreOrder storeOrder)
        {
            StoreOrderData = storeOrder;

            if (StoreOrderData.Products is null)
                StoreOrderData.Products = StoreOrderDb.GetStoreOrderCollectionById(StoreOrderData.Id);
           

            StoreOrderDetail.ItemsSource = StoreOrderData.Products;
            TotalAmount.Content = StoreOrderData.Products.Count.ToString();
        }

        private void ID_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
        public bool StoOrderOverviewFilter(object item)
        {
            bool reply = false;
            int id = Convert.ToInt32(((StoreOrder)item).Id.Substring(1,8));
            switch (((StoreOrder)item).Category.CategoryName) {
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
                case "調貨":
                        if (
                           ((bool)RadioButtonTransfer.IsChecked || (bool)RadioButtonAll.IsChecked)
                           && (((StoreOrder)item).Id.Contains(TextBoxId.Text) || TextBoxId.Text == string.Empty)
                           && (((StoreOrder)item).Manufactory.Name.Contains(Manufactory.Text) || Manufactory.Text == string.Empty)
                           && (((StoreOrder)item).OrdEmp.Contains(OrdEmp.Text) || OrdEmp.Text == string.Empty)
                           && (((StoreOrder)item).RecEmp.Contains(ReceiveEmp.Text) || ReceiveEmp.Text == string.Empty)
                           && (id <= edate && id >= sdate)
                           ) reply = true;
                    break;
            }
            return reply;
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SearchData();
        }

        private void SearchData()
        {
            
            sdate = datestart.SelectedDate.ToString() != "" ?Convert.ToInt32(((DateTime)datestart.SelectedDate).ToString("yyyyMMdd")) : 0;
            edate = dateend.SelectedDate.ToString() != "" ?Convert.ToInt32(((DateTime)dateend.SelectedDate).ToString("yyyyMMdd")) : 99999999;
            StoOrderOverview.Items.Filter = StoOrderOverviewFilter;
            StoreOrderData = null;
            StoOrderOverview.SelectedIndex = 0;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(StoOrderOverview != null)
            SearchData();
        }

        public void PassValueSearchData() {
            if (Proid != null)
            {
                TextBoxId.Text = Proid;
                SearchData();
                Proid = null;
            }
        }
    }
}
