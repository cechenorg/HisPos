using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.RewardDetailRecordReport
{
    public class RewardDetailRecordReports : ObservableCollection<RewardDetailRecordReport>
    {
        public RewardDetailRecordReports()
        {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = RewardDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new RewardDetailRecordReport(r));
            }
        }
    }
}