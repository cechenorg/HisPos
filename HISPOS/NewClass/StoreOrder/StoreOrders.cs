using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Drawing.Charts;
using DomainModel.Enum;
using DataTable = System.Data.DataTable;

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

        public StoreOrders(List<StoreOrder> list) : base(list)
        {
        }

        public StoreOrders()
        {
        }

        public static StoreOrders GetOrdersNotDone()
        {
            StoreOrders data = new StoreOrders(StoreOrderDB.GetNotDoneStoreOrders());
            var orderedList = data.OrderBy(_ => _.IsWaitOrder).ThenBy(_ => _.CreateDateTime).ToList();
            StoreOrders result = new StoreOrders();
            for (int i = 0; i < orderedList.Count(); i++)
            {
                result.Add(orderedList[i]);
            }
            return result;
        }
        internal static StoreOrders GetOrdersMinus(string ID)
        {
            return new StoreOrders(StoreOrderDB.GetNotDoneMinus(ID));
        }

        public static StoreOrders GetOrdersDone(DateTime? searchStartDate, DateTime? searchEndDate, string searchOrderID, string searchManufactoryID, string searchProductID, string searchWareName)
        {
            return new StoreOrders(StoreOrderDB.GetDoneStoreOrders(searchStartDate, searchEndDate, searchOrderID, searchManufactoryID, searchProductID, searchWareName));
        }

        public void ReloadCollection()
        {
            var tempOrder = this.SingleOrDefault(s => s.OrderStatus == OrderStatusEnum.DONE);

            //if (tempOrder != null)
                //Remove(tempOrder);
        }

        internal void UpdateSingdeOrderStatus(DataTable dataTable)
        {
            foreach (var storeOrder in Items)
            {
                if (storeOrder.OrderStatus == OrderStatusEnum.WAITING || storeOrder.OrderStatus == OrderStatusEnum.SINGDE_PROCESSING)
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

                if (table != null && table.Rows.Count > 0)
                    StoreOrderDB.UpdateSingdeStoreOrderSyncFlagByID(row.Field<string>("sht_no"));
            }
        }

        internal static void AddNewPrescriptionOrdersFromSingde(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                DataTable table = StoreOrderDB.AddNewPrescriptionOrderFromSingde(row);

                if (table != null && table.Rows.Count > 0)
                    StoreOrderDB.UpdateSingdeStoreOrderSyncFlagByID(row.Field<string>("rx_order"));
            }
        }
    }
}