using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StockValue
{
    public class StockValues : ObservableCollection<StockValue>
    {
        public StockValues()
        {
        }

        public void GetDataByDate(DateTime startDate, DateTime endDate, string warID)
        {
            DataTable table = StockValueDb.GetDataByDate(startDate, endDate, warID);
            if (table.Rows.Count < 1)
                return;

            double stock = table.Rows[0].Field<double>("InitStock");
            foreach (DataRow r in table.Rows)
            {
                Add(new StockValue(r, ref stock));
            }
        }

        public void GetOTCDataByDate(DateTime startDate, DateTime endDate, string warID)
        {
            DataTable table = StockValueDb.GetOTCDataByDate(startDate, endDate, warID);
            if (table.Rows.Count < 1)
                return;

            double stock = table.Rows[0].Field<double>("InitStock");
            foreach (DataRow r in table.Rows)
            {
                Add(new StockValue(r, ref stock));
            }
        }
    }
}