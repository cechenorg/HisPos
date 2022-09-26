using His_Pos.Database;
using His_Pos.NewClass.Manufactory.ManufactoryManagement;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Manufactory
{
    public class ManufactoryDB
    {
        #region ----- Define DataTable -----

        public static DataTable ManufactoryPrincipalTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ManPri_ID", typeof(int));
            dataTable.Columns.Add("ManPri_Name", typeof(string));
            dataTable.Columns.Add("ManPri_Telephone", typeof(string));
            dataTable.Columns.Add("ManPri_Fax", typeof(string));
            dataTable.Columns.Add("ManPri_LINE", typeof(string));
            dataTable.Columns.Add("ManPri_Email", typeof(string));
            dataTable.Columns.Add("ManPri_Note", typeof(string));
            return dataTable;
        }

        #endregion ----- Define DataTable -----

        #region ----- Set DataTable -----

        #region ///// ManufactoryPrincipalTable /////

        public static DataTable SetManufactoryPrincipal(ManufactoryPrincipals principals)
        {
            DataTable manufactoryPrincipalTable = ManufactoryPrincipalTable();

            if (principals != null) 
            {
                foreach (var principal in principals)
                {
                    DataRow newRow = manufactoryPrincipalTable.NewRow();
                    DataBaseFunction.AddColumnValue(newRow, "ManPri_ID", principal.ID);
                    DataBaseFunction.AddColumnValue(newRow, "ManPri_Name", principal.Name);
                    DataBaseFunction.AddColumnValue(newRow, "ManPri_Telephone", principal.Telephone);
                    DataBaseFunction.AddColumnValue(newRow, "ManPri_Fax", principal.Fax);
                    DataBaseFunction.AddColumnValue(newRow, "ManPri_LINE", principal.Line);
                    DataBaseFunction.AddColumnValue(newRow, "ManPri_Email", principal.Email);
                    DataBaseFunction.AddColumnValue(newRow, "ManPri_Note", principal.Note);
                    manufactoryPrincipalTable.Rows.Add(newRow);
                }
            }

            return manufactoryPrincipalTable;
        }

        #endregion ///// ManufactoryPrincipalTable /////

        #endregion ----- Set DataTable -----

        internal static DataTable GetAllManufactories()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Manufactory]");
        }

        internal static DataTable DeleteManufactory(string manufactoryID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manufactoryID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteManufactoryByManufactoryID]", parameters);
        }

        internal static DataTable UpdateManufactoryDetail(ManufactoryManageDetail manufactoryManageDetail)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameters, "MAN_ID", manufactoryManageDetail.ID);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_NAME", manufactoryManageDetail.Name);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_NICKNAME", manufactoryManageDetail.NickName);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_RESNAME", manufactoryManageDetail.ResponsibleName);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_RESTEL", manufactoryManageDetail.ResponsibleTelephone);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_RESLINE", manufactoryManageDetail.ResponsibleLINE);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_TEL", manufactoryManageDetail.Telephone);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_FAX", manufactoryManageDetail.Fax);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_EIN", manufactoryManageDetail.EIN);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_EMAIL", manufactoryManageDetail.Email);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_MEDID", manufactoryManageDetail.MedicalID);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_CONTROLID", manufactoryManageDetail.ControlMedicineID);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_ADDR", manufactoryManageDetail.Address);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_NOTE", manufactoryManageDetail.Note);

            DataBaseFunction.AddSqlParameter(parameters, "MAN_PRINCIPALS", SetManufactoryPrincipal(manufactoryManageDetail.Principals));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateManufactoryDetailByManufactoryID]", parameters);
        }

        internal static DataTable ManufactoryHasControlMedicineID(string manufactoryID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manufactoryID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ManufactoryHasControlMedicineID]", parameters);
        }

        internal static DataTable GetManufactoryTradeRecords(string manufactoryID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manufactoryID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ManufactoryTradeRecordsByManufactoryID]", parameters);
        }

        internal static DataTable GetManufactoryPrincipals(string manufactoryID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manufactoryID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ManufactoryPrincipalsByManufactoryID]", parameters);
        }

        internal static DataTable AddNewManufactory(string manufactoryName, string manufactoryNickName, string manufactoryTelephone, string manufactoryAddress)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_NAME", manufactoryName));
            DataBaseFunction.AddSqlParameter(parameters, "MAN_NICKNAME", manufactoryNickName);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_TEL", manufactoryTelephone);
            DataBaseFunction.AddSqlParameter(parameters, "MAN_ADDR", manufactoryAddress);

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ManufactoryAddNewManufactory]", parameters);
        }

        internal static DataTable GetManufactoryManageDetailsBySearchCondition(string searchManufactoryName, string searchPrincipalName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_NAME", searchManufactoryName));
            parameters.Add(new SqlParameter("PRINCIPAL_NAME", searchPrincipalName));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ManufactoryManageDetailsBySearchCondition]", parameters);
        }
    }
}