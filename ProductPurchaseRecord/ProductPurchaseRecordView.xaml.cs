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
    public partial class ProductPurchaseRecordView : UserControl
    {
        public ObservableCollection<StoreOrder> storeOrderCollection;
        public StoreOrder storeOrderData;
        
      
        public ProductPurchaseRecordView()
        {
            InitializeComponent();
            
            UpdateUi();
        }
        public void UpdateUi() {
            storeOrderCollection = StoreOrderDb.GetStoreOrderOverview("D");
            StoOrderOverview.ItemsSource = storeOrderCollection;

        }

        private void ShowOrderDetail(object sender, RoutedEventArgs e)
        {
            StoreOrder storeOrder = (StoreOrder)(sender as DataGridCell).DataContext;
           // UpdateOrderDetailUi(storeOrder.Type);
            UpdateOrderDetailData(storeOrder);
        }
        private void UpdateOrderDetailData(StoreOrder storeOrder)
        {
            storeOrderData = storeOrder;
           
            //ID.Content = Id;
            //IsFirst = true;
            //ID.Content = storeOrder.Id;

            //PurchaseEmp.Content = storeOrder.OrdEmp;
            //OrderCategory.Text = storeOrder.Category;
            //Total.Content = storeOrder.TotalPrice;
            //ManufactoryAuto.Text = (storeOrder.Manufactory.Name is null) ? "" : storeOrder.Manufactory.Name;
            //ButtonNewProduct.IsEnabled = (storeOrder.Manufactory.Name is null) ? false : true;
            //Phone.Content = (storeOrder.Manufactory.Telphone is null) ? "" : storeOrder.Manufactory.Telphone;

            //if (storeOrder.Products is null)
            //    storeOrder.Products = StoreOrderDb.GetStoreOrderCollectionById(storeOrder.Id);


            //StoreOrderDetail.ItemsSource = storeOrderData.Products;
            //TotalAmount.Content = storeOrder.Products.Count.ToString();
            //IsChanged = false;
            //IsFirst = false;
        }

        private void ID_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
