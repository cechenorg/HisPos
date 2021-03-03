using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StockValue.StockEntry
{
    public class StockEntrys : Collection<StockEntry>
    {
        public StockEntrys()
        {
        }

        public void GetDataByDate(DateTime date)
        {
            var table = StockEntryDb.GetDataByDate(date);
            foreach (DataRow r in table.Rows)
            {
                Add(new StockEntry(r));
            }
        }
    }
}