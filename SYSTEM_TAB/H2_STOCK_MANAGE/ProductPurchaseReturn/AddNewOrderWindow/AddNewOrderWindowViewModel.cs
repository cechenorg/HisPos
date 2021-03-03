﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.StoreOrder;
using His_Pos.NewClass.WareHouse;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.AddNewOrderWindow
{
    public class AddNewOrderWindowViewModel : ViewModelBase
    {
        #region ----- Define Command -----

        public RelayCommand ToPurchaseCommand { get; set; }
        public RelayCommand ToReturnCommand { get; set; }
        public RelayCommand ConfirmAddCommand { get; set; }

        #endregion ----- Define Command -----

        #region ----- Define Variables -----

        private OrderTypeEnum orderType = OrderTypeEnum.PURCHASE;
        private Manufactory purchaseOrderManufactory;
        private Manufactory returnOrderManufactory;
        private WareHouse selectedWareHouse;

        public StoreOrder NewStoreOrder { get; set; }

        public OrderTypeEnum OrderType
        {
            get { return orderType; }
            set { Set(() => OrderType, ref orderType, value); }
        }

        public Manufactory PurchaseOrderManufactory
        {
            get { return purchaseOrderManufactory; }
            set { Set(() => PurchaseOrderManufactory, ref purchaseOrderManufactory, value); }
        }

        public Manufactory ReturnOrderManufactory
        {
            get { return returnOrderManufactory; }
            set { Set(() => ReturnOrderManufactory, ref returnOrderManufactory, value); }
        }

        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set { Set(() => SelectedWareHouse, ref selectedWareHouse, value); }
        }

        public Manufactories ManufactoryCollection { get; set; }
        public WareHouses WareHouseCollection { get; set; }
        public StoreOrders DonePurchaseOrders { get; set; }

        #endregion ----- Define Variables -----

        public AddNewOrderWindowViewModel()
        {
            ToPurchaseCommand = new RelayCommand(ToPurchaseAction);
            ToReturnCommand = new RelayCommand(ToReturnAction);
            ConfirmAddCommand = new RelayCommand(ConfirmAddAction);

            InitVariables();
        }

        #region ----- Define Actions -----

        private void ToPurchaseAction()
        {
            OrderType = OrderTypeEnum.PURCHASE;
        }

        private void ToReturnAction()
        {
            OrderType = OrderTypeEnum.RETURN;
        }

        private void ConfirmAddAction()
        {
            if (!CheckInputValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            NewStoreOrder = StoreOrder.AddNewStoreOrder(OrderType, (OrderType == OrderTypeEnum.PURCHASE) ? PurchaseOrderManufactory : ReturnOrderManufactory, ViewModelMainWindow.CurrentUser.ID, int.Parse(SelectedWareHouse.ID));
            MainWindow.ServerConnection.CloseConnection();

            Messenger.Default.Send(new NotificationMessage("CloseAddNewOrderWindow"));
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void InitVariables()
        {
            MainWindow.ServerConnection.OpenConnection();
            ManufactoryCollection = Manufactories.GetManufactories();
            WareHouseCollection = WareHouses.GetWareHouses();
            //DonePurchaseOrders = new StoreOrders(StoreOrderDB.GetDonePurchaseOrdersInOneWeek());
            MainWindow.ServerConnection.CloseConnection();

            PurchaseOrderManufactory = ManufactoryCollection[0];
            ReturnOrderManufactory = ManufactoryCollection[0];

            SelectedWareHouse = WareHouseCollection[0];
        }

        private bool CheckInputValid()
        {
            bool isValid = false;

            switch (OrderType)
            {
                case OrderTypeEnum.PURCHASE:
                    isValid = CheckPurchaseValid();
                    break;

                case OrderTypeEnum.RETURN:
                    isValid = CheckReturnValid();
                    break;
            }

            return isValid;
        }

        private bool CheckReturnValid()
        {
            if (ReturnOrderManufactory is null)
            {
                MessageWindow.ShowMessage("請選擇有效供應商", MessageType.ERROR);
                return false;
            }

            return true;
        }

        private bool CheckPurchaseValid()
        {
            if (PurchaseOrderManufactory is null)
            {
                MessageWindow.ShowMessage("請選擇有效供應商", MessageType.ERROR);
                return false;
            }

            return true;
        }

        #endregion ----- Define Functions -----
    }
}