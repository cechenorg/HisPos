using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.Class.Entry {
    public static class EntryDb {
        internal static ObservableCollection<Entry> GetEntryDetailByDate(string Date) {
            ObservableCollection<Entry> Collection = new ObservableCollection<Entry>();
            var dbConnection = new DatabaseConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DATE",Convert.ToDateTime(Date).AddYears(1911) ));
            var table = dbConnection.ExecuteProc("[HIS_POS_DB].[EntrySearchView].[GetEntryDetailByDate]",parameters);
            foreach (DataRow row in table.Rows)
            {
                Collection.Add(new Entry(row));
            }
            return Collection;
        }
    }
}
