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

namespace His_Pos.Class
{
    public static class ChronicDb
    {
        internal static void CaculateChironic() { //假設病人1-3沒領  要幫他算出2-1~2-3
            var dd = new DbConnection(Settings.Default.SQL_global);
            dd.ExecuteProc("[HIS_POS_DB].[Index].[CaculateChironic]");
        }
        internal static ObservableCollection<Chronic> GetChronicDeclareById(string cusId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            ObservableCollection<Chronic> chronics = new ObservableCollection<Chronic>();
            parameters.Add(new SqlParameter("CUS_ID", cusId));
           DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetChronicDeclareById]",parameters);
            foreach (DataRow row in dataTable.Rows) {
                chronics.Add(new Chronic(row));
            }
            return chronics;
        }
        internal static bool CheckChronicExistById(string cusId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            ObservableCollection<Chronic> chronics = new ObservableCollection<Chronic>();
            parameters.Add(new SqlParameter("CUS_ID", cusId));
            DataTable dataTable = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[CheckChronicExistById]", parameters);
            if (dataTable.Rows[0][0].ToString() == "0")
                return false;
            else
                return true;
        }

         
    }
}
