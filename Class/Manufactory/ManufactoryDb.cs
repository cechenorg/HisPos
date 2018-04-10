﻿using His_Pos.Properties;
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
        public static List<string> GetProductByManId(string id) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MAN_ID", id));
            DataTable table = dd.ExecuteProc("[HIS_POS_DB].[GET].[MANBYID]",parameters);
            List<string> list = new List<string>();
            foreach (DataRow row in table.Rows) {
                list.Add(row["PRO_ID"].ToString());
            }
            return list;
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

        internal static ObservableCollection<Manufactory> GetManufactoriesBasicSafe(StoreOrderProductType type)
        {
            ObservableCollection<Manufactory> manufactories = new ObservableCollection<Manufactory>();
            
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE", (type == StoreOrderProductType.BASIC) ? "BASIC" : "SAFE"));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[MANUFACTORYBASICORSAFE]", parameters);

            foreach (DataRow m in table.Rows)
            {
                manufactories.Add(new Manufactory(m, DataSource.MANUFACTORY));
            }

            return manufactories;
        }
    }
}
