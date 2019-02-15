using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CashFlow;
using System;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeEntry
{
    public class CooperativeEntry : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        #region Var
        private CashFlows cooperativeClinicEntryCollection = new CashFlows();
        public CashFlows CooperativeClinicEntryCollection
        {
            get => cooperativeClinicEntryCollection;
            set
            {
                Set(() => CooperativeClinicEntryCollection, ref cooperativeClinicEntryCollection, value);
            }
        }
        private CashFlow totalCopaymentEntry = new CashFlow();
        public CashFlow TotalCopaymentEntry
        {
            get => totalCopaymentEntry;
            set
            {
                Set(() => TotalCopaymentEntry, ref totalCopaymentEntry, value);
            }
        }
        private DateTime startDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        #endregion
        #region Command
        public RelayCommand SearchCommand { get; set; } 
        #endregion
        public CooperativeEntry() {
            SearchCommand = new RelayCommand(SearchAction);
        }
        #region Action
        private void SearchAction() {
            CooperativeClinicEntryCollection.GetCashFlowByDate(StartDate,EndDate);
        }
        #endregion
    }
}
