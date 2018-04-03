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
        internal static void SaveOrderDetail(StoreOrder storeOrder) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
         parameters.Add(new SqlParameter("STOORD_ID", storeOrder.Id));
         parameters.Add(new SqlParameter("ORD_EMP", storeOrder.OrdEmp));
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
