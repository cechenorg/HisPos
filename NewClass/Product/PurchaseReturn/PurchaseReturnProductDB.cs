using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.ChromeTabViewModel;

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

        internal static DataTable GetPurchaseProductByProductID(string iD)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", iD));

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

        internal static DataTable GetReturnProductByProductID(string iD)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", iD));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReturnProductByProductID]", parameters);
        }
        
        public static DataTable GetReturnProductBatchNumbers(string productID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductReturnBatchNumbers]", parameters);
        }
    }
}
