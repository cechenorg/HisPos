using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Medicine.ControlMedicineEdit
{
    public class ControlMedicineEditDb
    {
        internal static DataTable GetData(string medID,string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "medID", medID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineEditByMedID]", parameterList);
        }
    }
}
