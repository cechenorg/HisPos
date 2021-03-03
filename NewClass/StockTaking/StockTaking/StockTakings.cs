using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTaking
{
    public class StockTakings : ObservableCollection<StockTaking>
    {
        public StockTakings()
        {
        }

        public StockTakings(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Add(new StockTaking(r));
            }
        }

        public StockTakings GetStockTakingByCondition(DateTime? sDate, DateTime? eDate, string proID, string proName)
        {
            return new StockTakings(StockTakingDB.GetStockTakingRecordByCondition(sDate, eDate, proID, proName));
        }
    }
}