using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.MedBagLocation
{
    public class MedBagLocationDb
    {
        internal static void SaveLocationData(ObservableCollection<MedBagLocation> medBagLocations)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var m in medBagLocations)
            {
                parameters.Clear();
                parameters.Add(new SqlParameter("MEDBAG_ID", m.Id));
                parameters.Add(new SqlParameter("MEDBAG_NAME", m.Name));
                parameters.Add(new SqlParameter("MEDBAG_X", m.PathX));
                parameters.Add(new SqlParameter("MEDBAG_Y", m.PathY));
                parameters.Add(new SqlParameter("MEDBAG_WIDTH", m.Width));
                parameters.Add(new SqlParameter("MEDBAG_HEIGHT", m.Height));
                parameters.Add(new SqlParameter("MEDBAG_ACTUALWIDTH", m.ActualWidth));
                parameters.Add(new SqlParameter("MEDBAG_ACTUALHEIGHT", m.ActualHeight));
                dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[SavaMedBagLocation]", parameters);
            }
        }

        internal static void UpdateLocationName(string locId, string name)
        {
        }

        internal static void DeleteLocation(string locId)
        {
        }
    }
}