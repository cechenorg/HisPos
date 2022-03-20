using His_Pos.ChromeTabViewModel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseReturnProductDB
    {
        internal static DataTable GetProductsByStoreOrderID(string orederID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDER_ID", orederID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductByStoreOrderID]", parameters);
        }

        internal static DataTable GetReturnProductsByStoreOrderID(string orederID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDER_ID", orederID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReturnProductByStoreOrderID]", parameters);
        }

        internal static DataTable GetPurchaseProductByProductID(string proID, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[PurchaseProductByProductID]", parameters);
        }

        internal static DataTable GetSingdeProductsByStoreOrderID(string orederID)
        {
            return MainWindow.SingdeConnection.ExecuteProc($"call GetOrderDetail('{orederID}', '{ViewModelMainWindow.CurrentPharmacy.ID}')");
        }

        internal static DataTable GetProductsByStoreOrderIDForExport(string iD)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDER_ID", iD));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductByStoreOrderIDForExport]", parameters);
        }

        internal static DataTable GetReturnProductByProductID(string proID, string wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReturnProductByProductID]", parameters);
        }

        internal static DataTable GetOldReturnProductsByStoreOrderID(string orederID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDER_ID", orederID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[OldReturnProductByStoreOrderID]", parameters);
        }

        internal static DataTable GetProductOnTheWayDetailByID(string proID, int wareID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proID));
            parameters.Add(new SqlParameter("WARE_ID", wareID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductOnTheWayDetailByID]", parameters);
        }
    }
}