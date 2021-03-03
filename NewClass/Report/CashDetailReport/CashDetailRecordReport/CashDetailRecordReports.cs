using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.CashDetailReport.CashDetailRecordReport
{
    public class CashDetailRecordReports : ObservableCollection<CashDetailRecordReport>
    {
        public CashDetailRecordReports()
        {
        }

        public void GetDateByDate(int typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = CashDetailRecordReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new CashDetailRecordReport(r));
            }
        }
    }
}