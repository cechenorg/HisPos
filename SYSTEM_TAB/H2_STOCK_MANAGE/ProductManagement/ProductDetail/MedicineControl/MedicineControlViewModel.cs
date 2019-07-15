using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ExportProductRecord;
using His_Pos.NewClass.Product.ProductGroupSetting;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductManageDetail;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using His_Pos.NewClass.StoreOrder;
using His_Pos.NewClass.WareHouse;
using His_Pos.Service.ExportService;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ConsumeRecordWindow;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    class MedicineControlViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand SyncDataCommand { get; set; }
        public RelayCommand StockTakingCommand { get; set; }
        public RelayCommand ViewHistoryPriceCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        public RelayCommand SearchProductRecordCommand { get; set; }
        public RelayCommand ExportRecordCommand { get; set; }
        public RelayCommand ShowConsumeRecordCommand { get; set; }
        public RelayCommand<string> FilterRecordCommand { get; set; }
        public RelayCommand ShowProductGroupWindowCommand { get; set; }
        #endregion

        #region ----- Define Variables -----

        #region ///// DataChanged Variables /////
        private bool isDataChanged;

        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set
            {
                Set(() => IsDataChanged, ref isDataChanged, value);
                CancelChangeCommand.RaiseCanExecuteChanged();
                ConfirmChangeCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region ///// Medicine Variables /////
        private ProductManageMedicine medicine;
        private MedicineStockDetail stockDetail;
        private ProductInventoryRecords inventoryRecordCollection;
        private ProductInventoryRecord currentInventoryRecord;
        private ICollectionView inventoryRecordCollectionView;
        private ProductInventoryRecordType filterType = ProductInventoryRecordType.All;
        private ProductTypeEnum productType;

        public ProductManageMedicine Medicine
        {
            get { return medicine; }
            set { Set(() => Medicine, ref medicine, value); }
        }
        public MedicineStockDetail StockDetail
        {
            get { return stockDetail; }
            set { Set(() => StockDetail, ref stockDetail, value); }
        }
        public ProductInventoryRecords InventoryRecordCollection
        {
            get { return inventoryRecordCollection; }
            set { Set(() => InventoryRecordCollection, ref inventoryRecordCollection, value); }
        }
        public ProductInventoryRecord CurrentInventoryRecord
        {
            get { return currentInventoryRecord; }
            set { Set(() => CurrentInventoryRecord, ref currentInventoryRecord, value); }
        }
        public ProductTypeEnum ProductType
        {
            get { return productType; }
            set { Set(() => ProductType, ref productType, value); }
        }
        public ICollectionView InventoryRecordCollectionView
        {
            get => inventoryRecordCollectionView;
            set { Set(() => InventoryRecordCollectionView, ref inventoryRecordCollectionView, value); }
        }
        public ProductManageDetail MedicineDetail { get; set; }
        #endregion

        private string newInventory = "";
        private WareHouse selectedWareHouse;
        private ProductGroupSettings productGroupSettingCollection;
        private ProductRegisterPrescriptions productRegisterPrescriptionCollection;
        private DateTime? startDate = DateTime.Today.AddMonths(-3);
        private DateTime? endDate = DateTime.Today;
        
        public string NewInventory
        {
            get { return newInventory; }
            set
            {
                Set(() => NewInventory, ref newInventory, value);
                StockTakingCommand.RaiseCanExecuteChanged();
            }
        }
        public ProductGroupSettings ProductGroupSettingCollection
        {
            get { return productGroupSettingCollection; }
            set { Set(() => ProductGroupSettingCollection, ref productGroupSettingCollection, value); }
        }
        public ProductRegisterPrescriptions ProductRegisterPrescriptionCollection
        {
            get { return productRegisterPrescriptionCollection; }
            set { Set(() => ProductRegisterPrescriptionCollection, ref productRegisterPrescriptionCollection, value); }
        }
        public WareHouses WareHouseCollection { get; set; }
        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set
            {
                Set(() => SelectedWareHouse, ref selectedWareHouse, value); 
                SearchProductRecordAction();
            }
        }
        public DateTime? StartDate
        {
            get { return startDate; }
            set { Set(() => StartDate, ref startDate, value); }
        }
        public DateTime? EndDate
        {
            get { return endDate; }
            set { Set(() => EndDate, ref endDate, value); }
        }
        #endregion

        public MedicineControlViewModel(string proID, ProductTypeEnum type, string wareHouseID)
        {
            RegisterCommand();
            ProductType = type;
            InitMedicineData(proID, wareHouseID);
        }
        
        #region ----- Define Actions -----
        private void ConfirmChangeAction()
        {
            if(!IsMedicineDataValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認修改資料?", "");

            if(!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            Medicine.Save();
            MainWindow.ServerConnection.CloseConnection();
            
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            InitMedicineData(Medicine.ID, SelectedWareHouse.ID);
            IsDataChanged = false;
        }
        private void SyncDataAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            //ProductDetailDB.GetProductManageMedicineDataByID(proID);
            MainWindow.ServerConnection.CloseConnection();

            InitMedicineData(Medicine.ID, SelectedWareHouse.ID);
        }
        private void StockTakingAction()
        {
            if(!IsNewInventoryValid()) return;

            if (StockDetail.LastPrice == 0.0)
            {
                StockTakingNoLastPriceWindow stockTakingNoLastPriceWindow = new StockTakingNoLastPriceWindow();
                stockTakingNoLastPriceWindow.ShowDialog();

                if (stockTakingNoLastPriceWindow.ConfirmClicked)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    ProductDetailDB.UpdateProductLastPrice(Medicine.ID, stockTakingNoLastPriceWindow.Price, SelectedWareHouse.ID);
                    MainWindow.ServerConnection.CloseConnection();
                }
                else
                    return;
            }
            
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將庫存調整為 {NewInventory} ?", "");

            if(!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            ProductDetailDB.StockTakingProductManageMedicineByID(Medicine.ID, NewInventory,SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();

            InitMedicineData(Medicine.ID, SelectedWareHouse.ID);

            NewInventory = "";
        }
        private void ViewHistoryPriceAction()
        {
            NHIMedicineHistoryPriceWindow medicineHistoryPriceWindow = new NHIMedicineHistoryPriceWindow(Medicine.ID);
            medicineHistoryPriceWindow.ShowDialog();
        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        private void SearchProductRecordAction()
        {
            if (StartDate is null || EndDate is null)
            {
                MessageWindow.ShowMessage("日期格式錯誤", MessageType.ERROR);
                return;
            }

            MainWindow.ServerConnection.OpenConnection();
            InventoryRecordCollection = ProductInventoryRecords.GetInventoryRecordsByID(Medicine.ID, SelectedWareHouse.ID, (DateTime)StartDate, (DateTime)EndDate);
            
            InventoryRecordCollectionView = CollectionViewSource.GetDefaultView(InventoryRecordCollection);
            InventoryRecordCollectionView.Filter += RecordFilter;

            if (!InventoryRecordCollectionView.IsEmpty)
            {
                InventoryRecordCollectionView.MoveCurrentToLast();
                CurrentInventoryRecord = (ProductInventoryRecord)InventoryRecordCollectionView.CurrentItem;
            }

            ReloadStockDetail();
            ReloadProductGroupAndPrescription();
            MainWindow.ServerConnection.CloseConnection();
        }
        private void ExportRecordAction()
        {
            Collection<object> tempCollection = new Collection<object>() { new List<object>{Medicine.ID, StartDate, EndDate, SelectedWareHouse.ID} };

            MainWindow.ServerConnection.OpenConnection();
            ExportExcelService service = new ExportExcelService(tempCollection, new ExportProductRecordTemplate());
            bool isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{Medicine.ID}商品歷程{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
            MainWindow.ServerConnection.CloseConnection();

            if (isSuccess)
                MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
            else
                MessageWindow.ShowMessage("匯出失敗 請稍後再試", MessageType.ERROR);
        }
        private void ShowConsumeRecordAction()
        {
            ProductConsumeRecordWindow productConsumeRecordWindow = new ProductConsumeRecordWindow(Medicine.ID, SelectedWareHouse);
            productConsumeRecordWindow.ShowDialog();
        }
        private void FilterRecordAction(string filterCondition)
        {
            if (filterCondition != null)
                filterType = (ProductInventoryRecordType)int.Parse(filterCondition);

            InventoryRecordCollectionView.Filter += RecordFilter;

            if (!InventoryRecordCollectionView.IsEmpty)
            {
                InventoryRecordCollectionView.MoveCurrentToLast();
                CurrentInventoryRecord = (ProductInventoryRecord)InventoryRecordCollectionView.CurrentItem;
            }
        }
        private void ShowProductGroupWindowAction()
        {
            if (StockDetail.TotalInventory < 0)
            {
                MessageWindow.ShowMessage("欲調整商品群組 需先解決負庫問題", MessageType.ERROR);
                return;
            }

            ProductGroupSettingWindow productGroupSettingWindow = new ProductGroupSettingWindow(ProductGroupSettingCollection, SelectedWareHouse.ID, StockDetail.TotalInventory);
            productGroupSettingWindow.ShowDialog();

            SearchProductRecordAction();
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsMedicineDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsMedicineDataChanged);
            SyncDataCommand = new RelayCommand(SyncDataAction);
            StockTakingCommand = new RelayCommand(StockTakingAction, IsNewInventoryHasValue);
            ViewHistoryPriceCommand = new RelayCommand(ViewHistoryPriceAction);
            DataChangedCommand = new RelayCommand(DataChangedAction);
            SearchProductRecordCommand = new RelayCommand(SearchProductRecordAction);
            FilterRecordCommand = new RelayCommand<string>(FilterRecordAction);
            ShowProductGroupWindowCommand = new RelayCommand(ShowProductGroupWindowAction);

            ExportRecordCommand = new RelayCommand(ExportRecordAction);
            ShowConsumeRecordCommand = new RelayCommand(ShowConsumeRecordAction);
        }
        private void InitMedicineData(string proID, string wareHouseID)
        {
            MainWindow.ServerConnection.OpenConnection();
            WareHouseCollection = WareHouses.GetWareHouses();

            DataTable manageMedicineDetailDataTable = null;
            
            switch (ProductType)
            {
                case ProductTypeEnum.OTCMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageOTCMedicineDetailByID(proID);
                    break;
                case ProductTypeEnum.NHIMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageNHIMedicineDetailByID(proID);
                    break;
                case ProductTypeEnum.SpecialMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageSpecialMedicineDetailByID(proID);
                    break;
            }

            DataTable manageMedicineDataTable = ProductDetailDB.GetProductManageMedicineDataByID(proID);
            MainWindow.ServerConnection.CloseConnection();

            if (manageMedicineDataTable is null || manageMedicineDetailDataTable is null || manageMedicineDataTable.Rows.Count == 0 || manageMedicineDetailDataTable.Rows.Count == 0 || WareHouseCollection is null || WareHouseCollection.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            Medicine = new ProductManageMedicine(manageMedicineDataTable.Rows[0]);
            selectedWareHouse = WareHouseCollection[int.Parse(wareHouseID)];

            switch (ProductType)
            {
                case ProductTypeEnum.OTCMedicine:
                    MedicineDetail = new ProductManageDetail(manageMedicineDetailDataTable.Rows[0]);
                    break;
                case ProductTypeEnum.NHIMedicine:
                    MedicineDetail = new ProductNHIDetail(manageMedicineDetailDataTable.Rows[0]);
                    break;
                case ProductTypeEnum.SpecialMedicine:
                    MedicineDetail = new ProductNHISpecialDetail(manageMedicineDetailDataTable.Rows[0]);
                    break;
            }

            ReloadStockDetail();
            ReloadProductGroupAndPrescription();
        }
        private bool IsMedicineDataChanged()
        {
            return IsDataChanged;
        }
        private bool IsNewInventoryHasValue()
        {
            return !NewInventory.Equals(string.Empty);
        }
        private bool IsNewInventoryValid()
        {
            double newCheckedInventory = 0;
            bool isDouble = double.TryParse(NewInventory, out newCheckedInventory);

            if (!isDouble)
            {
                MessageWindow.ShowMessage("輸入數值錯誤!", MessageType.ERROR);
                return false;
            }

            if (newCheckedInventory < 0)
            {
                MessageWindow.ShowMessage("輸入數值不可小於0!", MessageType.ERROR);
                return false;
            }

            return true;
        }
        private void ReloadStockDetail()
        {
            DataTable stockDataTable = ProductDetailDB.GetMedicineStockDetailByID(Medicine.ID, SelectedWareHouse.ID);

            if (stockDataTable is null || stockDataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            StockDetail = new MedicineStockDetail(stockDataTable.Rows[0]);
        }
        private void ReloadProductGroupAndPrescription()
        {
            ProductGroupSettingCollection = ProductGroupSettings.GetProductGroupSettingsByID(Medicine.ID, SelectedWareHouse.ID);
            ProductRegisterPrescriptionCollection = ProductRegisterPrescriptions.GetRegisterPrescriptionsByID(Medicine.ID, SelectedWareHouse.ID);
        }
        private bool RecordFilter(object record)
        {
            if (ProductInventoryRecordType.All == filterType)
                return true;
            else
                return filterType == ((ProductInventoryRecord) record).Type;
        }
        private bool IsMedicineDataValid()
        {
            if (Medicine.IsCommon)
            {
                if (Medicine.BasicAmount is null || Medicine.SafeAmount is null)
                {
                    MessageWindow.ShowMessage("基準量及安全量為必填", MessageType.ERROR);
                    return false;
                }

                if (Medicine.MinOrderAmount < 0)
                {
                    MessageWindow.ShowMessage("包裝量不能小於1", MessageType.ERROR);
                    return false;
                }

                if (Medicine.BasicAmount <= Medicine.SafeAmount)
                {
                    MessageWindow.ShowMessage("基準量必須大於安全量", MessageType.ERROR);
                    return false;
                }

                if (Medicine.BasicAmount < 0)
                {
                    MessageWindow.ShowMessage("基準量必須大於0", MessageType.ERROR);
                    return false;
                }

                if (Medicine.SafeAmount < 0)
                {
                    MessageWindow.ShowMessage("安全量必須大於0", MessageType.ERROR);
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
