using System;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;

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
        #endregion

        #region ----- Define Variables -----

        #region ///// Search Variables /////
        private DateTime? searchStartDate = DateTime.Today;
        private DateTime? searchEndDate = DateTime.Today;
        private string searchOrderID = "";
        private string searchProductID = "";
        private string searchManufactoryID = "";

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
        #endregion

        private StoreOrder currentStoreOrder;
        private StoreOrders storeOrderCollection;
        private double totalPrice;
        
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
        #endregion

        public ProductPurchaseRecordViewModel()
        {
            TabName = MainWindow.HisFeatures[1].Functions[2];
            Icon = MainWindow.HisFeatures[1].Icon;
            RegisterCommands();
            RegisterMessengers();
        }

        #region ----- Define Actions -----
        private void SearchOrderAction()
        {
            if (!IsSearchConditionValid()) return;

            StoreOrderCollection = StoreOrders.GetOrdersDone(SearchStartDate, SearchEndDate, SearchOrderID, SearchManufactoryID, SearchProductID);

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
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            SearchOrderCommand = new RelayCommand(SearchOrderAction);
            FilterOrderCommand = new RelayCommand(FilterOrderAction);
            ClearSearchConditionCommand = new RelayCommand(ClearSearchConditionAction);
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
        #endregion
    }
}
