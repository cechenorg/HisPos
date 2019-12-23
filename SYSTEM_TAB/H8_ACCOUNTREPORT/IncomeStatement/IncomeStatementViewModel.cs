using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Report.IncomeStatement;

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
        private PrescriptionCountMatrix prescriptionCountMatrix;
        public PrescriptionCountMatrix PrescriptionCountMatrix
        {
            get => prescriptionCountMatrix;
            set
            {
                Set(() => PrescriptionCountMatrix, ref prescriptionCountMatrix, value);
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

        private ChronicProfitMatrix chronicProfitMatrix;
        public ChronicProfitMatrix ChronicProfitMatrix
        {
            get => chronicProfitMatrix;
            set
            {
                Set(() => ChronicProfitMatrix, ref chronicProfitMatrix, value);
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
        public override TabBase getTab()
        {
            return this;
        }

        public IncomeStatementViewModel()
        {
            var incomeStatementDataSet = NewClass.Report.CashReport.CashReportDb.GetYearIncomeStatementForExport(DateTime.Today.Year);
            PrescriptionCountMatrix = new PrescriptionCountMatrix(incomeStatementDataSet.Tables[6], incomeStatementDataSet.Tables[0]);
            PharmacyIncomeMatrix = new PharmacyIncomeMatrix(incomeStatementDataSet.Tables[1], incomeStatementDataSet.Tables[2], incomeStatementDataSet.Tables[7]);
            ChronicProfitMatrix = new ChronicProfitMatrix(PharmacyIncomeMatrix.GetChronicProfits());
            IncomeStatementMatrix = new IncomeStatementMatrix(incomeStatementDataSet);
        }
    }
}
