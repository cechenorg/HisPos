﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine
{
    public static class MedicineDb
    {
        public static DataTable GetMedicinesBySearchId(string medicineID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            Database.DataBaseFunction.AddSqlParameter(parameterList, "Pro_Id", medicineID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineBySearchId]", parameterList);     
        }
        public static void InsertCooperativeMedicineOTC(string medicineID,string medicineName)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            Database.DataBaseFunction.AddSqlParameter(parameterList, "MedId", medicineID);
            Database.DataBaseFunction.AddSqlParameter(parameterList, "Name", medicineName);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCooperativeMedicineOTC]", parameterList);
        } 
    }
}
