using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.StoreOrder.SingdeTotalOrder
{
    public class SingdeTotalOrder : ObservableObject
    {
        #region ----- Define Variables -----

        public string Date { get; set; }
        public int PurchaseCount { get; set; }
        public int ReturnCount { get; set; }
        public double PurchasePrice { get; set; }
        public double ReturnPrice { get; set; }
        public ProcessingStoreOrders StoreOrders { get; set; }

        public bool IsAllDone
        {
            get
            {
                if (StoreOrders is null) return false;
                return StoreOrders.Count(s => s.Status == OrderStatusEnum.SINGDE_PROCESSING) == 0;
            }
        }

        public double Total
        {
            get
            {
                return PurchasePrice - ReturnPrice;
            }
        }

        #endregion ----- Define Variables -----

        public SingdeTotalOrder(DataRow dataRow)
        {
            Date = dataRow.Field<string>("DATE");
            PurchaseCount = dataRow.Field<int>("P_COUNT");
            ReturnCount = dataRow.Field<int>("R_COUNT");
            PurchasePrice = (double)dataRow.Field<decimal>("P_TOTAL");
            ReturnPrice = (double)dataRow.Field<decimal>("R_TOTAL");
        }

        #region ----- Define Functions -----

        internal void GetProcessingOrders()
        {
            StoreOrders = ProcessingStoreOrders.GetProcessingStoreOrdersByDate(Date);
        }

        internal void OrderToDone(string id)
        {
            foreach (var order in StoreOrders)
            {
                if (order.ID == id)
                {
                    order.Status = OrderStatusEnum.DONE;

                    DataTable result = new DataTable();

                    switch (order.Type)
                    {
                        case OrderTypeEnum.PURCHASE:
                            result = StoreOrderDB.PurchaseStoreOrderToDone(id, false);
                            break;

                        case OrderTypeEnum.RETURN:
                            result = StoreOrderDB.ReturnStoreOrderToDone(id);
                            break;
                    }

                    if (result.Rows.Count == 0 || result.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                        MessageWindow.ShowMessage((order.Type == OrderTypeEnum.PURCHASE ? "進" : "退") + "貨單未完成\r\n請重新整理後重試", MessageType.ERROR);
                    break;
                }
            }

            RaisePropertyChanged(nameof(IsAllDone));
        }

        internal void AllOrderToDone()
        {
            foreach (var order in StoreOrders)
            {
                if (order.Status == OrderStatusEnum.SINGDE_PROCESSING)
                {
                    order.Status = OrderStatusEnum.DONE;

                    DataTable result = new DataTable();

                    switch (order.Type)
                    {
                        case OrderTypeEnum.PURCHASE:
                            result = StoreOrderDB.PurchaseStoreOrderToDone(order.ID, false);
                            break;

                        case OrderTypeEnum.RETURN:
                            result = StoreOrderDB.ReturnStoreOrderToDone(order.ID);
                            break;
                    }

                    if (result.Rows.Count == 0 || result.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                        MessageWindow.ShowMessage((order.Type == OrderTypeEnum.PURCHASE ? "進" : "退") + "貨單未完成\r\n請重新整理後重試", MessageType.ERROR);
                }
            }

            RaisePropertyChanged(nameof(IsAllDone));
        }

        #endregion ----- Define Functions -----
    }
}