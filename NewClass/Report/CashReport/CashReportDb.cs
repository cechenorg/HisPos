using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashReport
{
   public static class CashReportDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashReportByDate]", parameterList);
        }

        internal static DataSet GetYearIncomeStatementForExport(int year)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "YEAR", year);
            return MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[YearIncomeStatementForExport]",parameterList);
        }

        public static DataTable GetPerDayDataByDate(DateTime sDate, DateTime eDate,string insID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "InsID", insID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashReportPerDayByID]", parameterList);
        } 
    }
}
