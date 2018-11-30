using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.MedBagLocation
{
    public class MedBagLocationDb
    {
        internal static ObservableCollection<MedBagLocation> ObservableGetLocationData(string medBagId)
        {
            var medBagLocations = new ObservableCollection<MedBagLocation>();
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("MEDBAG_ID", medBagId),
            };
            var table = dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[GetMedBagLocationData]", parameters);
            foreach (DataRow row in table.Rows)
            {
                medBagLocations.Add(new MedBagLocation(row));
            }
            return medBagLocations;
        }

        public static DataTable CreateLocationDataTable()
        {
            var locationDataTable = new DataTable();
            locationDataTable.Columns.Add("MEDBAG_ID", typeof(string));
            locationDataTable.Columns.Add("MEDBAG_LOCNAME", typeof(string));
            locationDataTable.Columns.Add("MEDBAG_X", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_Y", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_WIDTH", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_HEIGHT", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_REALWIDTH", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_REALHEIGHT", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_LOCCONTENT", typeof(string));
            locationDataTable.Columns.Add("MEDBAG_CANVASLEFT", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_CANVASTOP", typeof(double));
            return locationDataTable;
        }

        public static DataTable AddLocationData(MedBag.MedBag medBag)
        {
            var locationDataTable = CreateLocationDataTable();
            foreach (var location in medBag.MedLocations)
            {
                var row = locationDataTable.NewRow();
                row["MEDBAG_ID"] = location.Id.ToString();
                row["MEDBAG_LOCNAME"] = location.Name;
                row["MEDBAG_X"] = location.PathX;
                row["MEDBAG_Y"] = location.PathY;
                row["MEDBAG_WIDTH"] = location.Width;
                row["MEDBAG_HEIGHT"] = location.Height;
                row["MEDBAG_REALWIDTH"] = location.RealWidth;
                row["MEDBAG_REALHEIGHT"] = location.RealHeight;
                row["MEDBAG_LOCCONTENT"] = location.Content;
                row["MEDBAG_CANVASLEFT"] = location.CanvasLeft;
                row["MEDBAG_CANVASTOP"] = location.CanvasTop;
                locationDataTable.Rows.Add(row);
            }
            return locationDataTable;
        }
    }
}
