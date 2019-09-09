using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
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
        public RelayCommand<string> FilterCommand { get; set; }
        #endregion

        #region ----- Define Variables -----

        #region ///// Search Condition /////
        public string SearchID { get; set; } = "";
        public string SearchName { get; set; } = "";
        public bool SearchIsEnable { get; set; }
        public bool SearchIsInventoryZero { get; set; }
        #endregion

        private ProductManageStructs searchProductCollection;
        private bool isBusy;
        private string busyContent;
        private double totalStockValue;
        private double medBagStockValue;
        private double shelfStockValue;
        private double errorStockValue;
        private WareHouse selectedWareHouse;
        private ICollectionView productCollectionView;
        private ProductManageFilterEnum filterType = ProductManageFilterEnum.ALL;
        private ProductSearchTypeEnum searchType = ProductSearchTypeEnum.ALL;
        private ProductSearchTypeEnum searchConditionType = ProductSearchTypeEnum.ALL;

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
        public double MedBagStockValue
        {
            get { return medBagStockValue; }
            set { Set(() => MedBagStockValue, ref medBagStockValue, value); }
        }
        public double ShelfStockValue
        {
            get { return shelfStockValue; }
            set { Set(() => ShelfStockValue, ref shelfStockValue, value); }
        }
        public double ErrorStockValue
        {
            get { return errorStockValue; }
            set { Set(() => ErrorStockValue, ref errorStockValue, value); }
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
        public ICollectionView ProductCollectionView
        {
            get => productCollectionView;
            set { Set(() => ProductCollectionView, ref productCollectionView, value); }
        }
        public WareHouses WareHouseCollection { get; set; }
        public bool HasError => ((int) ErrorStockValue).Equals(0);
        public double CurrentStockValue => (ProductCollectionView is null) ? 0 : ProductCollectionView.OfType<ProductManageStruct>().Sum(p => p.StockValue);
        public double CurrentShelfStockValue => (ProductCollectionView is null) ? 0 : ProductCollectionView.OfType<ProductManageStruct>().Sum(p => p.ShelfStockValue);
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

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (sender, args) =>
            {
                IsBusy = true;
                BusyContent = "查詢商品資料中";

                MainWindow.ServerConnection.OpenConnection();
                SearchProductCollection = ProductManageStructs.SearchProductByConditions(SearchID.Trim(), SearchName.Trim(), SearchIsEnable, SearchIsInventoryZero, SelectedWareHouse.ID);
                DataTable dataTable = ProductDetailDB.GetTotalStockValue(SelectedWareHouse.ID);
                MainWindow.ServerConnection.CloseConnection();

                ProductCollectionView = CollectionViewSource.GetDefaultView(SearchProductCollection);
                ProductCollectionView.Filter += ProductFilter;
                RaisePropertyChanged(nameof(CurrentStockValue));
                RaisePropertyChanged(nameof(CurrentShelfStockValue));

                TotalStockValue = dataTable.Rows[0].Field<double>("TOTALSTOCK");
                ShelfStockValue = dataTable.Rows[0].Field<double>("SHELF_STOCK");
                MedBagStockValue = dataTable.Rows[0].Field<double>("MEDBAG_STOCK");

                ErrorStockValue = TotalStockValue - ShelfStockValue - MedBagStockValue;
                RaisePropertyChanged(nameof(HasError));
            };

            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                if (SearchProductCollection.Count == 0)
                    MessageWindow.ShowMessage("無符合條件之品項!", MessageType.ERROR);

                SearchType = SearchConditionType;

                IsBusy = false;
            };

            backgroundWorker.RunWorkerAsync();
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
        private void FilterAction(string type)
        {
            filterType = (ProductManageFilterEnum) int.Parse(type);

            ProductCollectionView.Filter += ProductFilter;
            RaisePropertyChanged(nameof(CurrentStockValue));
            RaisePropertyChanged(nameof(CurrentShelfStockValue));
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            SearchCommand = new RelayCommand(SearchAction);
            ChangeSearchTypeCommand = new RelayCommand<string>(ChangeSearchTypeAction);
            InsertProductCommand = new RelayCommand(InsertProductAction);
            FilterCommand = new RelayCommand<string>(FilterAction);
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
        private bool ProductFilter(object product)
        {
            var tempProduct = product as ProductManageStruct;

            switch (filterType)
            {
                case ProductManageFilterEnum.ALL:
                    return true;
                case ProductManageFilterEnum.COMMON:
                    return tempProduct.IsCommon;
                case ProductManageFilterEnum.CONTROL:
                    return tempProduct.ControlLevel != null;
                case ProductManageFilterEnum.FROZE:
                    return tempProduct.IsFrozen;
                case ProductManageFilterEnum.DISABLE:
                    return !tempProduct.IsEnable; 
                case ProductManageFilterEnum.INV_ERROR:
                    return tempProduct.InventoryError;
            }

            return false;
        }
        #endregion
    }
}
