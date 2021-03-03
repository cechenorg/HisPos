using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingOTCDetailRecordReport
{
    public class StockTakingOTCDetailRecordReports : ObservableCollection<StockTakingOTCDetailRecordReport>
    {
        public StockTakingOTCDetailRecordReports()
        {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate, string type)
        {
            DataTable table = StockTakingOTCDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate, type);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingOTCDetailRecordReport(r));
            }
        }
    }
}