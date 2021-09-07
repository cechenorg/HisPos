using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.WareHouse;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Data;

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

        public RelayCommand<string> FilterIsOTCCommand { get; set; }
        public RelayCommand<string> IsOTCCommand { get; set; }

        #endregion ----- Define Command -----

        #region ----- Define Variables -----

        #region ///// Search Condition /////

        public string SearchID { get; set; } = "";
        public string SearchName { get; set; } = "";
        public bool SearchIsEnable { get; set; }
        public bool SearchIsInventoryZero { get; set; }
        public bool SearchIsSingdeInventory { get; set; }
        public bool SearchHasOnWay { get; set; }

        #endregion ///// Search Condition /////

        private ProductManageStructs searchProductCollection;
        private bool isBusy;
        private string isOTC;
        private string busyContent;
        private double totalStockValue;
        private double medBagStockValue;
        private double shelfStockValue;
        private double errorStockValue;
        private WareHouse selectedWareHouse;
        private ICollectionView productCollectionView;
        private ProductManageFilterEnum filterType = ProductManageFilterEnum.Medicine;
        private ProductManageFilterEnum filterIsOTC = ProductManageFilterEnum.Medicine;
        private ProductSearchTypeEnum searchType = ProductSearchTypeEnum.ALL;
        private ProductSearchTypeEnum searchConditionType = ProductSearchTypeEnum.ALL;

        public string IsOTC
        {
            get => isOTC;
            set { Set(() => IsOTC, ref isOTC, value); }
        }

        private Visibility mEDDeposit;

        public Visibility MEDDeposit
        {
            get => mEDDeposit;
            set { Set(() => MEDDeposit, ref mEDDeposit, value); }
        }

        private Visibility oTCDeposit;

        public Visibility OTCDeposit
        {
            get => oTCDeposit;
            set { Set(() => OTCDeposit, ref oTCDeposit, value); }
        }

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
        public bool HasError = true;
        public double CurrentStockValue => (ProductCollectionView is null) ? 0 : ProductCollectionView.OfType<ProductManageStruct>().Sum(p => p.StockValue);
        public double CurrentShelfStockValue => (ProductCollectionView is null) ? 0 : ProductCollectionView.OfType<ProductManageStruct>().Sum(p => p.ShelfStockValue);

        private double medShelfStockValue;

        public double MedShelfStockValue
        {
            get { return medShelfStockValue; }
            set { Set(() => MedShelfStockValue, ref medShelfStockValue, value); }
        }

        private double medStockValue;

        public double MedStockValue
        {
            get { return medStockValue; }
            set { Set(() => MedStockValue, ref medStockValue, value); }
        }

        private double oTCShelfStockValue;

        public double OTCShelfStockValue
        {
            get { return oTCShelfStockValue; }
            set { Set(() => OTCShelfStockValue, ref oTCShelfStockValue, value); }
        }

        private double oTCStockValue;

        public double OTCStockValue
        {
            get { return oTCStockValue; }
            set { Set(() => OTCStockValue, ref oTCStockValue, value); }
        }

        #endregion ----- Define Variables -----

        public ProductManagementViewModel()
        {
            InitData();
            RegisterCommand();
            SearchAction();
            MEDDeposit = Visibility.Visible;
            OTCDeposit = Visibility.Collapsed;
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
                SearchProductCollection = ProductManageStructs.SearchProductByConditions(SearchID.Trim(), SearchName.Trim(), SearchIsEnable, SearchIsInventoryZero, SelectedWareHouse.ID, SearchIsSingdeInventory);
                DataTable dataTable = ProductDetailDB.GetTotalStockValue(SelectedWareHouse.ID);
                MainWindow.ServerConnection.CloseConnection();

                ProductCollectionView = CollectionViewSource.GetDefaultView(SearchProductCollection);

                RaisePropertyChanged(nameof(CurrentStockValue));
                RaisePropertyChanged(nameof(CurrentShelfStockValue));

                TotalStockValue = dataTable.Rows[0].Field<double>("TOTALSTOCK");
                ShelfStockValue = dataTable.Rows[0].Field<double>("SHELF_STOCK");
                MedBagStockValue = dataTable.Rows[0].Field<double>("MEDBAG_STOCK");
                ErrorStockValue = dataTable.Rows[0].Field<double>("ERROR_STOCK");

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ProductCollectionView.Filter += MEDFilter;
                }));

                RaisePropertyChanged(nameof(CurrentStockValue));
                RaisePropertyChanged(nameof(CurrentShelfStockValue));
                MedShelfStockValue = CurrentShelfStockValue;
                MedStockValue = CurrentStockValue;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ProductCollectionView.Filter += OTCFilter;
                }));
                RaisePropertyChanged(nameof(CurrentStockValue));
                RaisePropertyChanged(nameof(CurrentShelfStockValue));
                OTCShelfStockValue = CurrentShelfStockValue;
                OTCStockValue = CurrentStockValue;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ProductCollectionView.Filter += ProductTitleFilter;
                    
                }));

                RaisePropertyChanged(nameof(CurrentStockValue));
                RaisePropertyChanged(nameof(CurrentShelfStockValue));

                //ErrorStockValue = TotalStockValue - ShelfStockValue - MedBagStockValue;
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
            filterType = (ProductManageFilterEnum)int.Parse(type);

            ProductCollectionView.Filter += ProductTitleFilter;
            RaisePropertyChanged(nameof(CurrentStockValue));
            RaisePropertyChanged(nameof(CurrentShelfStockValue));
        }

        private void FilterIsOTCAction(string type)
        {
            if (IsOTC == null)
            {
                filterIsOTC = (ProductManageFilterEnum)int.Parse("9");
            }
            else
            {
                filterIsOTC = (ProductManageFilterEnum)int.Parse(IsOTC);
            }

            if (filterIsOTC == (ProductManageFilterEnum)8)
            {
                MEDDeposit = Visibility.Collapsed;
                OTCDeposit = Visibility.Visible;
                ErrorStockValue = TotalStockValue - ShelfStockValue - MedBagStockValue;
            }
            else if (filterIsOTC == (ProductManageFilterEnum)9)
            {
                MEDDeposit = Visibility.Visible;
                OTCDeposit = Visibility.Collapsed;
                ErrorStockValue = TotalStockValue - ShelfStockValue - MedBagStockValue;
            }
            ProductCollectionView.Filter += ProductTitleFilter;
            RaisePropertyChanged(nameof(CurrentStockValue));
            RaisePropertyChanged(nameof(CurrentShelfStockValue));
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommand()
        {
            SearchCommand = new RelayCommand(SearchAction);
            ChangeSearchTypeCommand = new RelayCommand<string>(ChangeSearchTypeAction);
            InsertProductCommand = new RelayCommand(InsertProductAction);
            FilterCommand = new RelayCommand<string>(FilterAction);
            FilterIsOTCCommand = new RelayCommand<string>(FilterIsOTCAction);
            IsOTCCommand = new RelayCommand<string>(IsOTCAction);
        }

        private void IsOTCAction(string obj)
        {
            filterIsOTC = (ProductManageFilterEnum)int.Parse(obj);
            if (obj == "8")
            {
                IsOTC = "8";
            }
            else if (obj == "9")
            {
                IsOTC = "9";
            }

            if (filterIsOTC == (ProductManageFilterEnum)8)
            {
                MEDDeposit = Visibility.Collapsed;
                OTCDeposit = Visibility.Visible;
                ErrorStockValue = TotalStockValue - ShelfStockValue - MedBagStockValue;
            }
            else if (filterIsOTC == (ProductManageFilterEnum)9)
            {
                MEDDeposit = Visibility.Visible;
                OTCDeposit = Visibility.Collapsed;
                ErrorStockValue = TotalStockValue - ShelfStockValue - MedBagStockValue;
            }
            ProductCollectionView.Filter += ProductTitleFilter;
            RaisePropertyChanged(nameof(CurrentStockValue));
            RaisePropertyChanged(nameof(CurrentShelfStockValue));
        }

        private void InsertProductAction()
        {
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

        private bool MEDFilter(object product)
        {
            var tempProduct = product as ProductManageStruct;
            return tempProduct.ProductType != (ProductTypeEnum)2;
        }

        private bool OTCFilter(object product)
        {
            var tempProduct = product as ProductManageStruct;
            return tempProduct.ProductType == (ProductTypeEnum)2;
        }

        private bool ProductTitleFilter(object product)
        {
            var tempProduct = product as ProductManageStruct;

            if (SearchIsEnable == false && SearchIsInventoryZero == false && SearchIsSingdeInventory == false && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.Inventory != 0;
            }
            else if (SearchIsEnable == true && SearchIsInventoryZero == false && SearchIsSingdeInventory == false && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0;
                    }
                }
                return tempProduct.Inventory != 0;
            }
            else if (SearchIsEnable == true && SearchIsInventoryZero == true && SearchIsSingdeInventory == false && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2;
                    }
                }
                return true;
            }
            else if (SearchIsEnable == true && SearchIsInventoryZero == true && SearchIsSingdeInventory == true && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.SINGINV != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.SINGINV != 0;
                    }
                }
                return tempProduct.SINGINV != 0;
            }
            else if (SearchIsEnable == true && SearchIsInventoryZero == true && SearchIsSingdeInventory == true && SearchHasOnWay == true)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                return tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == true && SearchIsSingdeInventory == false && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true;
                    }
                }
                return tempProduct.IsEnable == true;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == true && SearchIsSingdeInventory == true && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.SINGINV != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == true && SearchIsSingdeInventory == true && SearchHasOnWay == true)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == false && SearchIsSingdeInventory == true && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == false && SearchIsSingdeInventory == true && SearchHasOnWay == true)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
            }
            else if (SearchIsEnable == true && SearchIsInventoryZero == false && SearchIsSingdeInventory == true && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
                    }
                }
                return tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
            }
            else if (SearchIsEnable == true && SearchIsInventoryZero == false && SearchIsSingdeInventory == false && SearchHasOnWay == true)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                return tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == true && SearchIsSingdeInventory == true && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.SINGINV != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == true && SearchIsSingdeInventory == true && SearchHasOnWay == true)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.SINGINV != 0 && tempProduct.AllOnTheWayAmount != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == false && SearchIsSingdeInventory == true && SearchHasOnWay == false)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.SINGINV != 0;
            }
            else if (SearchIsEnable == false && SearchIsInventoryZero == false && SearchIsSingdeInventory == false && SearchHasOnWay == true)
            {
                if (filterIsOTC == ProductManageFilterEnum.Medicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.Medicine:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType != (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
                {
                    switch (filterType)
                    {
                        case ProductManageFilterEnum.OTCMedicine:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.COMMON:
                            return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.CONTROL:
                            return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.FROZE:
                            return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.DISABLE:
                            return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.INV_ERROR:
                            return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        case ProductManageFilterEnum.ZERO:
                            return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;

                        default:
                            return tempProduct.ProductType == (ProductTypeEnum)2 && tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;
                    }
                }
                return tempProduct.IsEnable == true && tempProduct.Inventory != 0 && tempProduct.AllOnTheWayAmount != 0;
            }
            else
            {
                return false;
            }
        }

        private bool ProductFilter(object product)
        {
            var tempProduct = product as ProductManageStruct;
            if (filterIsOTC == ProductManageFilterEnum.Medicine)
            {
                switch (filterType)
                {
                    case ProductManageFilterEnum.Medicine:
                        return tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.COMMON:
                        return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.CONTROL:
                        return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.FROZE:
                        return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.DISABLE:
                        return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.INV_ERROR:
                        return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.ZERO:
                        return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)1;

                    default:
                        return tempProduct.ProductType != (ProductTypeEnum)2;
                }
            }
            else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
            {
                switch (filterType)
                {
                    case ProductManageFilterEnum.OTCMedicine:
                        return tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.COMMON:
                        return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.CONTROL:
                        return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.FROZE:
                        return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.DISABLE:
                        return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.INV_ERROR:
                        return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.ZERO:
                        return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2;

                    default:
                        return tempProduct.ProductType == (ProductTypeEnum)2;
                }
            }

            return false;
        }

        private bool ProductIsOTCFilter(object product)
        {
            var tempProduct = product as ProductManageStruct;
            if (filterIsOTC == ProductManageFilterEnum.Medicine)
            {
                switch (filterType)
                {
                    case ProductManageFilterEnum.Medicine:
                        return tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.COMMON:
                        return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.CONTROL:
                        return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.FROZE:
                        return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.DISABLE:
                        return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.INV_ERROR:
                        return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)1;

                    case ProductManageFilterEnum.ZERO:
                        return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)1;

                    default:
                        return tempProduct.ProductType == (ProductTypeEnum)1;
                }
            }
            else if (filterIsOTC == ProductManageFilterEnum.OTCMedicine)
            {
                switch (filterType)
                {
                    case ProductManageFilterEnum.OTCMedicine:
                        return tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.COMMON:
                        return tempProduct.IsCommon && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.CONTROL:
                        return tempProduct.ControlLevel != null && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.FROZE:
                        return tempProduct.IsFrozen && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.DISABLE:
                        return !tempProduct.IsEnable && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.INV_ERROR:
                        return tempProduct.InventoryError && tempProduct.ProductType == (ProductTypeEnum)2;

                    case ProductManageFilterEnum.ZERO:
                        return tempProduct.IsZero == 0 && tempProduct.ProductType == (ProductTypeEnum)2;

                    default:
                        return tempProduct.ProductType == (ProductTypeEnum)2;
                }
            }

            return false;
        }

        #endregion ----- Define Functions -----
    }
}