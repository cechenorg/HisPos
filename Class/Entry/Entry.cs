using System.Data;

namespace His_Pos.Class.Entry {
   public class Entry {
        public Entry(DataRow row) {
            Date = row["ENTRY_DATE"].ToString();
            EntryName = row["ENTRY_NAME"].ToString();
            EntryValue = row["ENTRY_VALUE"].ToString();
        } 
        public string Date { get; set; }
        public string EntryName { get; set; }
        public string EntryValue { get; set; }
    }
    
}
