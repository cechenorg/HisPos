﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePrescription
{
    public class DeclarePrescriptionDb
    {
        public static DataTable GetDeclarePrescriptionsByMonthRange(DateTime start,DateTime end)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", end);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[DeclarePrescriptionsByMonthRange]", parameterList);
        }
        public static void UpdateDeclareFileID(int fileID,List<int> preIDList)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "DeclareId", fileID);
            DataBaseFunction.AddSqlParameter(parameterList, "GeneralStringTable", SetIDTable(preIDList));
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateDeclareFileID]", parameterList);
        } 
        private static DataTable IDTable() {
            DataTable idTable = new DataTable();
            idTable.Columns.Add("ID", typeof(string));
            return idTable;
        }
        public static DataTable SetIDTable(List<int> preIDList) {
            DataTable table = IDTable();
            foreach (int id in preIDList) {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "ID", id);
                table.Rows.Add(newRow);
            } 
            return table;
        }
    } 
}
