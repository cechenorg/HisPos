using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report;
using His_Pos.NewClass.Report.CashDetailReport;
using His_Pos.NewClass.Report.CashDetailReport.CashDetailRecordReport;
using His_Pos.NewClass.Report.CashReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport;
using His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot;
using His_Pos.NewClass.Report.PrescriptionProfitReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using His_Pos.NewClass.Prescription.Service;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport {
    public class CashStockEntryReportViewModel : TabBase {
        #region Variables
        public override TabBase getTab() {
            return this;
        }
        private List<string> adjustCaseString;
        public List<string> AdjustCaseString
        {
            get => adjustCaseString;
            set
            {
                Set(() => AdjustCaseString, ref adjustCaseString, value);
            }
        }
        private string adjustCaseSelectItem = "全部";
        public string AdjustCaseSelectItem
        {
            get => adjustCaseSelectItem;
            set
            {
                Set(() => AdjustCaseSelectItem, ref adjustCaseSelectItem, value);
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
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
        private CashReports cashflowCollection;
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
        private PrescriptionProfitReport cooperativePrescriptionSelectedItem;
        public PrescriptionProfitReport CooperativePrescriptionSelectedItem
        {
            get => cooperativePrescriptionSelectedItem;
            set
            {
                Set(() => CooperativePrescriptionSelectedItem, ref cooperativePrescriptionSelectedItem, value);
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
        private PrescriptionDetailReport prescriptionDetailReportSum;
        public PrescriptionDetailReport PrescriptionDetailReportSum
        {
            get => prescriptionDetailReportSum;
            set
            {
                Set(() => PrescriptionDetailReportSum, ref prescriptionDetailReportSum, value);
            }
        }
        private PrescriptionDetailReports prescriptionDetailReportCollection;
        public PrescriptionDetailReports PrescriptionDetailReportCollection
        {
            get => prescriptionDetailReportCollection;
            set
            {
                Set(() => PrescriptionDetailReportCollection, ref prescriptionDetailReportCollection, value);
            }
        }
        private CollectionViewSource prescriptionDetailReportViewSource;
        private CollectionViewSource PrescriptionDetailReportViewSource
        {
            get => prescriptionDetailReportViewSource;
            set
            {
                Set(() => PrescriptionDetailReportViewSource, ref prescriptionDetailReportViewSource, value); 
            }
        }

        private ICollectionView prescriptionDetailReportView;
        public ICollectionView PrescriptionDetailReportView
        {
            get => prescriptionDetailReportView;
            private set
            {
                Set(() => PrescriptionDetailReportView, ref prescriptionDetailReportView, value);
            }
        }
        private PrescriptionDetailReport prescriptionDetailReportSelectItem;
        public PrescriptionDetailReport PrescriptionDetailReportSelectItem
        {
            get => prescriptionDetailReportSelectItem;
            set
            {
                Set(() => PrescriptionDetailReportSelectItem, ref prescriptionDetailReportSelectItem, value);
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
        private CashDetailReport cashDetailReportSelectItem;
        public CashDetailReport CashDetailReportSelectItem
        {
            get => cashDetailReportSelectItem;
            set
            {
                Set(() => CashDetailReportSelectItem, ref cashDetailReportSelectItem, value);
            }
        }
        private CashDetailRecordReports cashDetailRecordReportCollection = new CashDetailRecordReports();
        public CashDetailRecordReports CashDetailRecordReportCollection
        {
            get => cashDetailRecordReportCollection;
            set
            {
                Set(() => CashDetailRecordReportCollection, ref cashDetailRecordReportCollection, value);
            }
        }
        private PrescriptionDetailMedicineRepots  prescriptionDetailMedicineRepotCollection = new PrescriptionDetailMedicineRepots();
        public PrescriptionDetailMedicineRepots PrescriptionDetailMedicineRepotCollection
        {
            get => prescriptionDetailMedicineRepotCollection;
            set
            {
                Set(() => PrescriptionDetailMedicineRepotCollection, ref prescriptionDetailMedicineRepotCollection, value);
            }
        }
        private PrescriptionDetailMedicineRepot prescriptionDetailMedicineRepotSelectItem ;
        public PrescriptionDetailMedicineRepot PrescriptionDetailMedicineRepotSelectItem
        {
            get => prescriptionDetailMedicineRepotSelectItem;
            set
            {
                Set(() => PrescriptionDetailMedicineRepotSelectItem, ref prescriptionDetailMedicineRepotSelectItem, value);
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
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                Set(() => IsBusy, ref _isBusy, value);
            }
        }
        private string _busyContent;

        public string BusyContent
        {
            get => _busyContent;
            set
            {
                Set(() => BusyContent, ref _busyContent, value);
            }
        }
        #endregion
        #region Command
        public RelayCommand SelfPrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CooperativePrescriptionSelectionChangedCommand { get; set; }
        public RelayCommand CashSelectionChangedCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand CashDetailClickCommand { get; set; }
        public RelayCommand PrescriptionDetailClickCommand { get; set; }
        public RelayCommand PrescriptionDetailDoubleClickCommand { get; set; }
        public RelayCommand PrescriptionDetailMedicineDoubleClickCommand { get; set; }
        public RelayCommand PrintCashPerDayCommand { get; set; }
        public RelayCommand PrintPrescriptionProfitDetailCommand { get; set; }
        public RelayCommand ExportIncomeStatementCommand { get; set; }
        #endregion
        public CashStockEntryReportViewModel() {
            SearchCommand = new RelayCommand(SearchAction);
            SelfPrescriptionSelectionChangedCommand = new RelayCommand(SelfPrescriptionSelectionChangedAction);
            CooperativePrescriptionSelectionChangedCommand = new RelayCommand(CooperativePrescriptionSelectionChangedAction);
            CashSelectionChangedCommand = new RelayCommand(CashSelectionChangedAction);
            CashDetailClickCommand = new RelayCommand(CashDetailClickAction);
            PrescriptionDetailClickCommand = new RelayCommand(PrescriptionDetailClickAction);
            PrescriptionDetailDoubleClickCommand = new RelayCommand(PrescriptionDetailDoubleClickAction);
            PrescriptionDetailMedicineDoubleClickCommand = new RelayCommand(PrescriptionDetailMedicineDoubleClickAction);
            PrintCashPerDayCommand = new RelayCommand(PrintCashPerDayAction);
            PrintPrescriptionProfitDetailCommand = new RelayCommand(PrintPrescriptionProfitDetailAction);
            ExportIncomeStatementCommand = new RelayCommand(ExportIncomeStatementAction);
            GetData();
            InitCollection();
        }
        #region Action
        private void ExportIncomeStatementAction()
        {
            ExportIncomeStatementWindow exportIncomeStatementWindow = new ExportIncomeStatementWindow();
            exportIncomeStatementWindow.ShowDialog();
        }
        private void PrintPrescriptionProfitDetailAction()
        {
            if (PrescriptionDetailReportView is null) return;

            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "處方毛利明細";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = StartDate.ToString("yyyyMMdd") + "_" + EndDate.ToString("yyyyMMdd") + "處方毛利明細";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {  
                        file.WriteLine("姓名,醫療院所,藥服(估),藥品,自費,耗用,毛利");
                        double sumMedicalServicePoint = 0;
                        double sumMedicalPoint = 0;
                        double sumPaySelfPoint = 0;
                        double sumMeduse = 0;
                        double sumProfit = 0;
                        foreach (var c in PrescriptionDetailReportView)
                        {
                            PrescriptionDetailReport presc = (PrescriptionDetailReport)c;
                            file.WriteLine($"{presc.CusName},{presc.InsName},{presc.MedicalServicePoint},{presc.MedicalPoint},{presc.PaySelfPoint},{presc.Meduse},{presc.Profit}");
                            sumMedicalServicePoint += presc.MedicalServicePoint;
                            sumMedicalPoint += presc.MedicalPoint;
                            sumPaySelfPoint += presc.PaySelfPoint;
                            sumMeduse += presc.Meduse;
                            sumProfit += presc.Profit;
                        } 
                        file.WriteLine($"合計,,{sumMedicalServicePoint},{sumMedicalPoint},{sumPaySelfPoint},{sumMeduse},{sumProfit}");
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        private void InitCollection() {
            AdjustCaseString = new List<string>() { "全部", "一般箋", "慢箋", "自費調劑" }; 
        }
        private void PrintCashPerDayAction() {
            if (CashflowSelectedItem is null) 
                return;  
            DataTable table = CashReportDb.GetPerDayDataByDate(StartDate, EndDate, CashflowSelectedItem.TypeId);
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "金流存檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = DateTime.Today.ToString("yyyyMMdd") + ViewModelMainWindow.CurrentPharmacy.Name + "_" +  CashflowSelectedItem.TypeName + "金流存檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        file.WriteLine("日期,部分負擔,自費,自費調劑,押金,其他,總計");
                        int cCopayMentPrice = 0;
                        int cPaySelfPrice = 0;
                        int cAllPaySelfPrice = 0;
                        int cDepositPrice = 0;
                        int cOtherPrice = 0;
                        int cTotalPrice = 0;
                        foreach (DataRow s in table.Rows)
                        {
                            file.WriteLine($"{s.Field<DateTime>("date").AddYears(-1911).ToString("yyy/MM/dd")}," +
                                $"{s.Field<int>("CopayMentPrice")},{s.Field<int>("PaySelfPrice")},{s.Field<int>("AllPaySelfPrice")}" +
                                $",{s.Field<int>("DepositPrice")},{s.Field<int>("OtherPrice")},{s.Field<int>("TotalPrice")}");

                            cCopayMentPrice += s.Field<int>("CopayMentPrice");
                            cPaySelfPrice += s.Field<int>("PaySelfPrice");
                            cAllPaySelfPrice += s.Field<int>("AllPaySelfPrice");
                            cDepositPrice += s.Field<int>("DepositPrice");
                            cOtherPrice += s.Field<int>("OtherPrice");
                            cTotalPrice += s.Field<int>("TotalPrice");
                        }
                        file.WriteLine($"總計,{cCopayMentPrice},{cPaySelfPrice},{cAllPaySelfPrice},{cDepositPrice},{cOtherPrice},{cTotalPrice}");
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }

            }
        }
        private void PrescriptionDetailMedicineDoubleClickAction()
        {
            if (PrescriptionDetailMedicineRepotSelectItem is null) return;
            ProductDetailWindow.ShowProductDetailWindow();
            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { PrescriptionDetailMedicineRepotSelectItem.Id, "0" }, "ShowProductDetail"));
        } 
        private void PrescriptionDetailDoubleClickAction() {
            if (PrescriptionDetailReportSelectItem is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionService.ShowPrescriptionEditWindow(PrescriptionDetailReportSelectItem.Id); 
        }
        private void PrescriptionDetailClickAction() {
            if (PrescriptionDetailReportSelectItem is null)
            {
                PrescriptionDetailMedicineRepotCollection.Clear();
                return;
            }
            PrescriptionDetailMedicineRepotCollection.GerDataById(PrescriptionDetailReportSelectItem.Id);
        }
        private void CashDetailClickAction() {
            if (CashDetailReportSelectItem is null) {
                CashDetailRecordReportCollection.Clear();
                return;
            } 
            CashDetailRecordReportCollection.GetDateByDate(CashDetailReportSelectItem.Id,StartDate,EndDate);
        }
        private void CashSelectionChangedAction() {
            if (CashflowSelectedItem is null)
            {
                CashDetailReportCollection.Clear();
                return;
            }
            CashStockEntryReportEnum = CashStockEntryReportEnum.Cash;
            CashDetailReportCollection.GetDataByDate(CashflowSelectedItem.TypeId, StartDate, EndDate);
            CooperativePrescriptionSelectedItem = null;
            SelfPrescriptionSelectedItem = null;
        }
        private void CooperativePrescriptionSelectionChangedAction()
        {

            if (SelfPrescriptionSelectedItem is null && CooperativePrescriptionSelectedItem is null) { 
                PrescriptionDetailReportCollection.Clear();
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
            }
            if (CooperativePrescriptionSelectedItem is null)
                return; 
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;

            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                PrescriptionDetailReportCollection = new PrescriptionDetailReports(CooperativePrescriptionSelectedItem.TypeId, StartDate, EndDate);
                
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync(); 
            SelfPrescriptionSelectedItem = null;
            CashflowSelectedItem = null;
        }
        private void SelfPrescriptionSelectionChangedAction() {
            if (SelfPrescriptionSelectedItem is null && CooperativePrescriptionSelectedItem is null) {
                PrescriptionDetailReportCollection.Clear();
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
            }
               
            if (SelfPrescriptionSelectedItem is null)
                return; 
            CashStockEntryReportEnum = CashStockEntryReportEnum.Prescription;
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                PrescriptionDetailReportCollection = new PrescriptionDetailReports(SelfPrescriptionSelectedItem.TypeId, StartDate, EndDate);
                  
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                PrescriptionDetailReportViewSource = new CollectionViewSource { Source = PrescriptionDetailReportCollection };
                PrescriptionDetailReportView = PrescriptionDetailReportViewSource.View;
                PrescriptionDetailReportViewSource.Filter += AdjustCaseFilter;
                SumPrescriptionDetailReport();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
            CooperativePrescriptionSelectedItem = null;
            CashflowSelectedItem = null;
        }
        private void SearchAction() { 
                GetData(); 
        }
        private void GetData() {
            TotalPrescriptionProfitReportCollection.Clear();
            SelfPrescriptionProfitReportCollection.Clear();
            CooperativePrescriptionProfitReportCollection.Clear();
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                MainWindow.ServerConnection.OpenConnection();
                BusyContent = "報表查詢中";
                CashflowCollection = new CashReports(StartDate,EndDate);
                TotalPrescriptionProfitReportCollection.GetDataByDate(StartDate, EndDate);
               
                MainWindow.ServerConnection.CloseConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            { 
                foreach (var r in TotalPrescriptionProfitReportCollection)
                {
                    if (r.TypeId.Length == 1)
                        SelfPrescriptionProfitReportCollection.Add(r);
                    else
                        CooperativePrescriptionProfitReportCollection.Add(r);
                }
                CaculateTotalCashFlow();
                CaculateTotalPrescriptionProfit();
                CaculateSelfPrescriptionProfit();
                CaculateCooperativePrescriptionProfit();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
             
        }
        private void SumPrescriptionDetailReport() {
            PrescriptionDetailReportSum = new PrescriptionDetailReport();
            PrescriptionDetailReportSum.InsName = "總計"; 
            var tempCollection = PrescriptionDetailReportCollection.Where(p => true);
            switch (AdjustCaseSelectItem) {
                case "一般箋":
                    tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "1" || p.AdjustCaseID == "3");
                    break;
                case "慢箋":
                    tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "2" );
                    break;                                                                               
                case "自費調劑":                                                                         
                    tempCollection = PrescriptionDetailReportCollection.Where(p => p.AdjustCaseID == "0" );
                    break;
            }
            PrescriptionDetailReportSum.MedicalPoint = tempCollection.Sum(s => s.MedicalPoint);
            PrescriptionDetailReportSum.MedicalServicePoint = tempCollection.Sum(s => s.MedicalServicePoint);
            PrescriptionDetailReportSum.PaySelfPoint = tempCollection.Sum(s => s.PaySelfPoint);
            PrescriptionDetailReportSum.Meduse = tempCollection.Sum(s => s.Meduse);
            PrescriptionDetailReportSum.Profit = tempCollection.Sum(s => s.Profit);
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
        private void AdjustCaseFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is PrescriptionDetailReport src))
                e.Accepted = false;

            e.Accepted = false;

            PrescriptionDetailReport indexitem = ((PrescriptionDetailReport)e.Item);
            if (AdjustCaseSelectItem == "一般箋" && (indexitem.AdjustCaseID == "1" || indexitem.AdjustCaseID == "3"))
                e.Accepted = true;
            else if (AdjustCaseSelectItem == "慢箋" && indexitem.AdjustCaseID == "2")
                e.Accepted = true;
            else if (AdjustCaseSelectItem == "自費調劑" && indexitem.AdjustCaseID == "0")
                e.Accepted = true;
            else if (AdjustCaseSelectItem == "全部")
                e.Accepted = true;
        }
        #endregion
    }
}
