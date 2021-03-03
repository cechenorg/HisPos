using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.StockTakingDetailReport
{
    public class StockTakingDetailReports : ObservableCollection<StockTakingDetailReport>
    {
        public StockTakingDetailReports(string typeId, DateTime sDate, DateTime eDate)
        {
            GetDataByDate(typeId, sDate, eDate);
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = StockTakingDetailReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingDetailReport(r));
            }
        }
    }
}