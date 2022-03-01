using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.CashDetailReport
{
    public class CashDetailReports : ObservableCollection<CashDetailReport>
    {
        public CashDetailReports()
        {
        }

        public CashDetailReports(string typeId, DateTime sDate, DateTime eDate)
        {
            GetDataByDate(typeId, sDate, eDate);
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = CashDetailReportDb.GetDataByDate(typeId, sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new CashDetailReport(r));
            }
        }
    }
}