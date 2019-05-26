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
        public RelayCommand<string> ChangeSearchTypeCommand { get; set; }
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
        private double negtiveStockValue;
        private ProductSearchTypeEnum searchType = ProductSearchTypeEnum.ALL;
        private ProductSearchTypeEnum searchConditionType = ProductSearchTypeEnum.ALL;

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
        public double NegtiveStockValue
        {
            get { return negtiveStockValue; }
            set { Set(() => NegtiveStockValue, ref negtiveStockValue, value); }
        }
        public ProductSearchTypeEnum SearchType
        {
            get { return searchType; }
            set { Set(() => SearchType, ref searchType, value); }
        }
        public ProductSearchTypeEnum SearchConditionType
        {
            get { return searchConditionType; }
            set { Set(() => SearchConditionType, ref searchConditionType, value); }
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
            NegtiveStockValue = dataTable.Rows[0].Field<double>("NEG_TOTALSTOCK");

            if (SearchProductCollection.Count == 0)
                MessageWindow.ShowMessage("無符合條件之品項!", MessageType.ERROR);
        }
        private void ChangeSearchTypeAction(string type)
        {
            switch (type)
            {
                case "A":
                    SearchType = ProductSearchTypeEnum.ALL;
                    break;
                case "O":
                    SearchType = ProductSearchTypeEnum.OTC;
                    break;
                case "M":
                    SearchType = ProductSearchTypeEnum.Medicine;
                    break;
            }
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            SearchCommand = new RelayCommand(SearchAction);
            ChangeSearchTypeCommand = new RelayCommand<string>(ChangeSearchTypeAction);
        }
        private bool IsSearchConditionValid()
        {
            return true;
        }
        #endregion
    }
}
