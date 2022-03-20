using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.AccountReport.InstitutionDeclarePoint
{
    public static class InstitutionDeclarePointDb
    {
        public static DataTable GetDataByMonth(DateTime FirstDay, DateTime LastDay)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", FirstDay);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", LastDay);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[InstitutionDeclarePointByDate]", parameterList);
        }
    }
}