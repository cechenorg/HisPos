using His_Pos.Class.Manufactory;
using His_Pos.Class.StoreOrder;
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
        public ObservableCollection<StoreOrder> storeOrderCollection;
        private StoreOrder storeOrderData;
        public StoreOrder StoreOrderData
        {
            get { return storeOrderData; }
            set
            {
                storeOrderData = value;
                NotifyPropertyChanged("StoreOrderData");
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
        public ProductPurchaseRecordView()
        {
            InitializeComponent();
            
            UpdateUi();
            DataContext = this;
        }
        public void UpdateUi() {
            storeOrderCollection = StoreOrderDb.GetStoreOrderOverview(Class.OrderType.DONE);
            StoOrderOverview.ItemsSource = storeOrderCollection;
            StoOrderOverview.SelectedIndex = 0;
        }

        private void ShowOrderDetail(object sender, RoutedEventArgs e)
        {
            StoreOrder storeOrder = (StoreOrder)(sender as DataGridCell).DataContext;
           // UpdateOrderDetailUi(storeOrder.Type);
            UpdateOrderDetailData(storeOrder);
        }
        private void UpdateOrderDetailData(StoreOrder storeOrder)
        {
            StoreOrderData = storeOrder;
            //Id = storeOrder.Id;

            //ID.Content = Id;
            //IsFirst = true;
            //ID.Content = storeOrder.Id;
            //PurchaseEmp.Content = storeOrder.OrdEmp;
            //OrderCategory.Text = storeOrder.Category;
            //Total.Content = storeOrder.TotalPrice;
            //ManufactoryAuto.Text = (storeOrder.Manufactory.Name is null) ? "" : storeOrder.Manufactory.Name;
            //ButtonNewProduct.IsEnabled = (storeOrder.Manufactory.Name is null) ? false : true;
            //Phone.Content = (storeOrder.Manufactory.Telphone is null) ? "" : storeOrder.Manufactory.Telphone;

            if (StoreOrderData.Products is null)
                StoreOrderData.Products = StoreOrderDb.GetStoreOrderCollectionById(StoreOrderData.Id);
           

            StoreOrderDetail.ItemsSource = StoreOrderData.Products;
            TotalAmount.Content = StoreOrderData.Products.Count.ToString();
            //IsChanged = false;
            //IsFirst = false;
        }

        private void ID_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
