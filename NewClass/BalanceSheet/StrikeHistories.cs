using His_Pos.NewClass.Report.CashReport;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.BalanceSheet
{
    public class StrikeHistories : ObservableCollection<StrikeHistory>
    {
        public StrikeHistories()
        {
        }

        public void GetData()
        {
            Clear();
            var historiesTable = CashReportDb.GetStrikeHistories();
            foreach (DataRow r in historiesTable.Rows)
            {
                Add(new StrikeHistory(r));
            }
        }
        public void GetSelectData(string type, DateTime sdate, DateTime edate)
        {
            Clear();
            var historiesTable = CashReportDb.GetSelectStrikeHistories( type,  sdate,  edate);
            foreach (DataRow r in historiesTable.Rows)
            {
                Add(new StrikeHistory(r));
            }
        }
    }
}