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
        public static void UpdateOtcDataDetail(string ProId,string OtcSaveAmount,string OtcManufactory,string Location,string Description) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", ProId));
            parameters.Add(new SqlParameter("SAFEQTY", OtcSaveAmount));
            parameters.Add(new SqlParameter("MAN_NAME", OtcManufactory));
            parameters.Add(new SqlParameter("LOCATION", Location));
            parameters.Add(new SqlParameter("PRO_DESCRIPTION", Description));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEOTCDATADETAIL]", parameters);
        }
        public static void DeleteOtcUnitWithoutBasic(string ProId) {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", ProId));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[DELETEOTCUNITWITHOUTBASIC]", parameters);
        }
        public static void UpdateOtcDataUnit(string ProId,string PROUNI_TYPE,string PROUNI_QTY,string PRO_SELL_PRICE, string PRO_VIP_PRICE,string PRO_EMP_PRICE)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", ProId));
            parameters.Add(new SqlParameter("PROUNI_TYPE", PROUNI_TYPE));
            parameters.Add(new SqlParameter("PROUNI_QTY", PROUNI_QTY));
            parameters.Add(new SqlParameter("PRO_SELL_PRICE", PRO_SELL_PRICE));
            parameters.Add(new SqlParameter("PRO_VIP_PRICE", PRO_VIP_PRICE));
            parameters.Add(new SqlParameter("PRO_EMP_PRICE", PRO_EMP_PRICE));
            dd.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEOTCUNIT]", parameters);
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
        
        public static ObservableCollection<OTCStoreOrderOverview> GetOtcStoOrderByID(string OtcID)
        {
            ObservableCollection<OTCStoreOrderOverview> collection = new ObservableCollection<OTCStoreOrderOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OTCID", OtcID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[OTCSTOORDBYID]", parameters);

            foreach( DataRow row in table.Rows )
            {
                collection.Add(new OTCStoreOrderOverview(row["STOORD_DATE"].ToString(), row["EMP_NAME"].ToString(), row["STOORD_RECDATE"].ToString(), row["STOORDDET_PRICE"].ToString(), row["STOORDDET_QTY"].ToString()));
            }

            return collection;
        }

        internal static ObservableCollection<OTCStockOverview> GetOtcStockOverviewById(string OtcID)
        {
            ObservableCollection<OTCStockOverview> collection = new ObservableCollection<OTCStockOverview>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", OtcID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[PRODUCTDETAILSTOCK]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new OTCStockOverview(row["VALIDDATE"].ToString(), row["PRICE"].ToString(), row["STOCK"].ToString()));
            }

            return collection;
        }

        internal static ObservableCollection<OTCUnit> GetOtcUnitById(string OtcID)
        {
            ObservableCollection<OTCUnit> collection = new ObservableCollection<OTCUnit>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", OtcID));

            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[PRODUCTUNIT]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new OTCUnit(row["PROUNI_TYPE"].ToString(), row["PROUNI_QTY"].ToString(), row["PRO_SELL_PRICE"].ToString(), row["PRO_VIP_PRICE"].ToString(), row["PRO_EMP_PRICE"].ToString()));
            }

            return collection;
        }
    }
}
