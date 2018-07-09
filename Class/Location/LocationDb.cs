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

namespace His_Pos.Class
{
    public static class LocationDb
    {
        internal static ObservableCollection<string> ObservableGetLocationData()
        {
            ObservableCollection<string> sourceBig = new ObservableCollection<string>();
            sourceBig.Add("尚未有櫃位產品");
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[GetLocationData]");
            foreach (DataRow row in table.Rows)
            {
                sourceBig.Add(row["LOC_NAME"].ToString());
            }
            return sourceBig;
        }
        internal static DataTable GetLocationData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[GetLocationData]");
        }
        internal static void SaveLocationData(ObservableCollection<Location> locations)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var location in locations) {
                parameters.Clear();
                parameters.Add(new SqlParameter("LOC_ID", location.id));
                parameters.Add(new SqlParameter("LOC_NAME", location.name));
                parameters.Add(new SqlParameter("LOC_X", location.pathX));
                parameters.Add(new SqlParameter("LOC_Y", location.pathY));
                parameters.Add(new SqlParameter("LOC_WIDTH", location.width));
                parameters.Add(new SqlParameter("LOC_HEIGHT", location.heigh));
                dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[SavaLocation]",parameters);
            }
          
        }
        internal static DataTable GetLocationDetail(string id)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("LOC_ID", id));
            return dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[GetLocationDetail]", parameters);
        }
        internal static ObservableCollection<string> ObservableGetLocationDetail() {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("LOC_ID", DBNull.Value));
           var table = dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[GetLocationDetail]", parameters);
            ObservableCollection<string> sourceSmall = new ObservableCollection<string>();
            foreach (DataRow row in table.Rows)
            {
                sourceSmall.Add(row["LOCD_NAME"].ToString());
            }
            return sourceSmall;
        }
        internal static void UpdateLocationDetail(LocationDetail location)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
          
                parameters.Add(new SqlParameter("LOC_ID", location.id));
                parameters.Add(new SqlParameter("LOCD_NAME", location.name));
                parameters.Add(new SqlParameter("LOCD_ROW", location.locdrow));
                parameters.Add(new SqlParameter("LOCD_COLUMN", location.locdcolumn));
                dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[UpdateLocatiobDetail]", parameters);
            
        }
        internal static void DeleteLocationDetail(LocationDetail location)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
          
                parameters.Add(new SqlParameter("LOC_ID", location.id));
                parameters.Add(new SqlParameter("LOCD_NAME", location.name));
                parameters.Add(new SqlParameter("LOCD_ROW", location.locdrow));
                parameters.Add(new SqlParameter("LOCD_COLUMN", location.locdcolumn));
                dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[DeleteLocatiobDetail]", parameters);

        }
        internal static void UpdateLocationName(string locId,string name)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("LOC_ID", locId));
            parameters.Add(new SqlParameter("NAME", name));
            dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[UpdateLocationName]", parameters);
        }
        internal static bool CheckProductExist(string locId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("LOC_ID", locId));
           DataTable table = dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[CheckProductExist]", parameters);
            if (table.Rows[0][0].ToString() == "0")
                return true;
            else
                return false;
        }
        internal static void DeleteLocation(string locId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("LOC_ID", locId));
            dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[DeleteLocation]", parameters);
        }
        internal static DataTable GetProductLocation() {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[GetProductLocation]");
        }
        internal static DataTable UpdateLocationDetail(string id,string newvalue)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PRO_ID", id));
            parameters.Add(new SqlParameter("NEW_VALUE", newvalue));
            return dd.ExecuteProc("[HIS_POS_DB].[LocationManageView].[UpdateLocationDetail]", parameters);
        }
       
    }
}
