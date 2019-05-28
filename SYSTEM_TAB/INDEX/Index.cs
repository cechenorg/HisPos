using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.IndexReserve;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.StoreOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.INDEX
{
    class Index : TabBase {
        public override TabBase getTab() {
            return this;
        }
        #region Var
        private CollectionViewSource reserveCollectionViewSource;
        private CollectionViewSource ReserveCollectionViewSource
        {
            get => reserveCollectionViewSource;
            set
            {
                Set(() => ReserveCollectionViewSource, ref reserveCollectionViewSource, value);
                SetPhoneCount();
            }
        }

        private ICollectionView reserveCollectionView;
        public ICollectionView ReserveCollectionView
        {
            get => reserveCollectionView;
            private set
            {
                Set(() => ReserveCollectionView, ref reserveCollectionView, value); 
            }
        }
       
        private int indexReserveCount;  
        public int IndexReserveCount
        {
            get => indexReserveCount;
            set
            {
                Set(() => IndexReserveCount, ref indexReserveCount, value); 
            }
        }
        private bool isShowUnPrepareReserve = false;
        public bool IsShowUnPrepareReserve
        {
            get => isShowUnPrepareReserve;
            set
            {
                Set(() => IsShowUnPrepareReserve, ref isShowUnPrepareReserve, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        private bool isShowUnPhoneCall = false;
        public bool IsShowUnPhoneCall
        {
            get => isShowUnPhoneCall;
            set
            { 
                Set(() => IsShowUnPhoneCall, ref isShowUnPhoneCall, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        private bool isShowUnPhoneProcess = false;
        public bool IsShowUnPhoneProcess
        {
            get => isShowUnPhoneProcess;
            set
            {
                Set(() => IsShowUnPhoneProcess, ref isShowUnPhoneProcess, value);
                ReserveCollectionViewSource.Filter += Filter;
                SetPhoneCount();
            }
        }
        
        private DateTime startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime endDate = DateTime.Today.AddDays(5);
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private IndexReserves indexReserveCollection = new IndexReserves();
        public IndexReserves IndexReserveCollection
        {
            get => indexReserveCollection;
            set
            {
                Set(() => IndexReserveCollection, ref indexReserveCollection, value);
            }
        }
        private IndexReserve indexReserveSelectedItem;
        public IndexReserve IndexReserveSelectedItem
        {
            get => indexReserveSelectedItem;
            set
            {
                Set(() => IndexReserveSelectedItem, ref indexReserveSelectedItem, value);
            }
        }
        private IndexReserveDetails indexReserveDetailCollection = new IndexReserveDetails();
        public IndexReserveDetails IndexReserveDetailCollection
        {
            get => indexReserveDetailCollection;
            set
            {
                Set(() => IndexReserveDetailCollection, ref indexReserveDetailCollection, value);
            }
        }
        private List<string> phoneCallStatusString;
        public List<string> PhoneCallStatusString
        {
            get => phoneCallStatusString;
            set
            {
                Set(() => PhoneCallStatusString, ref phoneCallStatusString, value);
            }
        }
      
        private string phoneCallStatusStringSelectedItem;
        public string PhoneCallStatusStringSelectedItem
        {
            get => phoneCallStatusStringSelectedItem;
            set
            {
                Set(() => PhoneCallStatusStringSelectedItem, ref phoneCallStatusStringSelectedItem, value);
            }
        }
        private Customer customerData = new Customer();
        public Customer CustomerData
        {
            get => customerData;
            set
            {
                Set(() => CustomerData, ref customerData, value);
            }
        }
        #endregion
        #region Command
        public RelayCommand ReserveSearchCommand { get; set; }
        public RelayCommand ReserveMedicineSendCommand { get; set; }
        public RelayCommand IndexReserveSelectionChangedCommand { get; set; }
        public RelayCommand CommonMedStoreOrderCommand { get; set; }
        public RelayCommand StatusChangedCommand { get; set; }
        public RelayCommand ShowCustomerDetailWindowCommand { get; set; }
        public RelayCommand CustomerDataSaveCommand { get; set; }
        public RelayCommand ShowCustomerPrescriptionChangedCommand { get; set; }

        #endregion
        public Index() {
            InitStatusstring();
            ReserveSearchCommand = new RelayCommand(ReserveSearchAction);
            IndexReserveSelectionChangedCommand = new RelayCommand(IndexReserveSelectionChangedAction);
            ReserveMedicineSendCommand = new RelayCommand(ReserveMedicineSendAction);
            CommonMedStoreOrderCommand = new RelayCommand(CommonMedStoreOrderAction);
            StatusChangedCommand = new RelayCommand(StatusChangedAction);
            ShowCustomerDetailWindowCommand = new RelayCommand(ShowCustomerDetailWindowAction);
            CustomerDataSaveCommand = new RelayCommand(CustomerDataSaveAction);
            ShowCustomerPrescriptionChangedCommand = new RelayCommand(ShowCustomerPrescriptionChangedAction);
            ReserveSearchAction();
        }
        #region Action
        private void ShowCustomerPrescriptionChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            CustomerPrescriptionChangedWindow.CustomerPrescriptionChangedWindow customerPrescriptionChangedWindow = new CustomerPrescriptionChangedWindow.CustomerPrescriptionChangedWindow(IndexReserveSelectedItem.CusId); 
        }
        private void CustomerDataSaveAction() {
            CustomerData.Save();
        }
        private void SetPhoneCount() {
            int filteredCount = 0;
            foreach (var item in ReserveCollectionViewSource.View)
            {
                filteredCount++;
            }
            IndexReserveCount = filteredCount;
        }
        private void ShowCustomerDetailWindowAction() {
            if (IndexReserveSelectedItem is null) return; 
            CustomerDetailWindow.CustomerDetailWindow customerDetailWindow = new CustomerDetailWindow.CustomerDetailWindow(IndexReserveSelectedItem.CusId); 
        }
        private void InitStatusstring() {
            PhoneCallStatusString = new List<string>() { "未處理", "已聯絡", "電話未接" };
        }
        private void StatusChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveSelectedItem.SaveStatus();
            CustomerData = CustomerData.GetCustomerByCusId(IndexReserveSelectedItem.CusId);
        }
        private void CommonMedStoreOrderAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否將已設定為常備藥且低於安全量之藥品產生採購製表至基準量?","常備藥轉採購");
            if ((bool)confirmWindow.DialogResult)
            {
                StoreOrderDB.StoreOrderCommonMedicine();
                MessageWindow.ShowMessage("已轉出採購單 請至進退貨管理確認", MessageType.SUCCESS);
            }
        }
        private void ReserveMedicineSendAction()
        {
            
            StoreOrderDB.StoreOrderReserveByResIDList(StartDate,EndDate);
            MessageWindow.ShowMessage("已轉出採購單 請至進退貨管理確認",MessageType.SUCCESS);
        }
          
        private void IndexReserveSelectionChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveDetailCollection.GetDataById(IndexReserveSelectedItem.Id);
            CustomerData = CustomerData.GetCustomerByCusId(IndexReserveSelectedItem.CusId);
        }
        private void ReserveSearchAction() {
            IndexReserveCollection.GetDataByDate(StartDate, EndDate);
            ReserveCollectionViewSource = new CollectionViewSource { Source = IndexReserveCollection };
            ReserveCollectionView = ReserveCollectionViewSource.View;
            ReserveCollectionViewSource.Filter += Filter;
        }
        private void Filter(object sender, FilterEventArgs e) {
            if (e.Item is null) return;
            if (!(e.Item is IndexReserve src))
                e.Accepted = false;

            e.Accepted = false;

           IndexReserve indexitem = ((IndexReserve)e.Item);
            if (indexitem.IsNoPrepareMed && IsShowUnPrepareReserve)
                e.Accepted = true;
            else if (indexitem.PhoneCallStatus == "F" && IsShowUnPhoneCall)
                e.Accepted = true;
            else if (indexitem.PhoneCallStatus == "N" && IsShowUnPhoneProcess)
                e.Accepted = true;
            else if(!IsShowUnPrepareReserve && !IsShowUnPhoneCall && !IsShowUnPhoneProcess)
                e.Accepted = true;
            
        }
        #endregion
    }
}
