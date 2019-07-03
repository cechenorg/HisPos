using System;
using System.Data;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.WareHouse;

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
        public RelayCommand InsertProductCommand { get; set; }
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
        private WareHouse selectedWareHouse;
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
        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set { Set(() => SelectedWareHouse, ref selectedWareHouse, value); }
        }
        public WareHouses WareHouseCollection { get; set; }
        #endregion

        public ProductManagementViewModel()
        {
            InitData();
            RegisterCommand();
            SearchAction();
        }
        
        #region ----- Define Actions -----
        private void SearchAction()
        {
            if (!IsSearchConditionValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            SearchProductCollection = ProductManageStructs.SearchProductByConditions(SearchID.Trim(), SearchName.Trim(), SearchIsEnable, SearchIsInventoryZero, SelectedWareHouse.ID);
            DataTable dataTable = ProductDetailDB.GetTotalStockValue(SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();

            TotalStockValue = dataTable.Rows[0].Field<double>("TOTALSTOCK");

            if (SearchProductCollection.Count == 0)
                MessageWindow.ShowMessage("無符合條件之品項!", MessageType.ERROR);

            SearchType = SearchConditionType;
        }
        private void ChangeSearchTypeAction(string type)
        {
            switch (type)
            {
                case "A":
                    SearchConditionType = ProductSearchTypeEnum.ALL;
                    break;
                case "O":
                    SearchConditionType = ProductSearchTypeEnum.OTC;
                    break;
                case "M":
                    SearchConditionType = ProductSearchTypeEnum.Medicine;
                    break;
            }
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            SearchCommand = new RelayCommand(SearchAction);
            ChangeSearchTypeCommand = new RelayCommand<string>(ChangeSearchTypeAction);
            InsertProductCommand = new RelayCommand(InsertProductAction);
        }
        private void InsertProductAction() {
            InsertProductWindow.InsertProductWindow insertProductWindow = new InsertProductWindow.InsertProductWindow();
        }
        private bool IsSearchConditionValid()
        {
            return true;
        }
        private void InitData()
        {
            MainWindow.ServerConnection.OpenConnection();
            WareHouseCollection = WareHouses.GetWareHouses();
            MainWindow.ServerConnection.CloseConnection();

            if (WareHouseCollection is null || WareHouseCollection.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            SelectedWareHouse = WareHouseCollection[0];
        }
        #endregion
    }
}
