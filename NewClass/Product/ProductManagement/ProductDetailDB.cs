using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductDetailDB
    {
        internal static DataTable GetProductManageStructsByConditions(string searchID, string searchName, bool searchIsEnable, bool searchIsInventoryZero)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", searchID));
            parameters.Add(new SqlParameter("PRO_NAME", searchName));
            parameters.Add(new SqlParameter("SHOW_DISABLE", searchIsEnable));
            parameters.Add(new SqlParameter("SHOW_INV_ZERO", searchIsInventoryZero));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageStructBySearchCondition]", parameters);
        }

        internal static DataTable GetProductManageMedicineDataByID(string id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductManageMedicineByID]", parameters);
        }
    }
}
