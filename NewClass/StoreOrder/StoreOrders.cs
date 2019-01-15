using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrders : Collection<StoreOrder>
    {
        private StoreOrders(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new StoreOrder(row));
            }
        }

        public static StoreOrders GetOrdersNotDone()
        {
            return new StoreOrders(StoreOrderDB.GetNotDoneStoreOrders());
        }
    }
}
