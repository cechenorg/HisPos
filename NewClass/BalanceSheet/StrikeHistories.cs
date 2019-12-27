using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Report.CashReport;

namespace His_Pos.NewClass.BalanceSheet
{
    public class StrikeHistories : Collection<StrikeHistory>
    {
        public StrikeHistories()
        {

        }

        public void GetData()
        {
            var historiesTable = CashReportDb.GetStrikeHistories();
            foreach (DataRow r in historiesTable.Rows)
            {
                Add(new StrikeHistory(r));
            }
        }
    }
}
