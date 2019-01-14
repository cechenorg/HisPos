﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.AddNewOrderWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ProductPurchaseReturnViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Command -----
        public RelayCommand AddOrderCommand { get; set; }
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand ToNextStatusCommand { get; set; }
        public RelayCommand AllProcessingOrderToDoneCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private StoreOrder currentStoreOrder;

        public StoreOrders StoreOrderCollection { get; set; }
        public StoreOrder CurrentStoreOrder
        {
            get { return currentStoreOrder; }
            set { Set(() => CurrentStoreOrder, ref currentStoreOrder, value); }
        }
        #endregion

        public ProductPurchaseReturnViewModel()
        {
            AddOrderCommand = new RelayCommand(AddOrderAction);
            DeleteOrderCommand = new RelayCommand(DeleteOrderAction);
            ToNextStatusCommand = new RelayCommand(ToNextStatusAction);

            InitVariables();
        }

        #region ----- Define Actions -----
        private void AddOrderAction()
        {
            AddNewOrderWindow.AddNewOrderWindow addNewOrderWindow = new AddNewOrderWindow.AddNewOrderWindow();
            addNewOrderWindow.ShowDialog();

            AddNewOrderWindowViewModel viewModel = addNewOrderWindow.DataContext as AddNewOrderWindowViewModel;

            if (viewModel.NewStoreOrder != null)
            {
                StoreOrderCollection.Insert(0, viewModel.NewStoreOrder);
                CurrentStoreOrder = StoreOrderCollection[0];
            }
        }
        private void DeleteOrderAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = StoreOrderDB.RemoveStoreOrderByID(CurrentStoreOrder.ID);
            MainWindow.ServerConnection.CloseConnection();

            bool isSuccess = Boolean.Parse(dataTable.Rows[0][""].ToString());

            if (isSuccess)
                StoreOrderCollection.Remove(CurrentStoreOrder);
        }
        private void ToNextStatusAction()
        {
            if(CurrentStoreOrder.CheckOrder())
                CurrentStoreOrder.MoveToNextStatus();
        }
        #endregion

        #region ----- Define Functions -----
        private void InitVariables()
        {
            MainWindow.ServerConnection.OpenConnection();
            StoreOrderCollection = new StoreOrders(StoreOrderDB.GetNotDoneStoreOrders());
            MainWindow.ServerConnection.CloseConnection();

            if (StoreOrderCollection.Count > 0)
                CurrentStoreOrder = StoreOrderCollection[0];
        }
        #endregion
    }
}
