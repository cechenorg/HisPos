using InfraStructure.SQLService.SQLServer.StoreOrder;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class SingdeTotalOrders : Collection<SingdeTotalOrder>
    {
        private SingdeTotalOrders(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new SingdeTotalOrder(row));
            }
        }

        internal static SingdeTotalOrders GetSingdeTotalOrders()
        {
            StoreOrderService storeOrderService = new StoreOrderService(Properties.Settings.Default.SQL_localWithDB);
            return new SingdeTotalOrders(StoreOrderDB.GetSingdeTotalOrders());

            //return new SingdeTotalOrders(storeOrderService.Get_SingdeTotalOrdersNotDone());
        }
    }
}