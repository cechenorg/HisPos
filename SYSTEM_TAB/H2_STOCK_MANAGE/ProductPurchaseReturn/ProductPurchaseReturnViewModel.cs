using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.NewClass.StoreOrder;
using His_Pos.NewClass.StoreOrder.ExportOrderRecord;
using His_Pos.Service.ExportService;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.AddNewOrderWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ProductPurchaseReturnViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define ViewModel -----
        public NormalViewModel NormalViewModel { get; set; }
        public SingdeTotalViewModel SingdeTotalViewModel { get; set; }
        #endregion

        #region ----- Define Command -----
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand<string> ChangeUiTypeCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isBusy;
        private string busyContent;
        private BackgroundWorker initBackgroundWorker;
        private StoreOrders storeOrderCollection;
        private OrderUITypeEnum uiType;

        public bool IsBusy
        {
            get => isBusy;
            set { Set(() => IsBusy, ref isBusy, value); }
        }
        public string BusyContent
        {
            get => busyContent;
            set { Set(() => BusyContent, ref busyContent, value); }
        }
        public OrderUITypeEnum UiType
        {
            get => uiType;
            set { Set(() => UiType, ref uiType, value); }
        }
        #endregion

        public ProductPurchaseReturnViewModel()
        {
            TabName = MainWindow.HisFeatures[2].Functions[2];
            Icon = MainWindow.HisFeatures[2].Icon;

            NormalViewModel = new NormalViewModel();
            SingdeTotalViewModel = new SingdeTotalViewModel();

            InitBackgroundWorker();
            RegisterCommand();
        }

        #region ----- Define Actions -----
        private void ReloadAction()
        {
            InitVariables();
        }
        private void ChangeUiTypeAction(string type)
        {
            MainWindow.ServerConnection.OpenConnection();
            switch (type)
            {
                case "NORMAL":
                    UiType = OrderUITypeEnum.NORMAL;
                    NormalViewModel.InitData(StoreOrders.GetOrdersNotDone());
                    break;
                case "SINGDE":
                    UiType = OrderUITypeEnum.SINGDE;
                    SingdeTotalViewModel.InitData();
                    break;
            }
            MainWindow.ServerConnection.CloseConnection();
        }
        #endregion

        #region ----- Define Functions -----
        private void InitBackgroundWorker()
        {
            initBackgroundWorker = new BackgroundWorker();

            initBackgroundWorker.DoWork += (sender, args) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();

                DataTable dataTable;

                if (MainWindow.SingdeConnection.ConnectionStatus() == ConnectionState.Open)
                {
                    BusyContent = "取得杏德新訂單...";
                    dataTable = StoreOrderDB.GetNewSingdeOrders();
                    if (dataTable.Rows.Count > 0)
                        StoreOrders.AddNewOrdersFromSingde(dataTable);

                    dataTable = StoreOrderDB.GetNewSingdePrescriptionOrders();
                    if (dataTable.Rows.Count > 0)
                        StoreOrders.AddNewPrescriptionOrdersFromSingde(dataTable);
                }

                BusyContent = "取得訂單資料...";
                storeOrderCollection = StoreOrders.GetOrdersNotDone();

                if (MainWindow.SingdeConnection.ConnectionStatus() == ConnectionState.Open)
                {
                    List<StoreOrder> storeOrders = storeOrderCollection.Where(s => s.OrderStatus == OrderStatusEnum.WAITING || s.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING).OrderBy(s => s.CreateDateTime).ToList();
                    string dateTime = DateTime.Now.ToString("yyyyMMdd");
                    string qDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

                    if (storeOrders.Count > 0)
                        dateTime = storeOrders[0].CreateDateTime.ToString("yyyyMMdd");

                    BusyContent = "取得杏德訂單最新狀態...";
                    dataTable = StoreOrderDB.GetSingdeOrderNewStatus(qDate);
                    if (dataTable.Rows.Count > 0)
                    {
                        storeOrderCollection.UpdateSingdeOrderStatus(dataTable);
                        storeOrderCollection = new StoreOrders(storeOrderCollection.Where(s => s.OrderStatus != OrderStatusEnum.SCRAP).ToList());
                    }
                }

                MainWindow.SingdeConnection.CloseConnection();
                MainWindow.ServerConnection.CloseConnection();
            };

            initBackgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                SingdeTotalViewModel.InitData();
                NormalViewModel.InitData(storeOrderCollection);
                IsBusy = false;
            };
        }
        private void InitVariables()
        {
            IsBusy = true;

            if(!initBackgroundWorker.IsBusy)
                initBackgroundWorker.RunWorkerAsync();
        }
        private void RegisterCommand()
        {
            ReloadCommand = new RelayCommand(ReloadAction);
            ChangeUiTypeCommand = new RelayCommand<string>(ChangeUiTypeAction);
        }
        #endregion
    }
}
