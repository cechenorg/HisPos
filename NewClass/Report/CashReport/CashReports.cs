using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.CashReport
{
    public class CashReports : ObservableCollection<CashReport>
    {
        public CashReports()
        {
        }

        public CashReports(DateTime sDate, DateTime eDate)
        {
            GetDataByDate(sDate, eDate);
        }

        public void GetDataByDate(DateTime sDate, DateTime eDate)
        {
            DataTable table = CashReportDb.GetDataByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new CashReport(r));
            }
        }
    }
}