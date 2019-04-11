using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CashFlow;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.NewClass.Report.PrescriptionProfitReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport {
    public class CashStockEntryReportViewModel : TabBase {
        #region Variables
        public override TabBase getTab() {
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
        private CashReport totalCashFlow = new CashReport();
        public CashReport TotalCashFlow
        {
            get => totalCashFlow;
            set
            {
                Set(() => TotalCashFlow, ref totalCashFlow, value);
            }
        }
        private CashReports cashflowCollection = new CashReports();
        public CashReports CashflowCollection
        {
            get => cashflowCollection;
            set
            {
                Set(() => CashflowCollection, ref cashflowCollection, value);
            }
        }
        private PrescriptionProfitReports selfPrescriptionProfitReportCollection = new PrescriptionProfitReports();
        public PrescriptionProfitReports SelfPrescriptionProfitReportCollection
        {
            get => selfPrescriptionProfitReportCollection;
            set
            {
                Set(() => SelfPrescriptionProfitReportCollection, ref selfPrescriptionProfitReportCollection, value);
            }
        }
        private PrescriptionProfitReports cooperativePrescriptionProfitReportCollection = new PrescriptionProfitReports();
        public PrescriptionProfitReports CooperativePrescriptionProfitReportCollection
        {
            get => cooperativePrescriptionProfitReportCollection;
            set
            {
                Set(() => CooperativePrescriptionProfitReportCollection, ref cooperativePrescriptionProfitReportCollection, value);
            }
        }
        public PrescriptionProfitReports TotalPrescriptionProfitReportCollection { get; set; } = new PrescriptionProfitReports();

        private PrescriptionProfitReport selfPrescriptionProfitReport = new PrescriptionProfitReport();
        public PrescriptionProfitReport SelfPrescriptionProfitReport
        {
            get => selfPrescriptionProfitReport;
            set
            {
                Set(() => SelfPrescriptionProfitReport, ref selfPrescriptionProfitReport, value);
            }
        }
        private PrescriptionProfitReport cooperativePrescriptionProfitReport = new PrescriptionProfitReport();
        public PrescriptionProfitReport CooperativePrescriptionProfitReport
        {
            get => cooperativePrescriptionProfitReport;
            set
            {
                Set(() => CooperativePrescriptionProfitReport, ref cooperativePrescriptionProfitReport, value);
            }
        }
        private PrescriptionProfitReport totalPrescriptionProfitReport = new PrescriptionProfitReport();
        public PrescriptionProfitReport TotalPrescriptionProfitReport
        {
            get => totalPrescriptionProfitReport;
            set
            {
                Set(() => TotalPrescriptionProfitReport, ref totalPrescriptionProfitReport, value);
            }
        }

        #endregion
        #region Command
        public RelayCommand SearchCommand { get; set; }
        #endregion
        public CashStockEntryReportViewModel() {
            SearchCommand = new RelayCommand(SearchAction);
           
            GetData(); 
        }
        #region Action
        private void SearchAction() {
            GetData();
        }
        private void GetData() {
            CashflowCollection.GetDataByDate(StartDate, EndDate);
            TotalPrescriptionProfitReportCollection.GetDataByDate(StartDate, EndDate);
            SelfPrescriptionProfitReportCollection.Clear();
            CooperativePrescriptionProfitReportCollection.Clear();
            foreach (var r in TotalPrescriptionProfitReportCollection) {
                if (r.TypeId.Length == 1)
                    SelfPrescriptionProfitReportCollection.Add(r);
                else
                    CooperativePrescriptionProfitReportCollection.Add(r);
            }
            CaculateTotalCashFlow();
            CaculateTotalPrescriptionProfit();
            CaculateSelfPrescriptionProfit();
            CaculateCooperativePrescriptionProfit();
        }
        private void CaculateTotalCashFlow() {
            
            TotalCashFlow.CopayMentPrice = CashflowCollection.Sum(c => c.CopayMentPrice);
            TotalCashFlow.PaySelfPrice = CashflowCollection.Sum(c => c.PaySelfPrice);
            TotalCashFlow.AllPaySelfPrice = CashflowCollection.Sum(c => c.AllPaySelfPrice);
            TotalCashFlow.DepositPrice = CashflowCollection.Sum(c => c.DepositPrice);
            TotalCashFlow.OtherPrice = CashflowCollection.Sum(c => c.OtherPrice);
            TotalCashFlow.TotalPrice = CashflowCollection.Sum(c => c.TotalPrice); 
        }
        private void CaculateTotalPrescriptionProfit() {

            foreach (var r in TotalPrescriptionProfitReportCollection)
            {
                TotalPrescriptionProfitReport.Count += r.Count;
                TotalPrescriptionProfitReport.MedicalServicePoint += r.MedicalServicePoint;
               TotalPrescriptionProfitReport.MedicinePoint += r.MedicinePoint;
               TotalPrescriptionProfitReport.PaySelfPoint += r.PaySelfPoint;
               TotalPrescriptionProfitReport.MedUse += r.MedUse;
                TotalPrescriptionProfitReport.Profit += r.Profit;
            } 
        }
        private void CaculateSelfPrescriptionProfit()
        {
            foreach (var r in SelfPrescriptionProfitReportCollection) {
                SelfPrescriptionProfitReport.Count += r.Count;
                SelfPrescriptionProfitReport.MedicalServicePoint += r.MedicalServicePoint;
                SelfPrescriptionProfitReport.MedicinePoint += r.MedicinePoint;
                SelfPrescriptionProfitReport.PaySelfPoint += r.PaySelfPoint;
                SelfPrescriptionProfitReport.MedUse  += r.MedUse;
                SelfPrescriptionProfitReport.Profit += r.Profit;
            }
            
        }
        private void CaculateCooperativePrescriptionProfit()
        {
            foreach (var r in CooperativePrescriptionProfitReportCollection)
            {
                CooperativePrescriptionProfitReport.Count += r.Count;
                CooperativePrescriptionProfitReport.MedicalServicePoint += r.MedicalServicePoint;
                CooperativePrescriptionProfitReport.MedicinePoint += r.MedicinePoint;
                CooperativePrescriptionProfitReport.PaySelfPoint += r.PaySelfPoint;
                CooperativePrescriptionProfitReport.MedUse += r.MedUse;
                CooperativePrescriptionProfitReport.Profit += r.Profit;
            }
           
        }
        #endregion
    }
}
