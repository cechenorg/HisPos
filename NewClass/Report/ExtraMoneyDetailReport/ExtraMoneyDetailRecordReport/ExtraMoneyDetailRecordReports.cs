using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.ExtraMoneyDetailRecordReport
{
    public class ExtraMoneyDetailRecordReports : ObservableCollection<ExtraMoneyDetailRecordReport>
    {
        public ExtraMoneyDetailRecordReports()
        {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = ExtraMoneyDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new ExtraMoneyDetailRecordReport(r));
            }
        }
    }
}