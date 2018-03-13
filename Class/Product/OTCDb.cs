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
            return dd.ExecuteProc("[HIS_POS_DB].[GET].[OTC]");
        }

        public static ObservableCollection<CusOrderOverview> GetOtcCusOrderOverviewByID(string OtcID)
        {
            ObservableCollection<CusOrderOverview> collection = new ObservableCollection<CusOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OTCID", OtcID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[OTCCUSORDBYID]", parameters);

            foreach(DataRow row in table.Rows)
            {
                collection.Add(new CusOrderOverview(row["DATE"].ToString(), row["CUSORDDET_QTY"].ToString(), row["CUSORDDET_PROFITTOTAL"].ToString()));
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
    }
}
