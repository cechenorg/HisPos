﻿using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CashFlow;
using His_Pos.NewClass.Report;
using His_Pos.NewClass.Report.CashDetailReport;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport;
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
        private CashReport cashflowSelectedItem = new CashReport();
        public CashReport CashflowSelectedItem
        {
            get => cashflowSelectedItem;
            set
            {
                Set(() => CashflowSelectedItem, ref cashflowSelectedItem, value);
            }
        }
        private PrescriptionProfitReport selfPrescriptionSelectedItem;
        public PrescriptionProfitReport SelfPrescriptionSelectedItem
        {
            get => selfPrescriptionSelectedItem;
            set
            {
                Set(() => SelfPrescriptionSelectedItem, ref selfPrescriptionSelectedItem, value);
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
        private PrescriptionDetailReports prescriptionDetailReportCollection = new PrescriptionDetailReports();
        public PrescriptionDetailReports PrescriptionDetailReportCollection
        {
            get => prescriptionDetailReportCollection;
            set
            {
                Set(() => PrescriptionDetailReportCollection, ref prescriptionDetailReportCollection, value);
            }
        }
        private CashDetailReports cashDetailReportCollection = new CashDetailReports();
        public CashDetailReports CashDetailReportCollection
        {
            get => cashDetailReportCollection;
            set
            {
                Set(() => CashDetailReportCollection, ref cashDetailReportCollection, value);
            }
        }

        private CashStockEntryReportEnum cashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
        public CashStockEntryReportEnum CashStockEntryReportEnum
        {
            get => cashStockEntryReportEnum;
            set
            {
                Set(() => CashStockEntryReportEnum, ref cashStockEntryReportEnum, value);
            }
        }
        #endregion
        #region Command
        public RelayCommand PrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CashSelectionChangedCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        #endregion
        public CashStockEntryReportViewModel() {
            SearchCommand = new RelayCommand(SearchAction);
            PrescriptionSelectionChangedCommand = new RelayCommand(PrescriptionSelectionChangedAction);
            CashSelectionChangedCommand = new RelayCommand(CashSelectionChangedAction);
            GetData(); 
        }
        #region Action
        private void CashSelectionChangedAction() {
            if (CashflowSelectedItem is null)
            {
                CashDetailReportCollection.Clear();
                return;
            }
            CashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
            CashDetailReportCollection.GetDataByDate(CashflowSelectedItem.TypeId, StartDate, EndDate); 
        }
        private void PrescriptionSelectionChangedAction() {
            if (SelfPrescriptionSelectedItem is null) {
                PrescriptionDetailReportCollection.Clear();
                return;
            }
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            PrescriptionDetailReportCollection.GetDataByDate(SelfPrescriptionSelectedItem.TypeId,StartDate,EndDate);
        }
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
            TotalPrescriptionProfitReport = new PrescriptionProfitReport();
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
            SelfPrescriptionProfitReport = new PrescriptionProfitReport();
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
            CooperativePrescriptionProfitReport = new PrescriptionProfitReport();
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
