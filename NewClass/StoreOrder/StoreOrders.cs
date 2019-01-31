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

        internal void UpdateSingdeOrderStatus(DataTable dataTable)
        {
            foreach (var storeOrder in Items)
            {
                if (storeOrder.OrderStatus == OrderStatusEnum.WAITING)
                {
                    DataRow[] dataRows = dataTable.Select("ORDER_ID = " + storeOrder.ID);

                    if (dataRows.Length > 0)
                        storeOrder.UpdateOrderDataFromSingde(dataRows[0]);
                }
            }
        }
    }
}
