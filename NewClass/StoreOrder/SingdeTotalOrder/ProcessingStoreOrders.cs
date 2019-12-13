using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
