using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class.StoreOrder;
using His_Pos.NewClass.Product.ProductDaliyPurchase;
using His_Pos.NewClass.StoreOrder;

namespace His_Pos.SYSTEM_TAB.INDEX
{
    class Index : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        #region Var
        private ProductDailyPurchases productDailyPurchaseCollection = new ProductDailyPurchases();
        public ProductDailyPurchases ProductDailyPurchaseCollection {
            get { return productDailyPurchaseCollection; }
            set { Set(() => ProductDailyPurchaseCollection, ref productDailyPurchaseCollection, value); }
        } 
        #endregion
        #region Command
        public RelayCommand DailyPurchaseCommand { get; set; }
        public RelayCommand DailyPurchaseReloadCommand { get; set; }
        #endregion
        public Index() {
            ProductDailyPurchaseCollection.GetDailyPurchaseData();
            DailyPurchaseCommand = new RelayCommand(DailyPurchaseAction);
            DailyPurchaseReloadCommand = new RelayCommand(DailyPurchaseReloadAction);
        }
        #region Action
        private void DailyPurchaseAction() {
            StoreOrderDB.DailyProductsPurchase();
        }
        private void DailyPurchaseReloadAction() {
            ProductDailyPurchaseCollection.Clear();
            ProductDailyPurchaseCollection.GetDailyPurchaseData();
        }
        
        #endregion
    }
}
