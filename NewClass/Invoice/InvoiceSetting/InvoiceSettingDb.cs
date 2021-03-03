using System.Data;

namespace His_Pos.NewClass.Invoice.InvoiceSetting
{
    public static class InvoiceSettingDb
    {
        public static DataTable Init()
        {
            /*  List<SqlParameter> parameterList = new List<SqlParameter>();
             return MainWindow.ServerConnection.ExecuteProc("[Get].[CooperativeClinic]", parameterList);*/
            return null;
        }

        public static void Update(InvoiceSettings invoiceSetting)
        {
            /*  List<SqlParameter> parameterList = new List<SqlParameter>();
              DataBaseFunction.AddSqlParameter(parameterList, "CooperClinic", SetCooperativeClinic(cooperativeClinicSetting));
              MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCooperativeClinic]", parameterList);*/
        }

        #region TableSet

        public static DataTable SetCooperativeClinic(InvoiceSettings cooperativeClinicSettings)
        {
            /*DataTable cooperativeClinicTable = CooperativeClinicTable();
            foreach (var c in cooperativeClinicSettings)
            {
                DataRow newRow = cooperativeClinicTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "CooCli_ID",c.CooperavieClinic.ID);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_Type", c.TypeName);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_NorIsBuckle", c.NormalIsBuckle);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_NorWareHouseID", c.NormalWareHouse.ID);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_ChiIsBuckle", c.ChronicIsBuckle);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_ChiWareHouseID", c.ChronicWareHouse.ID);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_IsPurge", c.IsPurge);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_FolderPath", c.FilePath);
                cooperativeClinicTable.Rows.Add(newRow);
            }
            return cooperativeClinicTable; */
            return null;
        }

        public static DataTable CooperativeClinicTable()
        {
            /*DataTable masterTable = new DataTable();
            masterTable.Columns.Add("CooCli_ID", typeof(string));
            masterTable.Columns.Add("CooCli_Type", typeof(string));
            masterTable.Columns.Add("CooCli_NorIsBuckle", typeof(bool));
            masterTable.Columns.Add("CooCli_NorWareHouseID", typeof(int));
            masterTable.Columns.Add("CooCli_ChiIsBuckle", typeof(bool));
            masterTable.Columns.Add("CooCli_ChiWareHouseID", typeof(int));
            masterTable.Columns.Add("CooCli_IsPurge", typeof(bool));
            masterTable.Columns.Add("CooCli_FolderPath", typeof(string));
            return masterTable;*/
            return null;
        }

        #endregion TableSet
    }
}