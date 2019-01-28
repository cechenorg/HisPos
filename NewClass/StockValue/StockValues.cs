using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockValue {
    public class StockValues : Collection<StockValue> {
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
