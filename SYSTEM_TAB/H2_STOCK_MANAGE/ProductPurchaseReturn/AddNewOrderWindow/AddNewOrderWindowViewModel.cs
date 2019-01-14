using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.StoreOrder;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.AddNewOrderWindow
{
    public class AddNewOrderWindowViewModel : ViewModelBase
    {
        #region ----- Define Command -----
        public RelayCommand ToPurchaseCommand { get; set; }
        public RelayCommand ToReturnCommand { get; set; }
        public RelayCommand ConfirmAddCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private OrderTypeEnum orderType = OrderTypeEnum.PURCHASE;

        public StoreOrder NewStoreOrder { get; set; }
        public OrderTypeEnum OrderType
        {
            get { return orderType; }
            set { Set(() => OrderType, ref orderType, value); }
        }
        public Manufactory OrderManufactory { get; set; }
        public Manufactories ManufactoryCollection { get; set; }
        public StoreOrders DonePurchaseOrders { get; set; }
        #endregion

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
            NewStoreOrder = StoreOrderDB.AddNewStoreOrder(OrderType, OrderManufactory, MainWindow.CurrentUser.Id);
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseAddNewOrderWindow"));
        }
        #endregion

        #region ----- Define Functions -----
        private void InitVariables()
        {
            MainWindow.ServerConnection.OpenConnection();
            ManufactoryCollection = new Manufactories(ManufactoryDB.GetAllManufactories());
            //DonePurchaseOrders = StoreOrderDB.GetDonePurchaseOrdersInOneWeek();
            MainWindow.ServerConnection.CloseConnection();
        }
        #endregion
    }
}
