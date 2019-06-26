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
    public static class ProductGroupSettingDB
    {
        internal static DataTable GetProductGroupSettingsByID(string proID, string wareID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PRO_ID", proID);
            DataBaseFunction.AddSqlParameter(parameterList, "WARE_ID", wareID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductGroupSettingsByProID]", parameterList);
        }
    }
}
