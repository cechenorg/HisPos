using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.StockTakingReport
{
    public class StockTakingReports : ObservableCollection<StockTakingReport>
    {
        public StockTakingReports()
        {
        }

        public StockTakingReports(DateTime sDate, DateTime eDate)
        {
            GetDataByDate(sDate, eDate);
        }

        public void GetDataByDate(DateTime sDate, DateTime eDate)
        {
            Clear();
            DataTable table = StockTakingReportDb.GetDataByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTakingReport(r));
            }
            //DataTable pointEditTable = PrescriptionProfitReportDb.GetPrescriptionPointEditRecordByDates(sDate, eDate);
            //if (pointEditTable.Rows.Count > 0)
            //{
            //    var editDataRow = pointEditTable.Rows[0];
            //    var editProfit = this.SingleOrDefault(p => p.TypeId.Equals("5"));
            //    editProfit?.CountEditPoint(editDataRow);
            //}
        }
    }
}