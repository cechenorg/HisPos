using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Medicine.ControlMedicineDetail
{
    public static class ControlMedicineDetailDb
    {
        public static DataTable GetDataById(string medId, DateTime sDate, DateTime eDate, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "MedID", medId);
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineDetailByMedId]", parameterList);
        }

        public static DataTable GetDeclareData(DateTime sDate, DateTime eDate, List<string> warIDs)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warIDs", SetWarTable(warIDs));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineDeclareFileByDate]", parameterList);
        }

        #region TableSet

        public static DataTable WarTable()
        {
            DataTable warTable = new DataTable();
            warTable.Columns.Add("ID", typeof(int));
            return warTable;
        }

        public static DataTable SetWarTable(List<string> wars)
        {
            DataTable warTable = WarTable();
            foreach (var w in wars)
            {
                DataRow newRow = warTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "ID", w);

                warTable.Rows.Add(newRow);
            }
            return warTable;
        }

        #endregion TableSet
    }
}