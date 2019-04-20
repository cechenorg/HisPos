using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail
{
    public static class IndexReserveDetailDb
    {
        public static DataTable GetDataByDate(int Id) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Id", Id); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[IndexReserveDetailById]", parameterList);
        }
    }
}
