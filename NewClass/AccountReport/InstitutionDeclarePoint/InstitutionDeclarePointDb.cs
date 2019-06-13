using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.AccountReport.InstitutionDeclarePoint
{
  public static  class InstitutionDeclarePointDb
    {
        public static DataTable GetDataByMonth(DateTime dateTime)
        {
            DateTime FirstDay = new DateTime(dateTime.Year, dateTime.Month, 1);
            DateTime LastDay = new DateTime(dateTime.AddMonths(1).Year, dateTime.AddMonths(1).Month, 1).AddDays(-1);
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", FirstDay);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", LastDay);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[InstitutionDeclarePointByDate]", parameterList);   
    }
    }
}
