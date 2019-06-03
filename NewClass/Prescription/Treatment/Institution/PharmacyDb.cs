﻿using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Prescription.Treatment.Institution {
    
    public static class PharmacyDb {
        public static DataTable GetCurrentPharmacy() { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CurrentPharmacy]"); ;
        }
        public static void SetPharmacy(Pharmacy p) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_ID", p.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_Name", p.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_Address", p.Address);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_Telephone", p.Tel);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_ReaderCom", p.ReaderCom);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_VPN", p.VpnIp);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_ReaderIsNew", p.NewReader);
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCurrentPharmacy]", parameterList);
              
        }
        public static void InsertPharmacy(Pharmacy p) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_ID", p.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_Name", p.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_Address", p.Address);
            DataBaseFunction.AddSqlParameter(parameterList, "CurPha_Telephone", p.Tel); 
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCurrentPharmacy]", parameterList);
        }

        public static DataTable GetCurrentPharmacyRecord()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CurrentPharmacyRecord]");
        }
        public static DataTable GroupPharmacySchemaList() {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[GroupPharmacySchemaList]");
        }
        
    }
}
