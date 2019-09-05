using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Medicine.ControlMedicineDetail
{
    public static class ControlMedicineDetailDb
    {

        public static DataTable GetDataById(string medId, DateTime sDate, DateTime eDate,string warID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "MedID", medId);
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineDetailByMedId]", parameterList);
        }
        public static DataTable GetDeclareData( DateTime sDate, DateTime eDate, string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedicineDeclareFileByDate]", parameterList); 
    }
    }

}
