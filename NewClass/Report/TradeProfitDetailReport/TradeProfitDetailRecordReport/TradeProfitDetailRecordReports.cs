using His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport
{
    public class TradeProfitDetailRecordReports : ObservableCollection<TradeProfitDetailRecordReport>
    {
        public TradeProfitDetailRecordReports() {
        }

        public void GetDateByDate(int typeId, DateTime sDate, DateTime eDate) {
            DataTable table = TradeProfitDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailRecordReport(r));
            }
        }
    }
}
