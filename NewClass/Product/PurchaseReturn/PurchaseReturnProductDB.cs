using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
