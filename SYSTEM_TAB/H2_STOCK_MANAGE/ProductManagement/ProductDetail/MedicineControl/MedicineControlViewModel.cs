using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductGroupSetting;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductManageDetail;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using His_Pos.NewClass.WareHouse;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ConsumeRecordWindow;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    class MedicineControlViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define ViewModels -----
        public MedicineManageViewModel ManageViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand SyncDataCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
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
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        //private void ShowConsumeRecordAction()
        //{
        //    ProductConsumeRecordWindow productConsumeRecordWindow = new ProductConsumeRecordWindow(Medicine.ID, SelectedWareHouse);
        //    productConsumeRecordWindow.ShowDialog();
        //}
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsMedicineDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsMedicineDataChanged);
            SyncDataCommand = new RelayCommand(SyncDataAction);
            DataChangedCommand = new RelayCommand(DataChangedAction);
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

            SearchProductRecordAction();
        }
        private bool IsMedicineDataChanged()
        {
            return IsDataChanged;
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

                if (Medicine.BasicAmount < Medicine.SafeAmount)
                {
                    MessageWindow.ShowMessage("基準量不可小於安全量", MessageType.ERROR);
                    return false;
                }

                if (Medicine.BasicAmount < 0)
                {
                    MessageWindow.ShowMessage("基準量必須大於0", MessageType.ERROR);
                    return false;
                }

                if (Medicine.SafeAmount < 0)
                {
                    MessageWindow.ShowMessage("安全量不可小於0", MessageType.ERROR);
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
