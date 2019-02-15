using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockValue.StockEntry {
   public class StockEntrys : Collection<StockEntry> {
        public StockEntrys() { }
        public void GetDataByDate(DateTime date) {
            var table = StockEntryDb.GetDataByDate(date);
            foreach (DataRow r in table.Rows) {
                Add(new StockEntry(r));
            }
        }
    }
}
