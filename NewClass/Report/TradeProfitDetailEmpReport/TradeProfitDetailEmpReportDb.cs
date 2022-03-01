using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.TradeProfitDetailEmpReport
{
    public static class TradeProfitDetailEmpReportDb
    {
        public static DataTable GetDataByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "typeId", typeId);
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[POS].[TradeProfitDetailEmpReportByDate]", parameterList);
        }
    }
}