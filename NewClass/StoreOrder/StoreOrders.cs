using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrders : ObservableCollection<StoreOrder>
    {
        private StoreOrders(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                switch (row.Field<string>("StoOrd_Type"))
                {
                    case "P":
                        Add(new PurchaseOrder(row));
                        break;
                    case "R":
                        Add(new ReturnOrder(row));
                        break;
                }

            }
        }

        public static StoreOrders GetOrdersNotDone()
        {
            return new StoreOrders(StoreOrderDB.GetNotDoneStoreOrders());
        }

        public void ReloadCollection()
        {
            
        }
    }
}
