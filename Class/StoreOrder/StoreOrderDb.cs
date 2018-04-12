using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.Product;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.StoreOrder
{
    public static class StoreOrderDb
    {
        public static ObservableCollection<StoreOrder> GetStoreOrderOverview()
        {
            ObservableCollection<StoreOrder> StoreOrderOverviewCollection = new ObservableCollection<StoreOrder>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[STOORDDATA]");

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
        internal static void DeleteOrderProduct(string Id) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID",Id));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[DELETEORDERPRODUCT]",parameters);
        }
        internal static void PurchaseAndReturn(StoreOrder storeOrder) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var product in storeOrder.Products) {
                parameters.Add(new SqlParameter("TYPE", storeOrder.Category.Substring(0, 1)));
                parameters.Add(new SqlParameter("PRO_ID",product.Id));
                parameters.Add(new SqlParameter("PRO_INVENT", product.Amount));
                dd.ExecuteProc("[HIS_POS_DB].[SET].[PURCHASEANDRETURN]",parameters);
                parameters.Clear();
            }
        }
        internal static void InsertOrderProduct(StoreOrder storeOrder)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var row in storeOrder.Products)
            {
                parameters.Add(new SqlParameter("STOORD_ID", storeOrder.Id));
                parameters.Add(new SqlParameter("PRO_ID", row.Id));
                parameters.Add(new SqlParameter("QTY", row.Amount));
                parameters.Add(new SqlParameter("PRICE", row.Price));
                parameters.Add(new SqlParameter("DESCRIPTION", row.Note));
                dd.ExecuteProc("[HIS_POS_DB].[SET].[INSERTORDERPRODUCT]",parameters);

                parameters.Clear();
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
            if (String.IsNullOrEmpty(storeOrder.Category))
                parameters.Add(new SqlParameter("STOORD_TYPE",DBNull.Value));
            else
                parameters.Add(new SqlParameter("STOORD_TYPE", storeOrder.Category.Substring(0, 1)));

            if (storeOrder.Manufactory.Id == null)
                parameters.Add(new SqlParameter("MAN_ID", DBNull.Value));
            else
                parameters.Add(new SqlParameter("MAN_ID", storeOrder.Manufactory.Id));
            if (storeOrder.RecEmp == "無")
                parameters.Add(new SqlParameter("REC_EMP",DBNull.Value ));
            else
                parameters.Add(new SqlParameter("REC_EMP", storeOrder.RecEmp));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[SAVEORDERDETAIL]",parameters);
            DeleteOrderProduct(storeOrder.Id);
            InsertOrderProduct(storeOrder);
            if (type == "D") PurchaseAndReturn(storeOrder);
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

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[STOORDLIST]", parameters);

            int i = 0;
            foreach (DataRow row in table.Rows)
            {
                switch (row["PRO_TYPE"].ToString())
                {
                    case "M":
                        StoreOrderCollection.Add(new Medicine(row, DataSource.STOORDLIST));
                        StoreOrderCollection[i].Amount = Int32.Parse(row["STOORDDET_QTY"].ToString());
                        break;
                    case "O":
                        StoreOrderCollection.Add(new Otc(row, DataSource.STOORDLIST));
                        StoreOrderCollection[i].Amount = Int32.Parse(row["STOORDDET_QTY"].ToString());
                        break;
                }
                i++;
            }
            return StoreOrderCollection;
        }
    }
}
