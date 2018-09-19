using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.Product;
using His_Pos.Class.StoreOrder;
using His_Pos.H1_DECLARE.PrescriptionDec2;

namespace His_Pos.ProductPurchase
{
    public partial class ProductPurchaseView : UserControl
    {
        private void AddGoodSales(Manufactory manufactory = null)
        {
        }

        private void AddBasicOrSafe(StoreOrderProductType type,WareHouse wareHouse, Manufactory manufactory = null)
        {
            LoadingWindow loadingWindow = new LoadingWindow();

            loadingWindow.AddNewOrders(this, type, wareHouse, manufactory);
            loadingWindow.ShowDialog();

            StoOrderOverview.SelectedIndex = 0;

            //SetChanged();
        }

        private void AddNewOrderByUm(WareHouse wareHouse, Manufactory manufactory = null)
        {
            StoreOrderCollection.Insert(0, new StoreOrder(StoreOrderCategory.PURCHASE, MainWindow.CurrentUser, wareHouse, manufactory));
            StoOrderOverview.SelectedIndex = 0;

            //SetChanged();
        }

        private void AddReturn(WareHouse wareHouse, Manufactory manufactory)
        {
            StoreOrderCollection.Insert(0, new StoreOrder(StoreOrderCategory.RETURN, MainWindow.CurrentUser, wareHouse, manufactory));
            StoOrderOverview.SelectedIndex = 0;
        }

        private void AddReturnByOrder(string OrderId)
        {

        }

        public void AddOrderByPrescription(Prescription prescription, ObservableCollection<ChronicSendToServerWindow.PrescriptionSendData> declareMedicines)
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>();

            int newIndex = storeOrderCollection.Count - 1;

            for (int x = 0; x < storeOrderCollection.Count; x++)
            {
                if (storeOrderCollection[x].type == OrderType.PROCESSING)
                {
                    newIndex = x - 1;
                    break;
                }
            }

            foreach(var med in declareMedicines)
            {
                products.Add(new ProductPurchaseMedicine(ProductCollection.Single(p => p.Id.Equals(med.MedId) && p.WarId.Equals("0"))));
            }

            Manufactory manufactory = ManufactoryAutoCompleteCollection.Single(m => m.Id.Equals("0"));

            StoreOrder storeOrder = new StoreOrder(StoreOrderCategory.PURCHASE, MainWindow.CurrentUser, new WareHouse() { Id = "0" }, manufactory, products);

            storeOrder.Type = OrderType.WAITING;
            storeOrder.DeclareDataCount = 1;

            //對應關係存到DB
            //StoreOrderDb.SaveOrderDeclareData(storeOrder.Id, prescription.);

            //送到sinde
            //StoreOrderDb.SendDeclareOrderToSinde(StoreOrderData);

            SaveOrder();

            UpdateOneTheWayAmount();

            storeOrderCollection.Insert(newIndex, StoreOrderData);
            StoOrderOverview.SelectedItem = StoreOrderData;
            StoOrderOverview.ScrollIntoView(StoreOrderData);

            SetCurrentControl();
        }
    }
}
