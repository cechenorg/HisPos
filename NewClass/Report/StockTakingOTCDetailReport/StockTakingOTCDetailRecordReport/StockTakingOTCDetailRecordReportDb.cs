﻿using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingOTCDetailRecordReport
{
    public static class StockTakingOTCDetailRecordReportDb
    {
        public static DataTable GetDataByDate(string Id, DateTime sDate, DateTime eDate,string type)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Id", Id);
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "type", type);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockTakingDetailRecordByDate]", parameterList);
        }
    }
}
