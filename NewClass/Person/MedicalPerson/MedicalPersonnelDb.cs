using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public static class MedicalPersonnelDb
    {
        public static DataTable GetData()
        { 
           return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicalPersonnels]");
        }

        public static DataTable GetEnableMedicalPersonnels(DateTime start, DateTime end)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StartDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "EndDate", end);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EnablePharmacists]", parameterList);
        }
    }
}
