using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Medicine.ControlMedicineDeclare
{
    public static class ControlMedicineDeclareDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineDeclareByDate]", parameterList);
        }

        public static DataTable GetInventoryDataByDate(DateTime sDate, DateTime eDate, string inv)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "inv", inv);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineInventoryByDate]", parameterList);
        }

        public static DataTable GetUsageDataByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SDATE", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EDATE", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineUsageByDate]", parameterList);
        }
    }
}