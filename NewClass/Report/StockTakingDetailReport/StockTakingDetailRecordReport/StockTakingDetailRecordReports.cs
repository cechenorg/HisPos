using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport
{
    public class StockTakingDetailRecordReports : ObservableCollection<StockTakingDetailRecordReport>
    {
        public StockTakingDetailRecordReports()
        {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate, string type, DateTime Time)
        {
            DataTable table = StockTakingDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate, type ,Time);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingDetailRecordReport(r));
            }
        }
    }
}