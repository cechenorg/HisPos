using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class ProcessingStoreOrders : Collection<ProcessingStoreOrder>
    {
        private ProcessingStoreOrders(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProcessingStoreOrder(row));
            }
        }

        internal static ProcessingStoreOrders GetProcessingStoreOrdersByDate(string date)
        {
            return new ProcessingStoreOrders(StoreOrderDB.GetProcessingStoreOrdersByDate(date));
        }
    }
}