using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
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
        private Manufactory purchaseOrderManufactory;
        private Manufactory returnOrderManufactory;

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
            MainWindow.ServerConnection.OpenConnection();
            NewStoreOrder = StoreOrder.AddNewStoreOrder(OrderType, (OrderType == OrderTypeEnum.PURCHASE) ? PurchaseOrderManufactory : ReturnOrderManufactory, ViewModelMainWindow.CurrentUser.ID);
            MainWindow.ServerConnection.CloseConnection();

            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseAddNewOrderWindow"));
        }
        #endregion

        #region ----- Define Functions -----
        private void InitVariables()
        {
            MainWindow.ServerConnection.OpenConnection();
            ManufactoryCollection = Manufactories.GetManufactories();
            //DonePurchaseOrders = new StoreOrders(StoreOrderDB.GetDonePurchaseOrdersInOneWeek());
            MainWindow.ServerConnection.CloseConnection();

            PurchaseOrderManufactory = ManufactoryCollection[0];
            ReturnOrderManufactory = ManufactoryCollection[0];
        }
        #endregion
    }
}
