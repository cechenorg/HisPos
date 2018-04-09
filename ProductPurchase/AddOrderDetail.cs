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
            ObservableCollection<Manufactory> manufactories = (manufactory is null)
                ? ManufactoryDb.GetManufactoriesBasicSafe(type)
                : new ObservableCollection<Manufactory>() { manufactory };

            foreach (Manufactory man in manufactories)
            {
                StoreOrder newStoreOrder = new StoreOrder(MainWindow.CurrentUser, man, ProductDb.GetBasicOrSafe(man, type));

                storeOrderCollection.Insert(0, newStoreOrder);

                StoreOrderDb.SaveOrderDetail(newStoreOrder);
            }

            StoOrderOverview.SelectedIndex = 0;
        }

        private void AddNewOrderByUm(Manufactory manufactory = null)
        {
            storeOrderCollection.Insert(0, new StoreOrder(MainWindow.CurrentUser, manufactory));
            StoOrderOverview.SelectedIndex = 0;
        }
    }
}
