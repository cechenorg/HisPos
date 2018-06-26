using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Manufactory
{
   public class ManufactoryDb
    {
        public static DataTable GetProductByManId(string id) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", id));
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[MANBYID]", parameters); 
    }
        public static DataTable GetManufactoryData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[MANUFACTORY]");
        }
        internal static void UpdateProductManufactory(string productId, ManufactoryChanged manufactoryChanged)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", productId));
            parameters.Add(new SqlParameter("MAN_ID", manufactoryChanged.ManufactoryId));
            parameters.Add(new SqlParameter("ORDER_ID", manufactoryChanged.changedOrderId));
            parameters.Add(new SqlParameter("PROCESSTYPE", manufactoryChanged.ProcessType.ToString()));
            dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[UpdateProMan]", parameters);
        }

        internal static ObservableCollection<ManageManufactory> GetManageManufactory()
        {
            ObservableCollection<ManageManufactory> collection = new ObservableCollection<ManageManufactory>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[GetManageManufactory]");

            foreach (DataRow row in table.Rows)
            {
                string parent = row["MAN_PARENT"].ToString();

                if (parent.Equals(String.Empty) )
                {
                    collection.Add(new ManageManufactory(row));
                }
                else
                {
                    ManageManufactory man = collection.Where(m => m.Id.Equals(parent)).ToList()[0];

                    man.ManufactoryPrincipals.Add(new ManufactoryPrincipal(row));
                }
            }

            return collection;
        }

        internal static ObservableCollection<ManufactoryStoreOrderOverview> GetManufactoryStoreOrderOverview(string manId)
        {
            ObservableCollection<ManufactoryStoreOrderOverview> collection = new ObservableCollection<ManufactoryStoreOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", manId));

            var table = dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[GetManufactoryStoreOrderOverview]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new ManufactoryStoreOrderOverview(row));
            }

            return collection;
        }

        internal static ObservableCollection<ProductDetailManufactory> GetManufactoryCollection(string proId)
        {
            ObservableCollection<ProductDetailManufactory> manufactories = new ObservableCollection<ProductDetailManufactory>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetProMan]", parameters);
            
            foreach (DataRow m in table.Rows)
            {
                manufactories.Add(new ProductDetailManufactory(m, DataSource.PROMAN));
            }

            return manufactories;
        }

        internal static void AddNewOrderBasicSafe(StoreOrderProductType type, Manufactory manufactory)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE", (type == StoreOrderProductType.BASIC) ? "BASIC" : "SAFE"));
            parameters.Add(new SqlParameter("ORDEMP_ID", MainWindow.CurrentUser.Id));

            if(manufactory is null)
                parameters.Add(new SqlParameter("MAN_ID", DBNull.Value));
            else
                parameters.Add(new SqlParameter("MAN_ID", manufactory.Id));

            dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[AddNewOrderBySafeOrBasic]", parameters);
        }

        internal static ManageManufactory AddNewManageManufactory()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[AddNewManageManufactory]");

            return new ManageManufactory(table.Rows[0]);
        }
    }
}
