using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.IncomeStatement;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport;
using System;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.IncomeStatement
{
    public class IncomeStatementViewModel : TabBase
    {
        private int year;

        public int Year
        {
            get => year;
            set
            {
                Set(() => Year, ref year, value);
            }
        }

        private string yearString;

        public string YearString
        {
            get => yearString;
            set
            {
                Set(() => YearString, ref yearString, value);
                var result = int.TryParse(value, out var parsedYear);
                Year = result ? parsedYear : -1;
            }
        }

        private PrescriptionCountMatrix prescriptionCountMatrix;

        public PrescriptionCountMatrix PrescriptionCountMatrix
        {
            get => prescriptionCountMatrix;
            set
            {
                Set(() => PrescriptionCountMatrix, ref prescriptionCountMatrix, value);
            }
        }

        private PointLostMatrix pointLostMatrix;

        public PointLostMatrix PointLostMatrix
        {
            get => pointLostMatrix;
            set
            {
                Set(() => PointLostMatrix, ref pointLostMatrix, value);
            }
        }

        private PharmacyIncomeMatrix pharmacyIncomeMatrix;

        public PharmacyIncomeMatrix PharmacyIncomeMatrix
        {
            get => pharmacyIncomeMatrix;
            set
            {
                Set(() => PharmacyIncomeMatrix, ref pharmacyIncomeMatrix, value);
            }
        }

        private ProfitSummaryMatrix chronicProfitMatrix;

        public ProfitSummaryMatrix ChronicProfitMatrix
        {
            get => chronicProfitMatrix;
            set
            {
                Set(() => ChronicProfitMatrix, ref chronicProfitMatrix, value);
            }
        }

        private ProfitSummaryMatrix prescribeProfitMatrix;

        public ProfitSummaryMatrix PrescribeProfitMatrix
        {
            get => prescribeProfitMatrix;
            set
            {
                Set(() => PrescribeProfitMatrix, ref prescribeProfitMatrix, value);
            }
        }

        private ProfitSummaryMatrix hisProfitMatrix;

        public ProfitSummaryMatrix HISProfitMatrix
        {
            get => hisProfitMatrix;
            set
            {
                Set(() => HISProfitMatrix, ref hisProfitMatrix, value);
            }
        }

        private IncomeStatementMatrix incomeStatementMatrix;

        public IncomeStatementMatrix IncomeStatementMatrix
        {
            get => incomeStatementMatrix;
            set
            {
                Set(() => IncomeStatementMatrix, ref incomeStatementMatrix, value);
            }
        }

        private CostAndInventoryMatrix costAndInventoryMatrix;

        public CostAndInventoryMatrix CostAndInventoryMatrix
        {
            get => costAndInventoryMatrix;
            set
            {
                Set(() => CostAndInventoryMatrix, ref costAndInventoryMatrix, value);
            }
        }

        public override TabBase getTab()
        {
            return this;
        }

        public RelayCommand Search { get; set; }
        public RelayCommand ExportIncomeStatementCommand { get; set; }

        public IncomeStatementViewModel()
        {
            Year = DateTime.Today.Year;
            YearString = Year.ToString();
            Search = new RelayCommand(SearchAction);
            ExportIncomeStatementCommand = new RelayCommand(ExportIncomeStatementAction);
            SearchAction();
        }

        private void SearchAction()
        {
            if (Year >= 2019)
            {
                GetData();
            }
            else if (Year >= 108 && Year <= 200)
            {
                Year += 1911;
                GetData();
            }
            else
            {
                MessageWindow.ShowMessage("年份超出範圍或格式錯誤", MessageType.ERROR);
            }
        }

        private void GetData()
        {
            MainWindow.ServerConnection.OpenConnection();
            var incomeStatementDataSet = NewClass.Report.CashReport.CashReportDb.GetYearIncomeStatementForExport(Year);
            PrescriptionCountMatrix = new PrescriptionCountMatrix(incomeStatementDataSet.Tables[6], incomeStatementDataSet.Tables[0]);
            PharmacyIncomeMatrix = new PharmacyIncomeMatrix(incomeStatementDataSet.Tables[1], incomeStatementDataSet.Tables[2], incomeStatementDataSet.Tables[7]);
            ChronicProfitMatrix = new ProfitSummaryMatrix("慢箋營業毛利", PharmacyIncomeMatrix.GetChronicProfits());
            IncomeStatementMatrix = new IncomeStatementMatrix(incomeStatementDataSet);
            PrescribeProfitMatrix = new ProfitSummaryMatrix("調劑營業毛利", IncomeStatementMatrix.GetPrescribeProfits());
            CostAndInventoryMatrix = new CostAndInventoryMatrix(incomeStatementDataSet.Tables[5]);
            HISProfitMatrix = new ProfitSummaryMatrix("調劑台營業毛利", IncomeStatementMatrix.GetHISProfits());
            PointLostMatrix = new PointLostMatrix(incomeStatementDataSet.Tables[8]);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void ExportIncomeStatementAction()
        {
            if (Year >= 2019)
            {
                ExportIncomeStatementSheet();
            }
            else if (Year >= 108 && Year <= 200)
            {
                Year += 1911;
                ExportIncomeStatementSheet();
            }
            else
            {
                MessageWindow.ShowMessage("年份超出範圍或格式錯誤", MessageType.ERROR);
            }
        }

        private void ExportIncomeStatementSheet()
        {
            ExportIncomeStatementWindow exportIncomeStatementWindow = new ExportIncomeStatementWindow(Year);
            exportIncomeStatementWindow.ShowDialog();
        }
    }
}