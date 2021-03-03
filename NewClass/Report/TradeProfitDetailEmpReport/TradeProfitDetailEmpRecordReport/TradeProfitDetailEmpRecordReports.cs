using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailEmpReport.TradeProfitDetailEmpRecordReport
{
    public class TradeProfitDetailEmpRecordReports : ObservableCollection<TradeProfitDetailEmpRecordReport>
    {
        public TradeProfitDetailEmpRecordReports()
        {
        }

        public void GetDateByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = TradeProfitDetailEmpRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailEmpRecordReport(r));
            }
        }
    }
}