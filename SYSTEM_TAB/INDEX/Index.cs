using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.StoreOrder;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.IndexReserve;
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
        
        #endregion
        #region Command
        public RelayCommand ReserveSearchCommand { get; set; }
        #endregion
        public Index() {
            ReserveSearchCommand = new RelayCommand(ReserveSearchAction);
        }
        #region Action
        private void ReserveSearchAction() {
            IndexReserveCollection.GetDataByDate(StartDate, EndDate);
        }
        #endregion
    }
}
