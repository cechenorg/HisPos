using His_Pos.NewClass.Report.CashReport;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.BalanceSheet
{
    public class ClosingHistories : ObservableCollection<ClosingHistory>
    {
        public ClosingHistories()
        {
        }

        public void GetData(DateTime beginDate, DateTime endDate)
        {
            Clear();
            var historiesTable = CashReportDb.GetClosingHistories(beginDate, endDate);
            foreach (DataRow r in historiesTable.Rows)
            {
                Add(new ClosingHistory(r));
            }
        }
    }
}