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
        private CashFlow totalCopaymentEntry = new CashFlow();
        public CashFlow TotalCopaymentEntry
        {
            get => totalCopaymentEntry;
            set
            {
                Set(() => TotalCopaymentEntry, ref totalCopaymentEntry, value);
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
        private decimal paySelfProfit;
        public decimal PaySelfProfit
        {
            get => paySelfProfit;
            set
            {
                Set(() => PaySelfProfit, ref paySelfProfit, value);
            }
        }
        private decimal payToCooperativeClinic;
        public decimal PayToCooperativeClinic
        {
            get => payToCooperativeClinic;
            set
            {
                Set(() => PayToCooperativeClinic, ref payToCooperativeClinic, value);
            }
        }
        private decimal prescriptionProfit;
        public decimal PrescriptionProfit
        {
            get => prescriptionProfit;
            set
            {
                Set(() => PrescriptionProfit, ref prescriptionProfit, value);
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
            CooperativeClinicEntryCollection.GetCashFlowByDate(StartDate);
            TotalCopaymentEntry = CooperativeClinicEntryCollection[0];
            PaySelfProfit = TotalCopaymentEntry.PaySelfValue + TotalCopaymentEntry.PayselfMedUseValue;
            PayToCooperativeClinic = TotalCopaymentEntry.ClinicCopaymentValue + TotalCopaymentEntry.ClinicPaySelfValue;
            PrescriptionProfit = TotalCopaymentEntry.NormalTotalPointValue + TotalCopaymentEntry.ChronicTotalPointValue + 
                                 TotalCopaymentEntry.NormalMedicineUseValue + TotalCopaymentEntry.ChronicmedicineUseValue;
        }
        #endregion
    }
}
