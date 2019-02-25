using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.StoreOrder;
using His_Pos.FunctionWindow;
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
        public RelayCommand DailyReturnCommand { get; set; }
        public RelayCommand DailyPurchaseReloadCommand { get; set; }
        #endregion
        public Index() {
            ProductDailyPurchaseCollection.GetDailyPurchaseData();
            DailyPurchaseCommand = new RelayCommand(DailyPurchaseAction);
            DailyReturnCommand = new RelayCommand(DailyReturnAction);
            DailyPurchaseReloadCommand = new RelayCommand(DailyPurchaseReloadAction);
        }
        #region Action
        private void DailyPurchaseAction()
        {
            StoreOrderDB.DailyProductsPurchase();
            MessageWindow.ShowMessage("已產生採購單, 確認數量後請自行傳送至杏德", MessageType.SUCCESS);
        }
        private void DailyReturnAction()
        {
            StoreOrderDB.DailyProductsReturn();
            MessageWindow.ShowMessage("已產生退貨單, 確認數量後請自行傳送至杏德", MessageType.SUCCESS);
        }
        private void DailyPurchaseReloadAction() {
            ProductDailyPurchaseCollection.Clear();
            ProductDailyPurchaseCollection.GetDailyPurchaseData();
        }
        
        #endregion
    }
}
