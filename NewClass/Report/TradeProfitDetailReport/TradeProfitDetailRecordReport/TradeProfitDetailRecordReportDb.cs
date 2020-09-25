﻿using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport
{
    public static class TradeProfitDetailRecordReportDb
    {
        public static DataTable GetDataByDate(int Id, DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Id", Id);
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[POS].[TradeProfitDetailRecordByDate]", parameterList);
        }
    }
}