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
using LiveCharts;


namespace His_Pos.Class.Product
{
    public class OTCDb
    {
        public static DataTable GetOtcData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[MainWindowView].[GetOtc]");
        }
        public static InventoryOtc GetOtcDetail(string proId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", proId));
            InventoryOtc otc = null;
            foreach (DataRow row in dd.ExecuteProc("[HIS_POS_DB].[GET].[OTCIMDETAILBYID]", parameters).Rows) {
                 otc = new InventoryOtc(row);
            }
            return otc;
        }
       
        
        

        internal static DataTable GetInventoryOtcs()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[InventoryManagementView].[GetInventoryOtc]");
        }

        public static ObservableCollection<CusOrderOverview> GetOtcCusOrderOverviewByID(string OtcID)
        {
            ObservableCollection<CusOrderOverview> collection = new ObservableCollection<CusOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OTCID", OtcID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetOtcCustomerById]", parameters);

            foreach(DataRow row in table.Rows)
            {
                collection.Add(new CusOrderOverview(row["DATE"].ToString(), row["CUSORDDET_QTY"].ToString(), row["CUS_NAME"].ToString()));
            }

            return collection;
        }

        public static ChartValues<double> GetOtcSalesByID(string OtcID)
        {
            ChartValues<double> chartValues = new ChartValues<double>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OTCID", OtcID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[OTCSALESBYID]", parameters);

            foreach (DataRow row in table.Rows)
            {
                chartValues.Add(Convert.ToDouble(row["CUSORDDET_QTY"].ToString()));
            }

            return chartValues;
        }
        
        public static ObservableCollection<OTCStoreOrderOverview> GetOtcStoOrderByID(string OtcID)
        {
            ObservableCollection<OTCStoreOrderOverview> collection = new ObservableCollection<OTCStoreOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OTCID", OtcID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[OtcDetail].[GetOtcStoordById]", parameters);

            foreach( DataRow row in table.Rows )
            {
                collection.Add(new OTCStoreOrderOverview(row["STOORD_ID"].ToString(),row["MAN_NAME"].ToString(),row["STOORD_DATE"].ToString(),row["STOORDDET_VALIDDATE"].ToString(),row["EMP_NAME"].ToString(), row["STOORD_RECDATE"].ToString(), row["STOORDDET_PRICE"].ToString(), row["STOORDDET_QTY"].ToString(),row["STOORDDET_FREEQTY"].ToString()));
            }

            return collection;
        }
     
    }
}
