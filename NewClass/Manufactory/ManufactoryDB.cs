using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;
using His_Pos.NewClass.Manufactory.ManufactoryManagement;

namespace His_Pos.NewClass.Manufactory
{
    public class ManufactoryDB
    {
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
            throw new NotImplementedException();
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
