using His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.RewardDetailRecordReport
{
    public class RewardDetailRecordReports : ObservableCollection<RewardDetailRecordReport>
    {
        public RewardDetailRecordReports() {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate) {
            DataTable table = RewardDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new RewardDetailRecordReport(r));
            }
        }
    }
}
