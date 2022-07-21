using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StoreOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using DomainModel.Enum;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Media;

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

        #endregion ----- Define ViewModel -----

        #region ----- Define Command -----

        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand<string> ChangeUiTypeCommand { get; set; }

        #endregion ----- Define Command -----

        #region ----- Define Variables -----

        private bool isBusy;
        private string busyContent;
        private BackgroundWorker initBackgroundWorker;
        private StoreOrder currentStoreOrder;
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

        #endregion ----- Define Variables -----

        public ProductPurchaseReturnViewModel()
        {
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
                    InitVariables();
                    //NormalViewModel.InitData(StoreOrders.GetOrdersNotDone());
                    break;

                case "SINGDE":
                    UiType = OrderUITypeEnum.SINGDE;
                    SingdeTotalViewModel.InitData();
                    break;
            }
            MainWindow.ServerConnection.CloseConnection();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void InitBackgroundWorker()
        {
            initBackgroundWorker = new BackgroundWorker();

            initBackgroundWorker.DoWork += (sender, args) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                MainWindow.SingdeConnection.OpenConnection();

                BusyContent = "取得訂單資料...";
                storeOrderCollection = StoreOrders.GetOrdersNotDone();

                if (MainWindow.SingdeConnection.ConnectionStatus() == ConnectionState.Open)
                {
                    List<StoreOrder> storeOrders = storeOrderCollection.Where(s => s.OrderStatus == OrderStatusEnum.WAITING || s.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING || s.OrderStatus == OrderStatusEnum.SCRAP).OrderBy(_ => _.IsWaitOrder).ThenBy(_ => _.ID.Substring(1, 11)).ToList();
                    string dateTime = DateTime.Now.ToString("yyyyMMdd");
                    if (storeOrders.Count > 0)
                        dateTime = storeOrders.Min(w=>w.CreateDateTime).ToString("yyyyMMdd");

                    BusyContent = "取得杏德訂單最新狀態...";
                    #region 組合字串，取得杏德最新狀態
                    string orderIDs = string.Empty;
                    for (int i = 0; i < storeOrders.Count; i++)
                    {
                        if (storeOrders[i].OrderStatus != OrderStatusEnum.SCRAP)
                        {
                            string orderID = string.IsNullOrEmpty(storeOrders[i].SourceID) ? storeOrders[i].ID : storeOrders[i].SourceID;
                            if (i == storeOrders.Count - 1)
                            {
                                orderIDs += string.Format("'{0}'", orderID);
                            }
                            else
                            {
                                orderIDs += string.Format("'{0}',", orderID);
                            }
                        }
                    }
                    DataTable dataTable = StoreOrderDB.GetSingdeOrderNewStatusByNo(dateTime, orderIDs);
                    #endregion
                    for (int i = 0; i < storeOrders.Count; i++)
                    {
                        if (storeOrders[i].OrderStatus != OrderStatusEnum.SCRAP)
                        {
                            DataRow[] drs = dataTable.Select(string.Format("ORDER_ID = '{0}'", string.IsNullOrEmpty(storeOrders[i].SourceID) ? storeOrders[i].ID : storeOrders[i].SourceID));
                            if (drs != null && drs.Length > 0)
                            {
                                int flag = Convert.ToInt32(drs[0]["FLAG"]);
                                if(flag == 2)
                                {
                                    string VoidReason = "5.其他:杏德訂單已作廢";
                                    StoreOrderDB.RemoveStoreOrderByID(storeOrders[i].ID, VoidReason);
                                    storeOrders[i].OrderStatus = OrderStatusEnum.SCRAP;
                                    storeOrders[i].IsEnable = false;
                                }
                                else
                                {
                                    if (storeOrders[i].ID == storeOrders[i].ReceiveID)//尚未更新為杏德單號
                                    {
                                        storeOrders[i].OrderType = OrderTypeEnum.PREPARE;//已出貨
                                        storeOrders[i].IsWaitOrder = 0;
                                    }
                                    else if (storeOrders[i].ID != storeOrders[i].ReceiveID)//已更新為杏德單號
                                    {
                                        storeOrders[i].OrderType = OrderTypeEnum.WAITPREPARE;//待入庫
                                        storeOrders[i].IsWaitOrder = 0;
                                    }
                                    currentStoreOrder = storeOrders[i];
                                    currentStoreOrder.UpdateOrderDataFromSingde(drs[0]);
                                }
                            }
                        }
                    }

                    var orderedList = storeOrderCollection.OrderBy(_ => _.IsWaitOrder).ThenBy(_ => _.ID.Substring(1, 11)).ToList();

                    StoreOrders result = new StoreOrders();
                    List<StoreOrder> tempOrder = new List<StoreOrder>();
                    for (int i = 0; i < orderedList.Count(); i++)
                    {
                        result.Add(orderedList[i]);
                    }

                    storeOrderCollection = result;
                    //(20220324)
                    SingdeTotalViewModel.InitData();
                    NormalViewModel.InitData(storeOrderCollection);
                    IsBusy = false;
                    //
                }

                MainWindow.SingdeConnection.CloseConnection(); 
                MainWindow.ServerConnection.CloseConnection();
            };

           
        }

        private void InitVariables()
        {
            IsBusy = true;

            if (!initBackgroundWorker.IsBusy)
                initBackgroundWorker.RunWorkerAsync();
        }

        private void RegisterCommand()
        {
            ReloadCommand = new RelayCommand(ReloadAction);
            ChangeUiTypeCommand = new RelayCommand<string>(ChangeUiTypeAction);
        }

        #endregion ----- Define Functions -----
    }
}