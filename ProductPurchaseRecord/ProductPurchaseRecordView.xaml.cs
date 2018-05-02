using His_Pos.Class.Manufactory;
using His_Pos.Class.StoreOrder;
using MahApps.Metro.Controls;
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
            DataContext = this;
            PassValueSearchData();
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
            int id = Convert.ToInt32(((StoreOrder)item).Id.Substring(0,8));
            switch (((StoreOrder)item).Category.CategoryName) {
                case "進貨":
                    if (
                        ((bool)RadioButtonPurchase.IsChecked || (bool)RadioButtonAll.IsChecked)
                        && (((StoreOrder)item).Id.Contains(TextBoxId.Text) || TextBoxId.Text == string.Empty)
                        && (((StoreOrder)item).Manufactory.Name.Contains(Manufactory.Text) || Manufactory.Text == string.Empty)
                        && (((StoreOrder)item).OrdEmp.Contains(OrdEmp.Text) || OrdEmp.Text == string.Empty)
                        && (id <= edate && id >= sdate)
                        && (OrdEmp.Text.Contains(((StoreOrder)item).OrdEmp) || OrdEmp.Text == string.Empty)
                        ) reply = true;
                    break;
                case "退貨":
                        if (
                           ((bool)RadioButtonReturns.IsChecked || (bool)RadioButtonAll.IsChecked)
                           && (((StoreOrder)item).Id.Contains(TextBoxId.Text) || TextBoxId.Text == string.Empty)
                           && (((StoreOrder)item).Manufactory.Name.Contains(Manufactory.Text) || Manufactory.Text == string.Empty)
                           && (((StoreOrder)item).OrdEmp.Contains(OrdEmp.Text) || OrdEmp.Text == string.Empty)
                           && (id <= edate && id >= sdate)
                           && (OrdEmp.Text.Contains(((StoreOrder)item).OrdEmp) || OrdEmp.Text == string.Empty)
                           ) reply = true;
                    break;
                case "調貨":
                        if (
                           ((bool)RadioButtonTransfer.IsChecked || (bool)RadioButtonAll.IsChecked)
                           && (((StoreOrder)item).Id.Contains(TextBoxId.Text) || TextBoxId.Text == string.Empty)
                           && (((StoreOrder)item).Manufactory.Name.Contains(Manufactory.Text) || Manufactory.Text == string.Empty)
                           && (((StoreOrder)item).OrdEmp.Contains(OrdEmp.Text) || OrdEmp.Text == string.Empty)
                           && (id <= edate && id >= sdate)
                           && (OrdEmp.Text.Contains(((StoreOrder)item).OrdEmp) || OrdEmp.Text == string.Empty)
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
