using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Product
{
    public class ProductDB
    {
        internal static DataTable GetProductStructsBySearchString(string searchString, string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductStructBySearchString]", parameters);
        }

        internal static DataTable GetPurchaseProductStructCountBySearchString(string searchString)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[PurchaseProductStructCountBySearchString]", parameters);
        }

        internal static DataTable GetReturnProductStructCountBySearchString(string searchString, string wareID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));
            parameters.Add(new SqlParameter("WAREID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReturnProductStructCountBySearchString]", parameters);
        }
    }
}
