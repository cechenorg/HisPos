using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.StoreOrder;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.IndexReserve;
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.NewClass.Product.ProductDaliyPurchase;
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

        private bool isShowUnPrepareReserve = false;
        public bool IsShowUnPrepareReserve
        {
            get => isShowUnPrepareReserve;
            set
            {
                Set(() => IsShowUnPrepareReserve, ref isShowUnPrepareReserve, value);
                ReserveCollectionViewSource.Filter += Filter;
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
        private List<string> medPrepareStatusString;
        public List<string> MedPrepareStatusString
        {
            get => medPrepareStatusString;
            set
            {
                Set(() => MedPrepareStatusString, ref medPrepareStatusString, value);
            }
        }
        private string medPrepareStatusStringSelectedItem;
        public string MedPrepareStatusStringSelectedItem
        {
            get => medPrepareStatusStringSelectedItem;
            set
            {
                Set(() => MedPrepareStatusStringSelectedItem, ref medPrepareStatusStringSelectedItem, value);
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
        #endregion
        #region Command
        public RelayCommand ReserveSearchCommand { get; set; }
        public RelayCommand ReserveMedicineSendCommand { get; set; }
        public RelayCommand IndexReserveSelectionChangedCommand { get; set; }
        public RelayCommand CommonMedStoreOrderCommand { get; set; }
        public RelayCommand StatusChangedCommand { get; set; }
        #endregion
        public Index() {
            InitStatusstring();
            ReserveSearchCommand = new RelayCommand(ReserveSearchAction);
            IndexReserveSelectionChangedCommand = new RelayCommand(IndexReserveSelectionChangedAction);
            ReserveMedicineSendCommand = new RelayCommand(ReserveMedicineSendAction);
            CommonMedStoreOrderCommand = new RelayCommand(CommonMedStoreOrderAction);
            StatusChangedCommand = new RelayCommand(StatusChangedAction);
            ReserveSearchAction();
        }
        #region Action
        private void InitStatusstring() {
            PhoneCallStatusString = new List<string>() { "未處理", "已聯絡", "電話未接" };
            MedPrepareStatusString = new List<string>() { "未處理", "已備藥", "不備藥" };
        }
        private void StatusChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveSelectedItem.SaveStatus();
            //ReserveCollectionViewSource.Filter += Filter;
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
            foreach (var r in IndexReserveCollection) {
                if (r.PrepareStatus != "F")
                    r.PrepareStatus = "D";
            } 
            StoreOrderDB.StoreOrderReserveByResIDList(StartDate,EndDate);
            MessageWindow.ShowMessage("已轉出採購單 請至進退貨管理確認",MessageType.SUCCESS);
        }
          
        private void IndexReserveSelectionChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveDetailCollection.GetDataById(IndexReserveSelectedItem.Id); 
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

            if (IsShowUnPrepareReserve || IsShowUnPhoneCall)
            {
                if (IsShowUnPrepareReserve && IsShowUnPhoneCall) 
                    e.Accepted = ((IndexReserve)e.Item).PrepareStatus == "F" || ((IndexReserve)e.Item).PhoneCallStatus == "F";
                else if (IsShowUnPrepareReserve && !IsShowUnPhoneCall)
                    e.Accepted = ((IndexReserve)e.Item).PrepareStatus == "F";
                else if (!IsShowUnPrepareReserve && IsShowUnPhoneCall)
                    e.Accepted = ((IndexReserve)e.Item).PhoneCallStatus == "F";
            }
            else {
                if (((IndexReserve)e.Item).PrepareStatus == "F" || ((IndexReserve)e.Item).PhoneCallStatus == "F")
                    e.Accepted = false;
                else
                    e.Accepted = true;

            }  
        }
        #endregion
    }
}
