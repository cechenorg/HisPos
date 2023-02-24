using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.RewardReport
{
    public class RewardReports : ObservableCollection<RewardReport>
    {
        public RewardReports()
        {
        }

        public RewardReports(string schema, DateTime sDate, DateTime eDate)
        {
            GetDataByDate(schema, sDate, eDate);
        }

        public void GetDataByDate(string schema, DateTime sDate, DateTime eDate)
        {
            Clear();
            DataTable table = RewardReportDb.GetDataByDate(schema ,sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new RewardReport(r));
            }
        }
    }
}