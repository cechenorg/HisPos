﻿using System;
using System.Data;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductGroupSetting;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductManageDetail;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using His_Pos.NewClass.WareHouse;

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
        public RelayCommand ShowProductGroupWindowCommand { get; set; }
        public RelayCommand SearchProductRecordCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isDataChanged;
        private string newInventory = "";
        private ProductManageMedicine medicine;
        private WareHouse selectedWareHouse;
        private ProductInventoryRecords inventoryRecordCollection;
        private ProductTypeEnum productType;
        private DateTime? startDate = DateTime.Today.AddMonths(-3);
        private DateTime? endDate = DateTime.Today;
        private MedicineStockDetail stockDetail;

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
        public string NewInventory
        {
            get { return newInventory; }
            set
            {
                Set(() => NewInventory, ref newInventory, value);
                StockTakingCommand.RaiseCanExecuteChanged();
            }
        }
        public ProductManageMedicine Medicine
        {
            get { return medicine; }
            set
            {
                Set(() => Medicine, ref medicine, value);
            }
        }
        private ProductGroupSettings productGroupSettingCollection = new ProductGroupSettings();
        public ProductGroupSettings ProductGroupSettingCollection
        {
            get { return productGroupSettingCollection; }
            set { Set(() => ProductGroupSettingCollection, ref productGroupSettingCollection, value); }
        }
        public ProductManageMedicine BackUpMedicine { get; set; }
        public ProductManageDetail MedicineDetail { get; set; }
        public MedicineStockDetail StockDetail
        {
            get { return stockDetail; }
            set { Set(() => StockDetail, ref stockDetail, value); }
        }
        public WareHouses WareHouseCollection { get; set; }
        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set
            {
                Set(() => SelectedWareHouse, ref selectedWareHouse, value); 
                ReloadStockDetail();
                SearchProductRecordAction();
            }
        }
        public ProductInventoryRecords InventoryRecordCollection
        {
            get { return inventoryRecordCollection; }
            set { Set(() => InventoryRecordCollection, ref inventoryRecordCollection, value); }
        }
        public ProductTypeEnum ProductType
        {
            get { return productType; }
            set { Set(() => ProductType, ref productType, value); }
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

        public MedicineControlViewModel(string id, ProductTypeEnum type)
        {
            RegisterCommand();
            ProductType = type;
            InitMedicineData(id);
        }
        
        #region ----- Define Actions -----
        private void ConfirmChangeAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認修改資料?", "");

            if(!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            Medicine.Save();
            MainWindow.ServerConnection.CloseConnection();

            BackUpMedicine = Medicine.Clone() as ProductManageMedicine;
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            Medicine = BackUpMedicine.Clone() as ProductManageMedicine;
            IsDataChanged = false;
        }
        private void SyncDataAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            //ProductDetailDB.GetProductManageMedicineDataByID(id);
            MainWindow.ServerConnection.CloseConnection();

            InitMedicineData(Medicine.ID);
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
                    ProductDetailDB.UpdateProductLastPrice(Medicine.ID, stockTakingNoLastPriceWindow.Price);
                    MainWindow.ServerConnection.CloseConnection();
                }
                else
                    return;
            }
            
            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將庫存調整為 {NewInventory} ?", "");

            if(!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            ProductDetailDB.StockTakingProductManageMedicineByID(Medicine.ID, NewInventory);
            MainWindow.ServerConnection.CloseConnection();

            InitMedicineData(Medicine.ID);

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
        private void ShowProductGroupWindowAction() {
            ProductGroupSettingWindow productGroupSettingWindow = new ProductGroupSettingWindow(Medicine.ID);
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
            MainWindow.ServerConnection.CloseConnection();
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
            ShowProductGroupWindowCommand = new RelayCommand(ShowProductGroupWindowAction);
            SearchProductRecordCommand = new RelayCommand(SearchProductRecordAction);
        }
        private void InitMedicineData(string id)
        {
            MainWindow.ServerConnection.OpenConnection();
            WareHouseCollection = WareHouses.GetWareHouses();

            DataTable manageMedicineDetailDataTable = null;
            
            switch (ProductType)
            {
                case ProductTypeEnum.OTCMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageOTCMedicineDetailByID(id);
                    break;
                case ProductTypeEnum.NHIMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageNHIMedicineDetailByID(id);
                    break;
                case ProductTypeEnum.SpecialMedicine:
                    manageMedicineDetailDataTable = ProductDetailDB.GetProductManageSpecialMedicineDetailByID(id);
                    break;
            }

            DataTable manageMedicineDataTable = ProductDetailDB.GetProductManageMedicineDataByID(id);
            MainWindow.ServerConnection.CloseConnection();

            if (manageMedicineDataTable is null || manageMedicineDetailDataTable is null || manageMedicineDataTable.Rows.Count == 0 || manageMedicineDetailDataTable.Rows.Count == 0 || WareHouseCollection is null || WareHouseCollection.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            Medicine = new ProductManageMedicine(manageMedicineDataTable.Rows[0]);
            SelectedWareHouse = WareHouseCollection[0];

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
            
            BackUpMedicine = Medicine.Clone() as ProductManageMedicine;
            ProductGroupSettingCollection.GetDataByID(id);

            ReloadStockDetail();
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
            MainWindow.ServerConnection.OpenConnection();
            DataTable stockDataTable = ProductDetailDB.GetMedicineStockDetailByID(Medicine.ID, SelectedWareHouse.ID);

            if (stockDataTable is null || stockDataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            StockDetail = new MedicineStockDetail(stockDataTable.Rows[0]);
            MainWindow.ServerConnection.CloseConnection();
        }
        
        #endregion
    }
}
