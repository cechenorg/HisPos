using System.Data;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement
{
    public class ProductManagementViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Command -----
        public RelayCommand SearchCommand { get; set; }
        #endregion

        #region ----- Define Variables -----

        #region ///// Search Condition /////
        public string SearchID { get; set; } = "";
        public string SearchName { get; set; } = "";
        public bool SearchIsEnable { get; set; }
        public bool SearchIsInventoryZero { get; set; }
        #endregion

        private ProductManageStructs searchProductCollection;
        private double totalStockValue;

        public ProductManageStructs SearchProductCollection
        {
            get { return searchProductCollection; }
            set { Set(() => SearchProductCollection, ref searchProductCollection, value); }
        }
        public double TotalStockValue
        {
            get { return totalStockValue; }
            set { Set(() => TotalStockValue, ref totalStockValue, value); }
        }
        #endregion

        public ProductManagementViewModel()
        {
            RegisterCommand();
            SearchAction();
        }

        #region ----- Define Actions -----
        private void SearchAction()
        {
            if (!IsSearchConditionValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            SearchProductCollection = ProductManageStructs.SearchProductByConditions(SearchID.Trim(), SearchName.Trim(), SearchIsEnable, SearchIsInventoryZero);
            DataTable dataTable = ProductDetailDB.GetTotalStockValue();
            MainWindow.ServerConnection.CloseConnection();

            TotalStockValue = dataTable.Rows[0].Field<double>("TOTALSTOCK");

            if (SearchProductCollection.Count == 0)
                MessageWindow.ShowMessage("無符合條件之品項!", MessageType.ERROR);
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            SearchCommand = new RelayCommand(SearchAction);
        }
        private bool IsSearchConditionValid()
        {
            return true;
        }
        #endregion
    }
}
