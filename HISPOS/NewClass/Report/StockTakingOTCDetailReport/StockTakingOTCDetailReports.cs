using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.StockTakingDetailReport
{
    public class StockTakingOTCDetailReports : ObservableCollection<StockTakingOTCDetailReport>
    {
        public StockTakingOTCDetailReports(string typeId, DateTime sDate, DateTime eDate)
        {
            GetDataByDate(typeId, sDate, eDate);
        }
        public StockTakingOTCDetailReports(DataTable a)
        {
            GetDataByDate(a);
        }
        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = StockTakingOTCDetailReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingOTCDetailReport(r));
            }
        }
        public void GetDataByDate(DataTable a)
        {
            DataTable table = a;
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingOTCDetailReport(r));
            }
        }
    }
}