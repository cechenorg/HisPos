using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Product;

namespace His_Pos.NewClass.StoreOrder
{
    public class StoreOrderDB
    {
        internal static DataTable RemoveStoreOrderByID(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrder]", parameters);
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

        internal static void UpdateSingdeProductsByStoreOrderID(DataTable dataTable, string orederID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", orederID));
            parameters.Add(new SqlParameter("DETAILS", SetPurchaseOrderDetail(dataTable, orederID)));

            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateSingdeProductsByStoreOrderID]", parameters);
        }

        internal static DataTable AddNewStoreOrderFromSingde(DataRow row)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", row.Field<string>("sht_no")));
            parameters.Add(new SqlParameter("NOTE", row.Field<string>("sht_memo")));
            parameters.Add(new SqlParameter("CREATE_DATE", row.Field<DateTime>("upload_date")));
            parameters.Add(new SqlParameter("DETAILS", SetPurchaseOrderDetail(row.Field<string>("drug_list"), row.Field<string>("sht_no"))));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoreOrderFromSingde]", parameters);
        }

        internal static DataTable GetNotDoneStoreOrders()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderNotDone]");
        }

        internal static void SaveReturnOrder(ReturnOrder returnOrder)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", returnOrder.ID));
            DataBaseFunction.AddSqlParameter(parameters, "STOORD_NOTE", returnOrder.Note);
            parameters.Add(new SqlParameter("STOORD_DETAIL", SetReturnOrderDetail(returnOrder)));

            MainWindow.ServerConnection.ExecuteProc("[Set].[SaveStoreOrder]", parameters);
        }

        internal static void SavePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", purchaseOrder.ID));
            DataBaseFunction.AddSqlParameter(parameters, "STOORD_NOTE", purchaseOrder.Note);
            parameters.Add(new SqlParameter("STOORD_DETAIL", SetPurchaseOrderDetail(purchaseOrder)));

            new SQLServerConnection().ExecuteProc("[Set].[SaveStoreOrder]", parameters);
        }
        public static DataTable InsertPrescriptionOrder(PrescriptionSendDatas prescriptionSendDatas,Prescription.Prescription p) {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            DataBaseFunction.AddSqlParameter(parameterList, "StoreOrderMaster", SetPrescriptionOrderMaster(p));
            DataBaseFunction.AddSqlParameter(parameterList, "StoreOrderDetail", SetPrescriptionOrderDetail(prescriptionSendDatas)); 
            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescriptionStoreOrder]", parameterList); 
        }
        internal static DataTable GetSingdeOrderNewStatus(string dateTime)
        {
            return MainWindow.SingdeConnection.ExecuteProc($"call GetOrderStatus('{ViewModelMainWindow.CurrentPharmacy.Id}', '{dateTime}')");
        }
        public static DataTable RemoveSingdeStoreOrderByID(string storeOrderID)
        {
            return MainWindow.SingdeConnection.ExecuteProc($"call RemoveOrder('{ViewModelMainWindow.CurrentPharmacy.Id}', '{storeOrderID}')");
        }
        public static DataTable UpdateSingdeStoreOrderSyncFlagByID(string storeOrderID) {
            return MainWindow.SingdeConnection.ExecuteProc($"call UpdateStoreOrderSyncFlag('{storeOrderID}', '{ViewModelMainWindow.CurrentPharmacy.Id}')");
        }
        internal static void StoreOrderToDone(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));

            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStoreOrderToDone]", parameters);
        }

        #region TableSet
        public static DataTable SetPrescriptionOrderMaster(Prescription.Prescription p) {
            DataTable storeOrderMasterTable = StoreOrderMasterTable();
            DataRow newRow = storeOrderMasterTable.NewRow(); 
            newRow["StoOrd_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_OrderEmployeeID", ViewModelMainWindow.CurrentUser.ID);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveEmployeeID", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_CreateTime", DateTime.Now);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveTime", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_ManufactoryID", "0");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Status", "W");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Type", "P");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_WarehouseID", "0");
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_Note", null);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_PrescriptionID", p.Id);
            DataBaseFunction.AddColumnValue(newRow, "StoOrd_IsEnable", true);
           
            storeOrderMasterTable.Rows.Add(newRow);
            return storeOrderMasterTable; 
        }

        public static DataTable SetPrescriptionOrderDetail(PrescriptionSendDatas datas)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (var pro in datas)
            {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.MedId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.SendAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "顆");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }

        internal static void StoreOrderToWaiting(string storeOrderID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrderID));

            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateStoreOrderToWaiting]", parameters);
        }

        internal static DataTable SendStoreOrderToSingde(StoreOrder storeOrder)
        {
            string orderMedicines = "";

            if (storeOrder is PurchaseOrder)
            {
                foreach (var product in ((PurchaseOrder)storeOrder).OrderProducts)
                {
                    orderMedicines += product.ID.PadRight(12, ' ');
                    
                    orderMedicines += product.OrderAmount.ToString().PadLeft(10, ' ');

                    orderMedicines += product.Note;
                    orderMedicines += "\r\n";
                }
            }
            else
            {
                foreach (var product in ((ReturnOrder)storeOrder).OrderProducts)
                {
                    orderMedicines += product.ID.PadRight(12, ' ');

                    //12 10 6 20 7
                    //orderMedicines += (-((ITrade)product).Amount).ToString().PadLeft(10, ' ');

                    //orderMedicines += product.Note;
                    orderMedicines += "\r\n";
                }
            }

            return MainWindow.SingdeConnection.ExecuteProc($"call InsertNewOrder('{ViewModelMainWindow.CurrentPharmacy.Id}','{storeOrder.ID}', '{storeOrder.Note}', '{orderMedicines}')");
        }

        internal static DataTable GetNewSingdeOrders()
        {
            return MainWindow.SingdeConnection.ExecuteProc($"call GetNewStoreOrderBySingde('{ViewModelMainWindow.CurrentPharmacy.Id}')");
        }

        public static DataTable SetStoreOrderMaster(StoreOrder s) {
            DataTable storeOrderMasterTable = StoreOrderMasterTable();
            DataRow newRow = storeOrderMasterTable.NewRow(); 
            switch (s.OrderType) {
                case OrderTypeEnum.PURCHASE:
                    newRow["StoOrd_ID"] = DBNull.Value;
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_ID", s.ID);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_OrderEmployeeID", s.OrderEmployeeName);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveEmployeeID",null );
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_CreateTime", DateTime.Now);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveTime", null);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_ManufactoryID", s.OrderManufactory.ID); 
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_Status", GetOrderStatus(s.OrderStatus) );
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_Type", GetOrderType(s.OrderType));
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_WarehouseID", s.OrderWarehouse.ID);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_Note", s.Note);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_PrescriptionID", null); 
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_IsEnable", true);
                    break;
                case OrderTypeEnum.RETURN:
                    newRow["StoOrd_ID"] = DBNull.Value;
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_OrderEmployeeID", ViewModelMainWindow.CurrentUser.ID);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveEmployeeID", null);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_CreateTime", DateTime.Now);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_ReceiveTime", null );
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_ManufactoryID", s.OrderManufactory.ID);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_Status", GetOrderStatus(s.OrderStatus));
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_Type", GetOrderType(s.OrderType));
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_WarehouseID", s.OrderWarehouse.ID);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_Note", s.Note);
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_PrescriptionID", null); 
                    DataBaseFunction.AddColumnValue(newRow, "StoOrd_IsEnable", true);
                    break;
            } 
            storeOrderMasterTable.Rows.Add(newRow); 
            return storeOrderMasterTable; 
        }
        private static DataTable SetPurchaseOrderDetail(PurchaseOrder p) {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (var pro in p.OrderProducts) {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", p.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.OrderAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", pro.UnitName);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", pro.UnitAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", pro.RealAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", pro.Price);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", pro.SubTotal);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate",pro.ValidDate);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", pro.BatchNumber);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", pro.Note);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", pro.FreeAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", pro.Invoice);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            } 
            return storeOrderDetailTable; 
        }
        private static DataTable SetReturnOrderDetail(ReturnOrder r)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (var pro in r.OrderProducts)
            {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", r.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", pro.ID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", pro.ReturnAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", pro.UnitName);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", pro.UnitAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", pro.RealAmount);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", pro.Price);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", pro.SubTotal);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", pro.BatchNumber);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", pro.Note);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", pro.BatchLimit);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }

        private static DataTable SetPurchaseOrderDetail(DataTable table, string storeOrderID)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();
            foreach (DataRow row in table.Rows)
            {
                string dateString = row.Field<string>("VALIDDATE");
                DateTime? validDate = null;
                if (dateString != null && dateString != string.Empty)
                    validDate = new DateTime(int.Parse(dateString.Substring(0, dateString.Length - 4)) + 1911, int.Parse(dateString.Substring(dateString.Length - 4, 2)), int.Parse(dateString.Substring(dateString.Length - 2, 2)));

                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", storeOrderID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", row.Field<string>("PRO_ID"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", row.Field<float>("AMOUNT"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "基本單位");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", row.Field<float>("AMOUNT"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", row.Field<float>("PRICE") / row.Field<float>("AMOUNT"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", row.Field<float>("PRICE"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", validDate);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", row.Field<string>("BATCHNUM"));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }
        private static DataTable SetPurchaseOrderDetail(string productsFromSingde, string storeOrderID)
        {
            int detailId = 1;
            DataTable storeOrderDetailTable = StoreOrderDetailTable();

            string[] drugs = productsFromSingde.Split(new[] { "\r" }, StringSplitOptions.None);

            foreach (string drug in drugs)
            {
                DataRow newRow = storeOrderDetailTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_MasterID", storeOrderID);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ProductID", drug.Substring(0, 12).Trim());
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ID", detailId);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_OrderAmount", Double.Parse(drug.Substring(12, 10).Trim()));
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitName", "基本單位");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_UnitAmount", 1);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_RealAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Price", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_SubTotal", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_ValidDate", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_BatchNumber", null);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Note", (drug.Length >= 22) ? drug.Substring(22).Trim() : "");
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_FreeAmount", 0);
                DataBaseFunction.AddColumnValue(newRow, "StoOrdDet_Invoice", null);
                storeOrderDetailTable.Rows.Add(newRow);
                detailId++;
            }
            return storeOrderDetailTable;
        }
        public static DataTable StoreOrderMasterTable() {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("StoOrd_ID", typeof(string));
            masterTable.Columns.Add("StoOrd_OrderEmployeeID", typeof(int));
            masterTable.Columns.Add("StoOrd_ReceiveEmployeeID", typeof(int));
            masterTable.Columns.Add("StoOrd_CreateTime", typeof(DateTime));
            masterTable.Columns.Add("StoOrd_ReceiveTime", typeof(DateTime));
            masterTable.Columns.Add("StoOrd_ManufactoryID", typeof(int));
            masterTable.Columns.Add("StoOrd_Status", typeof(string));
            masterTable.Columns.Add("StoOrd_Type", typeof(string));
            masterTable.Columns.Add("StoOrd_WarehouseID", typeof(int));
            masterTable.Columns.Add("StoOrd_Note", typeof(string));
            masterTable.Columns.Add("StoOrd_PrescriptionID", typeof(int));
            masterTable.Columns.Add("StoOrd_IsEnable", typeof(bool)); 
            return masterTable; 
        }
        public static DataTable StoreOrderDetailTable() {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("StoOrdDet_MasterID", typeof(string));
            detailTable.Columns.Add("StoOrdDet_ProductID", typeof(string));
            detailTable.Columns.Add("StoOrdDet_ID", typeof(short));
            detailTable.Columns.Add("StoOrdDet_OrderAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_UnitName", typeof(string));
            detailTable.Columns.Add("StoOrdDet_UnitAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_RealAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_Price", typeof(double));
            detailTable.Columns.Add("StoOrdDet_SubTotal", typeof(double));
            detailTable.Columns.Add("StoOrdDet_ValidDate", typeof(DateTime));
            detailTable.Columns.Add("StoOrdDet_BatchNumber", typeof(string));
            detailTable.Columns.Add("StoOrdDet_Note", typeof(string));
            detailTable.Columns.Add("StoOrdDet_FreeAmount", typeof(int));
            detailTable.Columns.Add("StoOrdDet_Invoice", typeof(string));
            return detailTable; 
        }
        private static string GetOrderStatus(OrderStatusEnum OrderStatus) { 
            switch (OrderStatus)
            {
                case OrderStatusEnum.NORMAL_UNPROCESSING:
                case OrderStatusEnum.SINGDE_UNPROCESSING:
                    return "U"; 
                case OrderStatusEnum.WAITING:
                    return "W"; 
                case OrderStatusEnum.SINGDE_PROCESSING:
                case OrderStatusEnum.NORMAL_PROCESSING:
                    return "P"; 
                case OrderStatusEnum.SCRAP:
                    return "S"; 
                case OrderStatusEnum.DONE:
                    return "D";
                default:
                    return string.Empty;
            } 
        }
        private static string GetOrderType(OrderTypeEnum orderTypeEnum) {
            switch (orderTypeEnum) {
                case OrderTypeEnum.PURCHASE:
                    return "P";
                case OrderTypeEnum.RETURN:
                    return "R";
                default:
                    return string.Empty;
            }
        }
        #endregion

        
    }
}
