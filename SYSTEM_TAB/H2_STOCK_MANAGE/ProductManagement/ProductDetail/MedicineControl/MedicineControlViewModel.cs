﻿using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.SetSelfPayMultiplierWindow;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    internal class MedicineControlViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define ViewModels -----

        public MedicineManageViewModel ManageViewModel { get; set; }

        #endregion ----- Define ViewModels -----

        #region ----- Define Commands -----

        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand SyncDataCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        public RelayCommand SetSelfPayMultiplierCommand { get; set; }
        public RelayCommand SetPricesCommand { get; set; }

        #endregion ----- Define Commands -----

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

        #endregion ///// DataChanged Variables /////

        private ProductManageMedicine medicine;
        private ProductTypeEnum productType;
        private bool isNHIProduct;

        public ProductManageMedicine Medicine
        {
            get { return medicine; }
            set { Set(() => Medicine, ref medicine, value); }
        }

        public ProductTypeEnum ProductType
        {
            get { return productType; }
            set { Set(() => ProductType, ref productType, value); }
        }

        public bool IsNHIProduct
        {
            get { return isNHIProduct; }
            set { Set(() => IsNHIProduct, ref isNHIProduct, value); }
        }

        #endregion ----- Define Variables -----

        public MedicineControlViewModel(string proID, ProductTypeEnum type, string wareHouseID)
        {
            ManageViewModel = new MedicineManageViewModel();

            RegisterCommand();
            ProductType = type;
            InitMedicineData(proID, wareHouseID);

            IsNHIProduct = true;
            if ((int)ProductType == 2) IsNHIProduct = true;
        }

        #region ----- Define Actions -----

        private void ConfirmChangeAction()
        {
            if (!IsMedicineDataValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認修改資料?", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            bool isSuccess = Medicine.Save();
            MainWindow.ServerConnection.CloseConnection();

            if (isSuccess)
                IsDataChanged = false;
        }

        private void CancelChangeAction()
        {
            InitMedicineData(Medicine.ID);
            IsDataChanged = false;
        }

        private void SyncDataAction()
        {
        }

        private void DataChangedAction()
        {
            IsDataChanged = true;
        }

        private void SetSelfPayMultiplierAction()
        {
            SetSelfPayMultiplierWindow selfPayMultiplierWindow = new SetSelfPayMultiplierWindow(Medicine.SelfPayMultiplier);
            selfPayMultiplierWindow.ShowDialog();

            if ((bool)selfPayMultiplierWindow.DialogResult)
            {
                Medicine.SelfPayMultiplier = selfPayMultiplierWindow.SelfPayMultiplier;
            }
        }

        //private void ShowConsumeRecordAction()
        //{
        //    ProductConsumeRecordWindow productConsumeRecordWindow = new ProductConsumeRecordWindow(Medicine.ID, SelectedWareHouse);
        //    productConsumeRecordWindow.ShowDialog();
        //}

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommand()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsMedicineDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsMedicineDataChanged);
            SyncDataCommand = new RelayCommand(SyncDataAction);
            DataChangedCommand = new RelayCommand(DataChangedAction);
            SetSelfPayMultiplierCommand = new RelayCommand(SetSelfPayMultiplierAction);
        }

        private void InitMedicineData(string proID, string wareHouseID = "")
        {
            MainWindow.ServerConnection.OpenConnection();
            ManageViewModel.ReloadData(proID, wareHouseID, ProductType);
            DataTable manageMedicineDataTable = ProductDetailDB.GetProductManageMedicineDataByID(proID);
            MainWindow.ServerConnection.CloseConnection();

            if (manageMedicineDataTable is null || manageMedicineDataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            Medicine = new ProductManageMedicine(manageMedicineDataTable.Rows[0]);
        }

        private bool IsMedicineDataChanged()
        {
            return IsDataChanged;
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

            if (Medicine.SelfPayType == SelfPayTypeEnum.Customize)
            {
                if (Medicine.SelfPayPrice is null)
                {
                    MessageWindow.ShowMessage("自訂自費金額不可為空", MessageType.ERROR);
                    return false;
                }
            }

            return true;
        }

        #endregion ----- Define Functions -----
    }
}