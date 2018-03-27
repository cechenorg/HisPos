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
        public static ObservableCollection<StoreOrderOverview> GetStoreOrderOverview()
        {
            ObservableCollection<StoreOrderOverview> StoreOrderOverviewCollection = new ObservableCollection<StoreOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[STOORDDATA]");

            foreach (DataRow row in table.Rows)
            {
                StoreOrderOverviewCollection.Add(new StoreOrderOverview(row["STOORD_FLAG"].ToString(), row["STOORD_ID"].ToString(),
                                                 row["ORD_EMP"].ToString(), Double.Parse(row["TOTAL"].ToString()), row["REC_EMP"].ToString(), row["MAN_ID"].ToString()));
            }

            return StoreOrderOverviewCollection;
        }

        public static ObservableCollection<AbstractClass.Product> GetStoreOrderCollectionById(string StoOrdId)
        {
            ObservableCollection<AbstractClass.Product> StoreOrderCollection = new ObservableCollection<AbstractClass.Product>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORD_ID", StoOrdId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[STOORDLIST]", parameters);


            foreach (DataRow row in table.Rows)
            {
                switch (row[""].ToString())
                {
                    case "M":
                        StoreOrderCollection.Add(new Medicine(row));
                        break;
                    case "O":
                        StoreOrderCollection.Add(new Otc(row));
                        break;
                }
            }

            return StoreOrderCollection;
        }
    }
}
