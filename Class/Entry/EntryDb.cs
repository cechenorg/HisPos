using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Entry {
    public static class EntryDb {
        internal static ObservableCollection<Entry> GetEntryDetailByDate(string Date) {
            ObservableCollection<Entry> Collection = new ObservableCollection<Entry>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DATE",DateTimeExtensions.ToUsDate(Date)));
            var table = dbConnection.ExecuteProc("[HIS_POS_DB].[EntrySearchView].[GetEntryDetailByDate]",parameters);
            foreach (DataRow row in table.Rows)
            {
                Collection.Add(new Entry(row));
            }
            return Collection;
        }
    }
}
