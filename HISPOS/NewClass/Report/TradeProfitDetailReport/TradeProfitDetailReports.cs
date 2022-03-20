using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport
{
    public class TradeProfitDetailReports : ObservableCollection<TradeProfitDetailReport>
    {
        public TradeProfitDetailReports(string typeId, DateTime sDate, DateTime eDate)
        {
            GetDataByDate(typeId, sDate, eDate);
        }
        public TradeProfitDetailReports(DataTable aa)
        {
            GetDataByDate(aa);
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = TradeProfitDetailReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailReport(r));
            }
        }

        public void GetDataByDate(DataTable aa)
        {
            DataTable table = aa;
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailReport(r));
            }
        }
    }
}