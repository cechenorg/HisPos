using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockValue.StockEntry {
   public class StockEntry {
        public StockEntry() {

        }
        public StockEntry(DataRow r) {
            Date = r.Field<DateTime>("StoEnt_Time");
            EntryName = r.Field<string>("StoEnt_Name");
            EntryDetail = r.Field<string>("StoEnt_Detail");
            EntryValue = r.Field<decimal>("StoEnt_Value");
        }
        public DateTime Date { get; set; }
        public string EntryName { get; set; }
        public string EntryDetail { get; set; }
        public decimal EntryValue { get; set; }
    }
}
