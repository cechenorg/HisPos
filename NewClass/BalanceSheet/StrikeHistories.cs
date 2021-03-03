using His_Pos.NewClass.Report.CashReport;
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
    }
}