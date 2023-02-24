using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.StockValue
{
    public static class StockValueDb
    {
        public static void UpdateDailyStockValue()
        {
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateDailyStockValue]");
        }
        public static DataTable GetStockVale(DateTime sdate, DateTime edate)
        {
            DataTable table = new DataTable();
            DataColumn dcID = new DataColumn("ID", typeof(string));
            DataColumn dcName = new DataColumn("Name", typeof(string));
            DataColumn dcValue = new DataColumn("Value", typeof(decimal));
            table.Columns.Add(dcID);
            table.Columns.Add(dcName);
            table.Columns.Add(dcValue);
            DataTable table_med = GetDataByDate(sdate, edate, "0");
            DataTable table_otc = GetOTCDataByDate(sdate, edate, "0");
            if (table_med != null && table_med.Rows.Count > 0)
            {
                DataRow newRow = table.NewRow();
                newRow["ID"] = "006";
                newRow["Name"] = "藥品";
                decimal med_Stock = Convert.ToDecimal(table_med.Rows[0]["InitStock"]) +
                                    Convert.ToDecimal(table_med.Compute("Sum(進貨)", "1=1")) +
                                    Convert.ToDecimal(table_med.Compute("Sum(退貨)", "1=1")) +
                                    Convert.ToDecimal(table_med.Compute("Sum(調劑耗用)", "1=1")) +
                                    Convert.ToDecimal(table_med.Compute("Sum(盤點)", "1=1")) +
                                    Convert.ToDecimal(table_med.Compute("Sum(報廢)", "1=1")) +
                                    Convert.ToDecimal(table_med.Compute("Sum(進貨負庫調整)", "1=1")) +
                                    Convert.ToDecimal(table_med.Compute("Sum(調整)", "1=1"));
                newRow["Value"] = Math.Round(med_Stock, 0);
                table.Rows.Add(newRow);
            }
            if (table_otc != null && table_otc.Rows.Count > 0)
            {
                DataRow newRow = table.NewRow();
                newRow["ID"] = "006";
                newRow["Name"] = "OTC";
                decimal otc_Stock = Convert.ToDecimal(table_otc.Rows[0]["InitStock"]) +
                                    Convert.ToDecimal(table_otc.Compute("Sum(進貨)", "1=1")) +
                                    Convert.ToDecimal(table_otc.Compute("Sum(退貨)", "1=1")) +
                                    Convert.ToDecimal(table_otc.Compute("Sum(調劑耗用)", "1=1")) +
                                    Convert.ToDecimal(table_otc.Compute("Sum(盤點)", "1=1")) +
                                    Convert.ToDecimal(table_otc.Compute("Sum(報廢)", "1=1")) +
                                    Convert.ToDecimal(table_otc.Compute("Sum(進貨負庫調整)", "1=1")) +
                                    Convert.ToDecimal(table_otc.Compute("Sum(調整)", "1=1"));
                newRow["Value"] = Math.Round(otc_Stock, 0);
                table.Rows.Add(newRow);
            }
            return table;
        }

        public static DataTable GetDataByDate(DateTime startDate, DateTime endDate, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", startDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StockChangeReport]", parameterList);
        }

        public static DataTable GetOTCDataByDate(DateTime startDate, DateTime endDate, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", startDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[OTCStockChangeReport]", parameterList);
        }
    }
}