﻿using System.Windows.Controls;
using His_Pos.Class;
using His_Pos.Class.Manufactory;
using His_Pos.Class.StoreOrder;
using His_Pos.FunctionWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchase
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
            StoreOrder newStoreOrder = StoreOrderDb.AddReturnOrderByPurchace(OrderId);

            StoreOrderCollection.Insert(0, newStoreOrder);
            StoOrderOverview.SelectedIndex = 0;
        }

    }
}