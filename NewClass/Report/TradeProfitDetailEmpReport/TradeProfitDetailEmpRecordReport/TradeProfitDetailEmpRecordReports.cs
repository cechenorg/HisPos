using His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailEmpReport.TradeProfitDetailEmpRecordReport
{
    public class TradeProfitDetailEmpRecordReports : ObservableCollection<TradeProfitDetailEmpRecordReport>
    {
        public TradeProfitDetailEmpRecordReports() {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate) {
            DataTable table = TradeProfitDetailEmpRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailEmpRecordReport(r));
            }
        }
    }
}
