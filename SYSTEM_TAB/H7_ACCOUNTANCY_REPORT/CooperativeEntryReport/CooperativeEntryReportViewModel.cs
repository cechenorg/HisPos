using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CashFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CooperativeEntryReport {
    public class CooperativeEntryReportViewModel: TabBase  {
        #region Variables
        public override TabBase getTab()
        {
            return this;
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
        private DateTime endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private CashFlows cashflowCollection = new CashFlows();
        public CashFlows CashflowCollection
        {
            get => cashflowCollection;
            set
            {
                Set(() => CashflowCollection, ref cashflowCollection, value);
            }
        }
        #endregion
        #region Command
        public RelayCommand SearchCommand { get; set; }
        #endregion
        public CooperativeEntryReportViewModel()
        {
            CashflowCollection.GetCashFlowByDate(StartDate, EndDate);
            SearchCommand = new RelayCommand(SearchAction);
        }
        #region Action
        private void SearchAction()
        {
            CashflowCollection.GetCashFlowByDate(StartDate, EndDate);
        }
        #endregion
    }
}
