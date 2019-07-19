using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Database;

namespace His_Pos.NewClass.Medicine.ControlMedicineDeclare {
    public static class ControlMedicineDeclareDb {
        public static DataTable GetDataByDate(DateTime sDate,DateTime eDate) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ControlMedDeclareByDate]", parameterList);
        }
    }
}
