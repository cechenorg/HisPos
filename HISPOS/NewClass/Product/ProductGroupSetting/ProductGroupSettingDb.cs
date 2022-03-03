using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public static class ProductGroupSettingDB
    {
        internal static DataTable GetProductGroupSettingsByID(string proID, string wareID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PRO_ID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "WARE_ID", wareID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductGroupSettingsByProID]", parameterList);
        }

        internal static DataTable SplitProduct(string proID, double splitAmount, string wareHouseID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SPL_PRO_ID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "WARE_ID", wareHouseID);
            DataBaseFunction.AddSqlParameter(parameterList, "SPL_AMOUNT", splitAmount);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductSplitInventory]", parameterList);
        }

        internal static DataTable MergeProduct(string proID, string merProID, string wareHouseID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PRO_ID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "MER_PRO_ID", merProID);
            DataBaseFunction.AddSqlParameter(parameterList, "WARE_ID", wareHouseID);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductMergeInventory]", parameterList);
        }
    }
}