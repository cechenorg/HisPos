using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StockValue {
    public class StockValues : ObservableCollection<StockValue> {
        public StockValues() {
        }
        public void GetDataByDate(DateTime startDate,DateTime endDate) {
            DataTable table = StockValueDb.GetDataByDate(startDate,endDate);
            foreach (DataRow r in table.Rows) {
                Add(new StockValue(r));
            }
        }

    }
}
