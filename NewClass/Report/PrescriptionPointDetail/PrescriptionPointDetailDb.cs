using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionPointDetail
{
    public static class PrescriptionPointDetailDb
    {
        public static DataTable GetDataByDate(DateTime Date,string cooperativeInsId )
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "date", Date);
            DataBaseFunction.AddSqlParameter(parameterList, "cooperativeInsId", cooperativeInsId); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionPointDetailByDate]", parameterList);
        }
         
    }
}
