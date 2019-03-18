using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.ControlMedicineDetail
{
    public static class ControlMedicineDetailDb
    {

        public static DataTable GetDataById(string medId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "MedID", medId);  
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineDetailByMedId]", parameterList);
        }
    }

}
