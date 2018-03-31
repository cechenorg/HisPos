using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Product
{
    public class MedicineDb 
    {
        public static DataTable GetMedicineData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[MEDICINE]");
        }
       
        public static Medicine GetMedDetail(string proId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            Medicine med = null;
            foreach (DataRow row in dd.ExecuteProc("[HIS_POS_DB].[GET].[MEDIMDETAILBYID]", parameters).Rows)
            {
                med = new Medicine(row, DataSource.MEDICINE);
            }
            return med;
        }
    }
}
