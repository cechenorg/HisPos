using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.H4_BASIC_MANAGE.MedBagManage
{
    public static class MedBagDb
    {
        internal static void SaveLocationData(ObservableCollection<Location> locations)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var location in locations)
            {
                parameters.Clear();
            }
        }

        internal static void GetLocationDetail(string id)
        {
        }

        internal static void ObservableGetLocationDetail()
        {
        }

        internal static void UpdateLocationDetail(LocationDetail location)
        {
        }

        internal static void DeleteLocationDetail(LocationDetail location)
        {
        }

        internal static void UpdateLocationName(string locId, string name)
        {
        }

        internal static void CheckProductExist(string locId)
        {
        }

        internal static void DeleteLocation(string locId)
        {
        }

        internal static void GetProductLocation()
        {
        }

        internal static void UpdateLocationDetail(string id, string newvalue)
        {
        }
    }
}