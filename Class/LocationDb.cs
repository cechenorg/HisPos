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
    }
}
