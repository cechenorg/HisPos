using System;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
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
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
        public string SearchOrderID { get; set; }
        public string SearchID { get; set; }
        #endregion
        
        private StoreOrder currentStoreOrder;
        private StoreOrders storeOrderCollection;
        
        public StoreOrders StoreOrderCollection
        {
            get { return storeOrderCollection; }
            set { Set(() => StoreOrderCollection, ref storeOrderCollection, value); }
        }
        public StoreOrder CurrentStoreOrder
        {
            get { return currentStoreOrder; }
            set { Set(() => CurrentStoreOrder, ref currentStoreOrder, value); }
        }
        #endregion

        #region ----- Define Actions -----
        #endregion

        #region ----- Define Functions -----
        #endregion
    }
}
