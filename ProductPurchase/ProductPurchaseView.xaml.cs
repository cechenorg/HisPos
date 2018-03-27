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
using His_Pos.Class.StoreOrder;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// ProductPurchaseView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseView : UserControl
    {
        private ObservableCollection<StoreOrderOverview> storeOrderOverviewCollection;
        private ObservableCollection<Product> StoreOrderCollection;

        public ProductPurchaseView()
        {
            InitializeComponent();
            UpdateUi();
        }

        private void UpdateUi()
        {

            storeOrderOverviewCollection = StoreOrderDb.GetStoreOrderOverview();
            StoOrderOverview.ItemsSource = storeOrderOverviewCollection;

            StoOrderOverview.SelectedIndex = 0;
        }

        private void ShowOrderDetail(object sender, RoutedEventArgs e)
        {
            UpdateOrderDetailUi((StoreOrderOverview)(sender as DataGridCell).DataContext);
        }

        private void UpdateOrderDetailUi(StoreOrderOverview storeOrderOverview)
        {
            ID.Content = storeOrderOverview.Id;
            PurchaseEmp.Text = storeOrderOverview.OrdEmp;
            Total.Content = storeOrderOverview.TotalPrice;
            Name.Content = storeOrderOverview.Manufactory.Name;
            Phone.Content = storeOrderOverview.Manufactory.Telphone;

            StoreOrderCollection = StoreOrderDb.GetStoreOrderCollectionById(storeOrderOverview.Id);
        }

        private void AddNewOrder(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
