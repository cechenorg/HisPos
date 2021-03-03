using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport
{
    public class TradeProfitDetailRecordReports : ObservableCollection<TradeProfitDetailRecordReport>
    {
        public TradeProfitDetailRecordReports()
        {
        }

        public void GetDateByDate(int typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = TradeProfitDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailRecordReport(r));
            }
        }
    }
}