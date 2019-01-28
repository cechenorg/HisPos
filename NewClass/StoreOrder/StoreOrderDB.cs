using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Manufactory;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrderDB
    {
        internal static DataTable RemoveStoreOrderByID(string iD)
        {
            throw new NotImplementedException();
        }

        internal static DataTable GetDonePurchaseOrdersInOneWeek()
        {
            throw new NotImplementedException();
        }

        internal static DataTable AddNewStoreOrder(OrderTypeEnum orderType, Manufactory.Manufactory orderManufactory, int employeeID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE", orderType.ToString()));
            parameters.Add(new SqlParameter("MANUFACTORY", orderManufactory.ID));
            parameters.Add(new SqlParameter("EMPLOYEE", employeeID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[StoreOrderAddNewOrder]", parameters);
        }

        internal static DataTable GetNotDoneStoreOrders()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderNotDone]");
        }

        internal static void SaveReturnOrder(ReturnOrder returnOrder)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("", returnOrder.ID));

            MainWindow.ServerConnection.ExecuteProc("[Set].", parameters);
        }

        internal static void SavePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("", purchaseOrder.ID));

            MainWindow.ServerConnection.ExecuteProc("[Set].", parameters);
        }
    }
}
