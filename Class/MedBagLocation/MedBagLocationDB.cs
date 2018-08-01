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
    public class MedBagLocationDB
    {
        internal static void SaveLocationData(ObservableCollection<MedBagLocation> MedBagLocations)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var location in MedBagLocations)
            {
                parameters.Clear();
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