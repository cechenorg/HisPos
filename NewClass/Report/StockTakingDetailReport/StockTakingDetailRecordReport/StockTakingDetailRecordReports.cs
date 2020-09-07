using His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport
{
    public class StockTakingDetailRecordReports: ObservableCollection<StockTakingDetailRecordReport>
    {
        public StockTakingDetailRecordReports() {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate) {
            DataTable table = StockTakingDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingDetailRecordReport(r));
            }
        }
    }
}
