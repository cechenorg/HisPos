using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public static class InstitutionDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Institution]");
        }
        public static DataTable GetCommonInstitution() {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CommonInstitution]"); 
        }
        public static void UpdateUsedTime(string insId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "InsId", insId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InstitutionUsedTime]", parameterList); 
        }

        public static DataTable CheckDivisionValid(string institutionId, string divisionId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "InsId", institutionId);
            DataBaseFunction.AddSqlParameter(parameterList, "DivId", divisionId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckDivisionEnable]", parameterList);
        }
    }
}
