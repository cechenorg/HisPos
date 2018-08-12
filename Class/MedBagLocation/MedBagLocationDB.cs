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
        internal static void SaveLocationData(ObservableCollection<MedBagLocation> medBagLocations,string medBagId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            foreach (var m in medBagLocations)
            {
                parameters.Clear();
                parameters.Add(new SqlParameter("MEDBAG_ID", medBagId));
                parameters.Add(new SqlParameter("MEDBAG_LOCNAME", m.Name));
                parameters.Add(new SqlParameter("MEDBAG_X", m.PathX));
                parameters.Add(new SqlParameter("MEDBAG_Y", m.PathY));
                parameters.Add(new SqlParameter("MEDBAG_WIDTH", m.Width));
                parameters.Add(new SqlParameter("MEDBAG_HEIGHT", m.Height));
                parameters.Add(new SqlParameter("MEDBAG_REALWIDTH", m.RealWidth));
                parameters.Add(new SqlParameter("MEDBAG_REALHEIGHT", m.RealHeight));
                parameters.Add(new SqlParameter("MEDBAG_LOCCONTENT", m.Content));
                parameters.Add(new SqlParameter("MEDBAG_CANVASLEFT", m.CanvasLeft));
                parameters.Add(new SqlParameter("MEDBAG_CANVASTOP", m.CanvasTop));
                dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[SavaMedBagLocation]", parameters);
            }
        }

        internal static ObservableCollection<MedBag.MedBag> ObservableGetLocationData()
        {
            ObservableCollection<MedBag.MedBag> medBagCollection = new ObservableCollection<MedBag.MedBag>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[GetMedBagLocationData]");
            foreach (DataRow row in table.Rows)
            {
                medBagCollection.Add(new MedBag.MedBag(row));
            }
            return medBagCollection;
        }

        internal static void UpdateLocationName(string locId, string name)
        {
        }

        internal static void DeleteLocation(string locId)
        {
        }
    }
}