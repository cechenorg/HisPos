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
            dd.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEPROMAN]", parameters);
        }

        internal static ObservableCollection<ProductDetailManufactory> GetManufactoryCollection(string proId)
        {
            ObservableCollection<ProductDetailManufactory> manufactories = new ObservableCollection<ProductDetailManufactory>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[PROMAN]", parameters);
            
            foreach (DataRow m in table.Rows)
            {
                manufactories.Add(new ProductDetailManufactory(m, DataSource.PROMAN));
            }

            return manufactories;
        }

        internal static ObservableCollection<Manufactory> GetManufactoriesBelowSafeAmount()
        {
            throw new NotImplementedException();
        }
    }
}
