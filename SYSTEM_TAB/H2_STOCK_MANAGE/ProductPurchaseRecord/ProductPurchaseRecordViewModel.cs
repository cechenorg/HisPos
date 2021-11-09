using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using His_Pos.NewClass.StoreOrder.ExportOrderRecord;
using His_Pos.Service.ExportService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    public class ProductPurchaseRecordViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----

        public RelayCommand SearchOrderCommand { get; set; }
        public RelayCommand FilterOrderCommand { get; set; }
        public RelayCommand ClearSearchConditionCommand { get; set; }
        public RelayCommand DeleteOrderCommand { get; set; }

        public RelayCommand DeleteOrderReturnCommand { get; set; }
        public RelayCommand<string> ExportOrderDataCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        #region ///// Search Variables /////

        private DateTime? searchStartDate = DateTime.Today;
        private DateTime? searchEndDate = DateTime.Today;
        private string searchOrderID = "";
        private string searchProductID = "";
        private string searchManufactoryID = "";
        private string searchWareName = "";

        public DateTime? SearchStartDate
        {
            get { return searchStartDate; }
            set { Set(() => SearchStartDate, ref searchStartDate, value); }
        }

        public DateTime? SearchEndDate
        {
            get { return searchEndDate; }
            set { Set(() => SearchEndDate, ref searchEndDate, value); }
        }

        public string SearchOrderID
        {
            get { return searchOrderID; }
            set { Set(() => SearchOrderID, ref searchOrderID, value); }
        }

        public string SearchProductID
        {
            get { return searchProductID; }
            set { Set(() => SearchProductID, ref searchProductID, value); }
        }

        public string SearchManufactoryID
        {
            get { return searchManufactoryID; }
            set { Set(() => SearchManufactoryID, ref searchManufactoryID, value); }
        }

        public string SearchWareName
        {
            get { return searchWareName; }
            set { Set(() => SearchWareName, ref searchWareName, value); }
        }

        #endregion ///// Search Variables /////

        private StoreOrder currentStoreOrder;
        private StoreOrders storeOrderCollection;
        private double totalPrice;
        private bool isBusy;
        private string busyContent;

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

        public StoreOrders StoreOrderCollection
        {
            get { return storeOrderCollection; }
            set { Set(() => StoreOrderCollection, ref storeOrderCollection, value); }
        }

        public StoreOrder CurrentStoreOrder
        {
            get { return currentStoreOrder; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetOrderProducts();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentStoreOrder, ref currentStoreOrder, value);
            }
        }

        public double TotalPrice
        {
            get { return totalPrice; }
            set { Set(() => TotalPrice, ref totalPrice, value); }
        }

        #endregion ----- Define Variables -----

        public ProductPurchaseRecordViewModel()
        {
            TabName = MainWindow.HisFeatures[3].Functions[3];
            Icon = MainWindow.HisFeatures[3].Icon;
            RegisterCommands();
            RegisterMessengers();
        }

        #region ----- Define Actions -----

        private void SearchOrderAction()
        {
            if (!IsSearchConditionValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            StoreOrderCollection = StoreOrders.GetOrdersDone(SearchStartDate, SearchEndDate, SearchOrderID, SearchManufactoryID, SearchProductID, SearchWareName);
            MainWindow.ServerConnection.CloseConnection();

            if (StoreOrderCollection.Count > 0)
            {
                CurrentStoreOrder = StoreOrderCollection[0];

                double purchaseSum = StoreOrderCollection.Where(s => s.OrderStatus != OrderStatusEnum.SCRAP && s.OrderType == OrderTypeEnum.PURCHASE).Sum(s => s.TotalPrice);
                double returnSum = StoreOrderCollection.Where(s => s.OrderStatus != OrderStatusEnum.SCRAP && s.OrderType == OrderTypeEnum.RETURN).Sum(s => s.TotalPrice);

                TotalPrice = purchaseSum - returnSum;
            }
            else
            {
                TotalPrice = 0;
                MessageWindow.ShowMessage("無符合條件項目", MessageType.ERROR);
            }
        }

        private void FilterOrderAction()
        {
        }

        private void ClearSearchConditionAction()
        {
            SearchStartDate = null;
            SearchEndDate = null;
            SearchOrderID = "";
            SearchManufactoryID = "";
            SearchProductID = "";
        }

        private void DeleteOrderAction()
        {
            DeleteOrderWindow deleteOrderWindow = new DeleteOrderWindow(CurrentStoreOrder.ID, CurrentStoreOrder.ReceiveID);
            deleteOrderWindow.ShowDialog();
        }

        private void ExportOrderDataAction(string type)
        {
            IsBusy = true;
            BusyContent = "匯出資料";

            bool isSuccess = false;

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (sender, args) =>
            {
                Collection<object> tempCollection;

                switch (type)
                {
                    case "S":
                        tempCollection = new Collection<object>() { CurrentStoreOrder };
                        break;

                    case "A":
                        tempCollection = new Collection<object>(StoreOrderCollection.ToArray());
                        break;

                    default:
                        MessageWindow.ShowMessage("資料異常 請稍後再試", MessageType.ERROR);
                        return;
                }

                MainWindow.ServerConnection.OpenConnection();
                ExportExcelService service = new ExportExcelService(tempCollection, new ExportOrderRecordTemplate());
                isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\進退貨紀錄{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
                MainWindow.ServerConnection.CloseConnection();
            };

            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                if (isSuccess)
                    MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
                else
                    MessageWindow.ShowMessage("匯出失敗 請稍後再試", MessageType.ERROR);

                IsBusy = false;
            };

            backgroundWorker.RunWorkerAsync();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            SearchOrderCommand = new RelayCommand(SearchOrderAction);
            FilterOrderCommand = new RelayCommand(FilterOrderAction);
            ClearSearchConditionCommand = new RelayCommand(ClearSearchConditionAction);
            DeleteOrderCommand = new RelayCommand(DeleteOrderAction);

            //DeleteOrderReturnCommand = new RelayCommand(DeleteOrderReturnAction);
            DeleteOrderReturnCommand = new RelayCommand(DeleteOrderAction);

            ExportOrderDataCommand = new RelayCommand<string>(ExportOrderDataAction);
        }

        private void DeleteOrderReturnAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否進行刪除?", "確認");
            if (!(bool)confirmWindow.DialogResult)
                return;

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", CurrentStoreOrder.ID));
 
            DataTable iii =MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStoreOrderToScrap]", parameters);
            if (iii.Rows.Count >= 1) 
            {
                MessageWindow.ShowMessage("刪除失敗!請勿重複刪除!", MessageType.ERROR);
            }
            MainWindow.ServerConnection.CloseConnection();
            MessageWindow.ShowMessage("刪除成功!", MessageType.SUCCESS);
            SearchOrderAction();
        }

        private void RegisterMessengers()
        {
            Messenger.Default.Register<NotificationMessage<string>>(this, ShowOrderDetailByOrderID);
        }

        private void ShowOrderDetailByOrderID(NotificationMessage<string> notificationMessage)
        {
            if (notificationMessage.Target == this)
            {
                MainWindow.Instance.AddNewTab(TabName);
                ClearSearchConditionAction();
                SearchOrderID = notificationMessage.Content;
                SearchOrderAction();
            }
        }

        private bool IsSearchConditionValid()
        {
            if ((SearchStartDate is null && SearchEndDate != null) || (SearchStartDate != null && SearchEndDate is null))
            {
                MessageWindow.ShowMessage("日期未填寫完整!", MessageType.ERROR);
                return false;
            }

            if (SearchEndDate < SearchStartDate)
            {
                MessageWindow.ShowMessage("起始日期大於終結日期!", MessageType.ERROR);
                return false;
            }

            if (SearchStartDate is null && SearchEndDate is null && SearchProductID == "" &&
                SearchManufactoryID == "" && SearchOrderID == "")
            {
                MessageWindow.ShowMessage("必須輸入至少一種查詢條件!", MessageType.ERROR);
                return false;
            }

            return true;
        }

        #endregion ----- Define Functions -----
    }
}