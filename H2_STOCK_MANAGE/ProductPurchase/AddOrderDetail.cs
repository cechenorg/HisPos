using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;

namespace His_Pos.ProductPurchase
{
    public partial class ProductPurchaseView : UserControl
    {
        private void AddGoodSales(Manufactory manufactory = null)
        {
            throw new NotImplementedException();
        }

        private void AddBasicOrSafe(StoreOrderProductType type,Manufactory manufactory = null)
        {
            LoadingWindow loadingWindow = new LoadingWindow();

            loadingWindow.AddNewOrders(this, type, manufactory);
            loadingWindow.ShowDialog();

            StoOrderOverview.SelectedIndex = 0;

            SetChanged();
        }

        private void AddNewOrderByUm(Manufactory manufactory = null)
        {
            StoreOrderCollection.Insert(0, new StoreOrder(MainWindow.CurrentUser, manufactory));
            StoOrderOverview.SelectedIndex = 0;

            SetChanged();
        }
    }
}
