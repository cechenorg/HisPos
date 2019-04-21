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

namespace His_Pos.SYSTEM_TAB.INDEX
{
    class Index : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        #region Var
        private bool isShowUnPrepareReserve = false;
        public bool IsShowUnPrepareReserve
        {
            get => isShowUnPrepareReserve;
            set
            {
                Set(() => IsShowUnPrepareReserve, ref isShowUnPrepareReserve, value);
            }
        }
        private bool isShowUnPhoneCall = false;
        public bool IsShowUnPhoneCall
        {
            get => isShowUnPhoneCall;
            set
            {
                Set(() => IsShowUnPhoneCall, ref isShowUnPhoneCall, value);
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
        
        #endregion
        #region Command
        public RelayCommand ReserveSearchCommand { get; set; }
        public RelayCommand ReserveMedicineSendCommand { get; set; }
        public RelayCommand IndexReserveSelectionChangedCommand { get; set; }
        public RelayCommand SetPhoneCallNoCommand { get; set; }
        public RelayCommand SetPhoneCallYesCommand { get; set; }
        public RelayCommand SetPrepareMedNoCommand { get; set; }
        public RelayCommand CommonMedStoreOrderCommand { get; set; }
        #endregion
        public Index() {
            ReserveSearchCommand = new RelayCommand(ReserveSearchAction);
            IndexReserveSelectionChangedCommand = new RelayCommand(IndexReserveSelectionChangedAction);
            SetPhoneCallNoCommand = new RelayCommand(SetPhoneCallNoAction);
            SetPhoneCallYesCommand = new RelayCommand(SetPhoneCallYesAction);
            SetPrepareMedNoCommand = new RelayCommand(SetPrepareMedNoAction);
            ReserveMedicineSendCommand = new RelayCommand(ReserveMedicineSendAction);
            CommonMedStoreOrderCommand = new RelayCommand(CommonMedStoreOrderAction);
        }
        #region Action
        private void CommonMedStoreOrderAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否將已設定為常備藥且低於安全量之藥品產生採購製表至基準量?","常備藥轉採購");
            StoreOrderDB.StoreOrderCommonMedicine();
        }
        private void ReserveMedicineSendAction()
        {
            List<int> idList = new List<int>();
            foreach (var r in IndexReserveCollection) {
                if (r.IsSend)
                {
                    idList.Add(r.Id);
                    r.PrepareStatus = "D";
                } 
            } 
            StoreOrderDB.StoreOrderReserveByResIDList(idList);
            MessageWindow.ShowMessage("已轉出採購單 請至進退貨管理確認",MessageType.SUCCESS);
            IndexReserveCollection.GetDataByDate(StartDate, EndDate,IsShowUnPhoneCall,IsShowUnPrepareReserve);
        }
        private void SetPhoneCallNoAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveSelectedItem.PhoneCallStatus = "F";
            IndexReserveSelectedItem.SaveStatus();
            IndexReserveCollection.Remove(IndexReserveSelectedItem);
        }
        private void SetPhoneCallYesAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveSelectedItem.PhoneCallStatus = "D";
            IndexReserveSelectedItem.SaveStatus();
        }
        private void SetPrepareMedNoAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveSelectedItem.PrepareStatus = "F";
            IndexReserveSelectedItem.SaveStatus();
            IndexReserveCollection.Remove(IndexReserveSelectedItem);
        }
        private void IndexReserveSelectionChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveDetailCollection.GetDataById(IndexReserveSelectedItem.Id);
        }
        private void ReserveSearchAction() {
            IndexReserveCollection.GetDataByDate(StartDate, EndDate, IsShowUnPhoneCall, IsShowUnPrepareReserve);
        }
        #endregion
    }
}
