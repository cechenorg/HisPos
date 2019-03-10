using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CashFlow;
using System;
using System.Linq;

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
      
        private DateTime startDate = DateTime.Now;
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
        private int clinicProfit;
        public int ClinicProfit
        {
            get => clinicProfit;
            set
            {
                Set(() => ClinicProfit, ref clinicProfit, value);
            }
        }
        private int prescriptionProfit;
        public int PrescriptionProfit
        {
            get => prescriptionProfit;
            set
            {
                Set(() => PrescriptionProfit, ref prescriptionProfit, value);
            }
        }
        private int prescribeProfit;
        public int PrescribeProfit
        {
            get => prescribeProfit;
            set
            {
                Set(() => PrescribeProfit, ref prescribeProfit, value);
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
            CashFlows tempcashFlows = new CashFlows();
            CashFlow cashFlow = new CashFlow();
            if (CooperativeClinicEntryCollection.Count(c => c.Date.Equals(DateTime.Today)) == 0) {
                tempcashFlows.GetCashFlowByDate(DateTime.Today, DateTime.Today);
                cashFlow = tempcashFlows.Count(t => t.Date.Equals(DateTime.Today)) > 0 ? (CashFlow)tempcashFlows.Where(t => t.Date.Equals(DateTime.Today)) : null; 
            }
            else
                cashFlow = CooperativeClinicEntryCollection.Count(t => t.Date.Equals(DateTime.Today)) > 0 ? (CashFlow)CooperativeClinicEntryCollection.Where(t => t.Date.Equals(DateTime.Today)) : null;

            ClinicProfit = cashFlow != null ? (int)cashFlow.ClinicProfitValue : 0;
            PrescriptionProfit = cashFlow != null ? (int)(cashFlow.NormalTotalPointValue + cashFlow.NormalMedicineUseValue + cashFlow.ChronicTotalPointValue + cashFlow.ChronicmedicineUseValue) : 0;
            PrescribeProfit = cashFlow != null ? (int)(cashFlow.PayselfAdjustValue - cashFlow.PayselfMedUseValue) : 0; 
        }
        #endregion
    }
}
