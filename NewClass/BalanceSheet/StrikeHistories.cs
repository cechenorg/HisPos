using His_Pos.NewClass.Report.CashReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

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

        public void GetSelectData(DataTable dt)
        {
            Clear();
            foreach (DataRow r in dt.Rows)
            {
                Add(new StrikeHistory(r));
            }
            
        }
    }
}