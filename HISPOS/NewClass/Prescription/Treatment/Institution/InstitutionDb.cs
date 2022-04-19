using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public static class InstitutionDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Institution]");
        }

        public static void InsertInstitution(string id,string name,string address ,string phone,string pharmacyName )
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "InsID", id);
            DataBaseFunction.AddSqlParameter(parameterList, "InsName", name);
            DataBaseFunction.AddSqlParameter(parameterList, "InsAddress", address ?? "");
            DataBaseFunction.AddSqlParameter(parameterList, "InsPhone", phone ?? "");
            DataBaseFunction.AddSqlParameter(parameterList, "Ins_InsertPharmacy", pharmacyName);
            
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertInstitution]", parameterList); 
        }

        public static DataTable GetCommonInstitution()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CommonInstitution]");
        }

        public static void UpdateUsedTime(string insId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "InsId", insId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InstitutionUsedTime]", parameterList);
        }

        public static DataTable CheckDivisionValid(string institutionID, string divisionId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "InsId", institutionID);
            DataBaseFunction.AddSqlParameter(parameterList, "DivId", divisionId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckDivisionIsValid]", parameterList);
        }

        public static DataTable GetEnableDivisions(string institutionID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "InsId", institutionID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EnableDivisions]", parameterList);
        }

        public static DataTable GetAdjustedInstitutions()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[AdjustedInstitutions]");
        }
    }
}