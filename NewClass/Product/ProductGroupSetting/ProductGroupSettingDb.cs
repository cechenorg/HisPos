using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public static class ProductGroupSettingDb
    {
        public static DataTable GetDataByID(string proID,string warID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "proID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductGroupSettingByProID]", parameterList);
        }
        public static void MergeProductGroup(ProductGroupSettings productGroupSettings, string warID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetPrescriptionDetail(productGroupSettings));
            DataBaseFunction.AddSqlParameter(parameterList, "EmpID", ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[MergeProductInventory]", parameterList);
        } 
        public static DataTable MedicineListTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("MedicineID", typeof(string));
            return masterTable;
        }
        public static DataTable SetPrescriptionDetail(ProductGroupSettings productGroupSettings)
        { //一般藥費
            DataTable medicineListTable = MedicineListTable();
            foreach (var m in productGroupSettings)
            {
                DataRow newRow = medicineListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "MedicineID", m.ID);
                medicineListTable.Rows.Add(newRow);
            }
            return medicineListTable;
        }
    }
}
