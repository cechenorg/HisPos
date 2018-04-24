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
        internal static void InsertOrderProduct(StoreOrder storeOrder, string flag)
        {
            if (storeOrder.Products == null) return;
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("STOORD_ID", storeOrder.Id));
            dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[DeleteStoreOrderDetail]", parameters);

            foreach (var row in storeOrder.Products)
            {
                parameters.Clear();
                parameters.Add(new SqlParameter("MAN_ID", storeOrder.Manufactory.Id));
                parameters.Add(new SqlParameter("STOORD_ID", storeOrder.Id));
                parameters.Add(new SqlParameter("PRO_ID", row.Id));
                parameters.Add(new SqlParameter("QTY", ((ITrade)row).Amount));
                parameters.Add(new SqlParameter("PRICE", ((ITrade)row).Price));
                parameters.Add(new SqlParameter("DESCRIPTION", ((IProductPurchase)row).Note));
                parameters.Add(new SqlParameter("VALIDDATE", ((IProductPurchase)row).ValidDate));
                parameters.Add(new SqlParameter("BATCHNUMBER", ((IProductPurchase)row).BatchNumber));
                parameters.Add(new SqlParameter("FREEQTY", ((IProductPurchase)row).FreeAmount));
                parameters.Add(new SqlParameter("INVOICE", ((IProductPurchase)row).Invoice));
                parameters.Add(new SqlParameter("FLAG", flag));
                if (String.IsNullOrEmpty(storeOrder.Category.CategoryName))
                    parameters.Add(new SqlParameter("TYPE", DBNull.Value));
                else
                    parameters.Add(new SqlParameter("TYPE", storeOrder.Category.CategoryName.Substring(0, 1)));
                dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[UpdateStoreOrderDetail]", parameters);
            }
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

            dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[SaveStoreOrder]", parameters);
            InsertOrderProduct(storeOrder, type);
        }

        internal static string GetNewOrderId(string OrdEmpId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ORDEMP_ID", OrdEmpId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[SET].[NEWSTOORD]", parameters);

            return table.Rows[0]["STOORD_ID"].ToString();
        }

        public static ObservableCollection<AbstractClass.Product> GetStoreOrderCollectionById(string StoOrdId)
        {
            ObservableCollection<AbstractClass.Product> StoreOrderCollection = new ObservableCollection<AbstractClass.Product>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", StoOrdId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetStoreOrderDetail]", parameters);
            
            foreach (DataRow row in table.Rows)
            {
                switch (row["PRO_TYPE"].ToString())
                {
                    case "M":
                        StoreOrderCollection.Add(new ProductPurchaseMedicine(row, DataSource.GetStoreOrderDetail));
                        break;
                    case "O":
                        StoreOrderCollection.Add(new ProductPurchaseOtc(row, DataSource.GetStoreOrderDetail));
                        break;
                }
            }
            return StoreOrderCollection;
        }
    }
}
