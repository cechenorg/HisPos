using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.Product;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;
using His_Pos.Struct.StoreOrder;

namespace His_Pos.Class.StoreOrder
{
    public static class StoreOrderDb
    {
        public static ObservableCollection<StoreOrder> GetStoreOrderOverview(OrderType type)
        {
            ObservableCollection<StoreOrder> StoreOrderOverviewCollection = new ObservableCollection<StoreOrder>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE", type.ToString()));
            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrder]", parameters);
            
            foreach (DataRow row in table.Rows)
            {
                StoreOrderOverviewCollection.Add(new StoreOrder(row));
            }

            return StoreOrderOverviewCollection;
        }
        internal static void DeleteOrder(string Id)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", Id));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[DELETEORDER]", parameters);
        }
        internal static void PurchaseAndReturn(StoreOrder storeOrder) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var product in storeOrder.Products) {
                parameters.Add(new SqlParameter("TYPE", storeOrder.Category.CategoryName.Substring(0, 1)));
                parameters.Add(new SqlParameter("PRO_ID",product.Id));
                //parameters.Add(new SqlParameter("PRO_INVENT", product.Amount));
                dd.ExecuteProc("[HIS_POS_DB].[SET].[PURCHASEANDRETURN]",parameters);
                parameters.Clear();
            }
        }

        internal static Collection<StoreOrderOverview> GetStoreOrderOverview()
        {
            Collection<StoreOrderOverview> collection = new Collection<StoreOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrderOverview]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new StoreOrderOverview(row));
            }

            return collection;
        }

        internal static void SaveOrderDetail(StoreOrder storeOrder) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", storeOrder.Id));
            parameters.Add(new SqlParameter("ORD_EMP", storeOrder.OrdEmp));
            string type = string.Empty;
            switch (storeOrder.Type) {
                case OrderType.UNPROCESSING:
                    type = "P";
                    break;
                case OrderType.PROCESSING:
                    type = "G";
                    break;
                case OrderType.DONE:
                    type = "D";
                    break;
                case OrderType.WAITING:
                    type = "W";
                    break;
            }
            parameters.Add(new SqlParameter("STOORD_FLAG", type));

            if (String.IsNullOrEmpty(storeOrder.Category.CategoryName))
                parameters.Add(new SqlParameter("STOORD_TYPE",DBNull.Value));
            else
                parameters.Add(new SqlParameter("STOORD_TYPE", storeOrder.Category.CategoryName.Substring(0, 1)));

            if (String.IsNullOrEmpty(storeOrder.Manufactory.Id))
                parameters.Add(new SqlParameter("MAN_ID", DBNull.Value));
            else
                parameters.Add(new SqlParameter("MAN_ID", storeOrder.Manufactory.Id));
            if (storeOrder.RecEmp == "")
                parameters.Add(new SqlParameter("REC_EMP",DBNull.Value ));
            else
                parameters.Add(new SqlParameter("REC_EMP", storeOrder.RecEmp));

            parameters.Add(new SqlParameter("PRINCIPAL_ID", storeOrder.Principal.Id));

            if (storeOrder.Warehouse is null)
                parameters.Add(new SqlParameter("WAREHOUSE_ID", DBNull.Value));
            else
                parameters.Add(new SqlParameter("WAREHOUSE_ID", storeOrder.Warehouse.Id));


            DataTable details = new DataTable();
            details.Columns.Add("PRO_ID", typeof(string));
            details.Columns.Add("ORDERQTY", typeof(int));
            details.Columns.Add("QTY", typeof(int));
            details.Columns.Add("PRICE", typeof(string));
            details.Columns.Add("DESCRIPTION", typeof(string));
            details.Columns.Add("VALIDDATE", typeof(string));
            details.Columns.Add("BATCHNUMBER", typeof(string));
            details.Columns.Add("FREEQTY", typeof(int));
            details.Columns.Add("INVOICE", typeof(string));
            DateTime datetimevalue;
            foreach (var product in storeOrder.Products)
            {
                var newRow = details.NewRow();

                newRow["PRO_ID"] = product.Id;
                newRow["ORDERQTY"] = ((IProductPurchase)product).OrderAmount;
                newRow["QTY"] = ((ITrade)product).Amount;
                newRow["PRICE"] = ((ITrade)product).Price == "" ? "0" : ((ITrade)product).Price;
                newRow["DESCRIPTION"] = ((IProductPurchase)product).Note;
                newRow["VALIDDATE"] = ( DateTime.TryParse(((IProductPurchase)product).ValidDate, out datetimevalue) ) ? ((IProductPurchase)product).ValidDate:string.Empty ;
                newRow["BATCHNUMBER"] = ((IProductPurchase) product).BatchNumber;
                newRow["FREEQTY"] = ((IProductPurchase)product).FreeAmount;
                newRow["INVOICE"] = ((IProductPurchase)product).Invoice;

                details.Rows.Add(newRow);
            }

            parameters.Add(new SqlParameter("DETAILS", details));

            dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[SaveStoreOrder]", parameters);
        }

        internal static string GetNewOrderId(string OrdEmpId, string wareId, string manId, string orderType)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDEMP_ID", OrdEmpId));
            parameters.Add(new SqlParameter("WARE_ID", wareId));
            parameters.Add(new SqlParameter("MAN_ID", manId));
            parameters.Add(new SqlParameter("ORDERTYPE", orderType));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[AddNewStoreOrder]", parameters);

            return table.Rows[0]["STOORD_ID"].ToString();
        }

        public static ObservableCollection<AbstractClass.Product> GetStoreOrderCollectionById(string StoOrdId)
        {
            ObservableCollection<AbstractClass.Product> StoreOrderCollection = new ObservableCollection<AbstractClass.Product>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", StoOrdId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrderDetail]", parameters);

            string lastProductId = "";

            foreach (DataRow row in table.Rows)
            {
                AbstractClass.Product product = null;
                string currentProductId = row["PRO_ID"].ToString();

                switch (row["PRO_TYPE"].ToString())
                {
                    case "M":
                        product = new ProductPurchaseMedicine(row, DataSource.GetStoreOrderDetail);
                        break;
                    case "O":
                        product = new ProductPurchaseOtc(row, DataSource.GetStoreOrderDetail);
                        break;
                }

                if (lastProductId == currentProductId)
                    ((IProductPurchase)product).IsFirstBatch = false;

                lastProductId = currentProductId;
                StoreOrderCollection.Add(product);
            }
            return StoreOrderCollection;
        }
    }
}
