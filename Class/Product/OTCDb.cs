using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;


namespace His_Pos.Class.Product
{
    public class OTCDb
    {
        public static ObservableCollection<Otc> GetOTC(string start, string end, string name, string id) {
            var conn = new DbConnection(Settings.Default.SQL_global);
            ObservableCollection<Otc> observableCollection = new ObservableCollection<Otc>();
           DateTimeExtensions dateTimeExtensions = new DateTimeExtensions();
            var parameters = new List<SqlParameter>();
            if (start != string.Empty)
                parameters.Add(new SqlParameter("SDATE", dateTimeExtensions.UsToTaiwan(start)));
            else
                parameters.Add(new SqlParameter("SDATE", DBNull.Value));
            if (end != string.Empty)
                parameters.Add(new SqlParameter("EDATE", dateTimeExtensions.UsToTaiwan(end)));
            else
                parameters.Add(new SqlParameter("EDATE", DBNull.Value));
            if (name != string.Empty)
                parameters.Add(new SqlParameter("NAME", name));
            else
                parameters.Add(new SqlParameter("NAME", DBNull.Value));
            if (id != string.Empty)
                parameters.Add(new SqlParameter("ID", id));
            else
                parameters.Add(new SqlParameter("ID", DBNull.Value));
            observableCollection.Clear();
            var table = conn.ExecuteProc("[HIS_POS_DB].[GET].[INVENTORYDATA]", parameters);
            foreach (DataRow row in table.Rows)
            {
                var product = new Otc(row["PRO_ID"].ToString(), row["PRO_NAME"].ToString(), row["PRO_PRICE"].ToString(), row["PRO_INVENTORY"].ToString());
                product.SafeAmount = row["PRO_SAFEQTY"].ToString();
                product.ManufactoryName = row["MAN_NAME"].ToString();
                product.CurrentImportTime = row["PURCHASE_DATE"].ToString();
                observableCollection.Add(product);
            }
            return observableCollection;
        }
    }
}
