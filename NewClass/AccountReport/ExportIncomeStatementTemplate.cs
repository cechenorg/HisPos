using His_Pos.NewClass.Report.CashReport;
using His_Pos.Service.ExportService;
using System.Data;

namespace His_Pos.NewClass.AccountReport
{
    public class ExportIncomeStatementTemplate : ExportExcelTemplate
    {
        public override string GetSheetName()
        {
            return "年度規劃";
        }

        protected override void CreateExcelSettings()
        {
            var year = (int)Source;
            DataSet dataSet = CashReportDb.GetYearIncomeStatementForExport(year);

            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[0], 2, 2).SetHasHeader(false));
            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[1], 10, 2).SetHasHeader(false));
            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[2], 22, 2).SetHasHeader(false));
            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[3], 28, 2).SetHasHeader(false));
            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[4], 30, 2).SetHasHeader(false));
            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[5], 33, 2).SetHasHeader(false));

            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[6], 4, 1).SetHasHeader(false).SetDataIsBold(true).SetDataFontSize(14));
            Settings.Add(new ExportDataTableExcelSetting(dataSet.Tables[7], 13, 1).SetHasHeader(false));
        }
    }
}