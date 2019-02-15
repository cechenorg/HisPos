﻿using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.CooperativeClinicJson {
    public static class CooperativeClinicJsonDb {
        public static void UpdateCooperAdjustMedcinesStatus() {
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCooperAdjustStatus]");
        }
        public static DataTable GetCooperAdjust() {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CooperAdjust]");
        }
        public static DataTable GetCooperAdjustMedicines(string preId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreMas_Id", preId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CooperAdjustMedicines]", parameterList); 
        }
        public static void InsertCooperJson(string json) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Content", json);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCooperJson]", parameterList);
        } 
    }
}
