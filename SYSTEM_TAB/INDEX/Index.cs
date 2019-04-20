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
        public RelayCommand IndexReserveSelectionChangedCommand { get; set; }
        #endregion
        public Index() {
            ReserveSearchCommand = new RelayCommand(ReserveSearchAction);
            IndexReserveSelectionChangedCommand = new RelayCommand(IndexReserveSelectionChangedAction);
        }
        #region Action
        private void IndexReserveSelectionChangedAction() {
            if (IndexReserveSelectedItem is null) return;
            IndexReserveDetailCollection.GetDataById(IndexReserveSelectedItem.Id);
        }
        private void ReserveSearchAction() {
            IndexReserveCollection.GetDataByDate(StartDate, EndDate);
        }
        #endregion
    }
}
