using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.MedicineSet
{
    public static class MedicineSetDb
    {
        public static DataTable GetData()
        { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicineSet]");
        }
        public static DataTable GetMedicineSets()//取得所有藥品組合
        {
            return MainWindow.ServerConnection.ExecuteProc("");
        }
        public static DataTable GetMedicineSetDetail(int setID)//取得藥品組合內容
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "", setID);
            return MainWindow.ServerConnection.ExecuteProc("", parameterList);
        }
        public static void UpdateMedicineSet(MedicineSet set)//更新藥品組合
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "", set.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "", set.Name);

            MainWindow.ServerConnection.ExecuteProc("", parameterList);
        }
        public static DataTable DeleteMedicineSet(int setID)//刪除藥品組合
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "", setID);
            return MainWindow.ServerConnection.ExecuteProc("", parameterList);
        }
    }
}
