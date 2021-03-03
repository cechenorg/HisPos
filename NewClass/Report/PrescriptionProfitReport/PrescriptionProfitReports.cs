using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionProfitReport
{
    public class PrescriptionProfitReports : ObservableCollection<PrescriptionProfitReport>
    {
        public PrescriptionProfitReports()
        {
        }

        public void GetDataByDate(DateTime sDate, DateTime eDate)
        {
            Clear();
            DataTable table = PrescriptionProfitReportDb.GetDataByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionProfitReport(r));
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