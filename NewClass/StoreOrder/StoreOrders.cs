using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

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
            var tempOrder =  this.SingleOrDefault(s => s.OrderStatus == OrderStatusEnum.DONE);

            if (tempOrder != null)
                Remove(tempOrder);
        }

        internal void UpdateSingdeOrderStatus(DataTable dataTable)
        {
            foreach (var storeOrder in Items)
            {
                if (storeOrder.OrderStatus == OrderStatusEnum.WAITING)
                {
                    DataRow[] dataRows = dataTable.Select("ORDER_ID = '" + storeOrder.ID + "'");

                    if (dataRows.Length > 0)
                        storeOrder.UpdateOrderDataFromSingde(dataRows[0]);
                }
            }
        }

        internal static void AddNewOrdersFromSingde(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                DataTable table = StoreOrderDB.AddNewStoreOrderFromSingde(row);

                if(table != null && table.Rows.Count > 0)
                    StoreOrderDB.UpdateSingdeStoreOrderSyncFlagByID(row.Field<string>("sht_no"));
            }
        }
    }
}
