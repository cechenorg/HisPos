using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Cooperative.CooperativeClinicSetting
{
    public static class CooperativeClinicSettingDb
    {
        public static DataTable Init()
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CooperativeClinic]", parameterList);
        }

        public static void Update(CooperativeClinicSettings cooperativeClinicSetting)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CooperClinic", SetCooperativeClinic(cooperativeClinicSetting));
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCooperativeClinic]", parameterList);
        }

        #region TableSet

        public static DataTable SetCooperativeClinic(CooperativeClinicSettings cooperativeClinicSettings)
        {
            DataTable cooperativeClinicTable = CooperativeClinicTable();
            foreach (var c in cooperativeClinicSettings)
            {
                DataRow newRow = cooperativeClinicTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "CooCli_ID", c.CooperavieClinic.ID);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_Type", c.TypeName);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_NorIsBuckle", c.NormalIsBuckle);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_NorWareHouseID", c.NormalWareHouse.ID);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_ChiIsBuckle", c.ChronicIsBuckle);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_ChiWareHouseID", c.ChronicWareHouse.ID);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_IsPurge", c.IsPurge);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_FolderPath", c.FilePath);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_AutoPrint", c.AutoPrint);
                cooperativeClinicTable.Rows.Add(newRow);
            }
            return cooperativeClinicTable;
        }

        public static DataTable CooperativeClinicTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("CooCli_ID", typeof(string));
            masterTable.Columns.Add("CooCli_Type", typeof(string));
            masterTable.Columns.Add("CooCli_NorIsBuckle", typeof(bool));
            masterTable.Columns.Add("CooCli_NorWareHouseID", typeof(int));
            masterTable.Columns.Add("CooCli_ChiIsBuckle", typeof(bool));
            masterTable.Columns.Add("CooCli_ChiWareHouseID", typeof(int));
            masterTable.Columns.Add("CooCli_IsPurge", typeof(bool));
            masterTable.Columns.Add("CooCli_FolderPath", typeof(string));
            masterTable.Columns.Add("CooCli_AutoPrint", typeof(string));
            return masterTable;
        }

        #endregion TableSet
    }
}