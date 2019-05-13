using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting.SplitProduct {
    public static class SplitProductDb {
        public static DataTable GetProductInventoryByProID(string proID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ProID", proID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductInventoryByProID]", parameterList);   
        }
        public static void SplitProductInventory(string proID,SplitProducts splitProducts) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ProID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "SplitList", SetSplitListTable(splitProducts)); 
            DataBaseFunction.AddSqlParameter(parameterList, "EmpID", ViewModelMainWindow.CurrentUser.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[SplitProductInventory]", parameterList);
        }

        #region TableSet
        public static DataTable SplitListTable() {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("WarID", typeof(int));
            masterTable.Columns.Add("Amount", typeof(double));  
            return masterTable;
        }
        public static DataTable SetSplitListTable(SplitProducts splitProducts) {  
            DataTable splitListTable = SplitListTable();
            foreach (var s in splitProducts)
            {
                DataRow newRow = splitListTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "WarID", s.WarID);
                DataBaseFunction.AddColumnValue(newRow, "Amount", s.SplitAmount);
                splitListTable.Rows.Add(newRow);
            }
            return splitListTable;
        }
        #endregion
    }
}
