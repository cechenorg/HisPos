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
        public static void SplitProductInventory(string proID,int amount,string warID) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ProID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "Amount", amount);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            DataBaseFunction.AddSqlParameter(parameterList, "EmpID", ViewModelMainWindow.CurrentUser.ID);
            MainWindow.ServerConnection.ExecuteProc("[Set].[SplitProductInventory]", parameterList);
        }

        #region TableSet
      
        #endregion
    }
}
