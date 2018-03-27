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
using His_Pos.Class.StoreOrder;

namespace His_Pos.ProductPurchase
{
    /// <summary>
    /// ProductPurchaseView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseView : UserControl
    {
        private ObservableCollection<StoreOrderOverview> storeOrderOverviewCollection;

        public ProductPurchaseView()
        {
            InitializeComponent();

            //storeOrderOverviewCollection = ;
            StoOrderOverview.ItemsSource = storeOrderOverviewCollection;
        }
    }
}
