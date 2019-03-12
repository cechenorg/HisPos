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
            SearchAction(); 
            SearchCommand = new RelayCommand(SearchAction);
        }
        #region Action
        private void SearchAction() {
            CooperativeClinicEntryCollection.GetCashFlowByDate(StartDate, EndDate);
            ClinicProfit = 0;
            PrescriptionProfit = 0;
            PrescribeProfit = 0;
            foreach (CashFlow c in CooperativeClinicEntryCollection)
            {
                ClinicProfit += (int)c.ClinicProfitValue;
                PrescriptionProfit += (int)(c.NormalTotalPointValue + c.NormalMedicineUseValue + c.ChronicTotalPointValue + c.ChronicmedicineUseValue);
                PrescribeProfit += (int)(c.PayselfAdjustValue + c.PayselfMedUseValue);
            }
        }
        #endregion
    }
}
