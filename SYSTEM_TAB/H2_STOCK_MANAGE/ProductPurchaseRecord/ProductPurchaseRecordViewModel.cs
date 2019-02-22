using System;
using System.Linq;
using GalaSoft.MvvmLight.Command;
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
        #endregion

        #region ----- Define Variables -----

        #region ///// Search Variables /////
        public DateTime? SearchStartDate { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime? SearchEndDate { get; set; } = DateTime.Today;
        public string SearchOrderID { get; set; } = "";
        public string SearchProductID { get; set; } = "";
        public string SearchManufactoryID { get; set; } = "";
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
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void SearchOrderAction()
        {
            if (!IsSearchConditionValid()) return;

            StoreOrderCollection = StoreOrders.GetOrdersDone(SearchStartDate, SearchEndDate, SearchOrderID, SearchManufactoryID, SearchProductID);

            if (StoreOrderCollection.Count > 0)
            {
                CurrentStoreOrder = StoreOrderCollection[0];
                TotalPrice = StoreOrderCollection.Sum(s => s.TotalPrice);
            }
            else
                MessageWindow.ShowMessage("無符合條件項目", MessageType.ERROR);
        }
        private void FilterOrderAction()
        {

        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            SearchOrderCommand = new RelayCommand(SearchOrderAction);
            FilterOrderCommand = new RelayCommand(FilterOrderAction);
        }
        private bool IsSearchConditionValid()
        {
            if (SearchStartDate is null || SearchEndDate is null)
            {
                MessageWindow.ShowMessage("日期未填寫完整!", MessageType.ERROR);
                return false;
            }

            if (SearchEndDate < SearchStartDate)
            {
                MessageWindow.ShowMessage("起始日期大於終結日期!", MessageType.ERROR);
                return false;
            }

            if (((DateTime)SearchStartDate).AddMonths(1) < SearchEndDate)
            {
                MessageWindow.ShowMessage("日期區間不可大於一個月!", MessageType.ERROR);
                return false;
            }

            return true;
        }
        #endregion
    }
}
