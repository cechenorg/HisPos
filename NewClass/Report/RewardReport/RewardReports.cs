using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.RewardReport
{
    public class RewardReports : ObservableCollection<RewardReport>
    {
        public RewardReports()
        {
        }

        public RewardReports(DateTime sDate, DateTime eDate)
        {
            GetDataByDate(sDate, eDate);
        }

        public void GetDataByDate(DateTime sDate, DateTime eDate)
        {
            Clear();
            DataTable table = RewardReportDb.GetDataByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new RewardReport(r));
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