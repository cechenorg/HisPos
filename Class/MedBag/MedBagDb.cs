using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Media.Imaging;
using His_Pos.Class.MedBagLocation;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.MedBag
{
    public class MedBagDb
    {
        internal static ObservableCollection<MedBag> ObservableGetMedBagData()
        {
            ObservableCollection<MedBag> medBags = new ObservableCollection<MedBag>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[GetMedBagData]");
            foreach (DataRow row in table.Rows)
            {
                medBags.Add(new MedBag(row));
            }
            return medBags;
        }
        internal static ObservableCollection<MedBagLocation.MedBagLocation> ObservableGetLocationData(string medBagId)
        {
            ObservableCollection<MedBagLocation.MedBagLocation> medBagLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("MEDBAG_ID", medBagId),
            };
            var table = dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[GetMedBagLocationData]", parameters);
            foreach (DataRow row in table.Rows)
            {
                medBagLocations.Add(new MedBagLocation.MedBagLocation(row));
            }
            return medBagLocations;
        }
        internal static void SaveMedBagData(MedBag medBag)
        {
            var locationDataTable = CreateLocationDataTable();
            foreach (var location in medBag.MedLocations)
            {
                AddLocationData(location, locationDataTable);
            }
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("MEDBAG_ID", medBag.Id),
                new SqlParameter("MEDBAG_NAME", medBag.Name),
                new SqlParameter("MEDBAG_IMAGE", ImageToByte(medBag.MedBagImage)),
                new SqlParameter("MEDBAG_SIZEX", medBag.BagWidth),
                new SqlParameter("MEDBAG_SIZEY", medBag.BagHeight),
                new SqlParameter("MEDBAG_MODE", medBag.Mode),
                new SqlParameter("MEDBAG_DEFAULT",medBag.Default),
                new SqlParameter("LOCATION",locationDataTable)
            };

            dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[SavaMedBag]", parameters);
            MedBagLocationDb.SaveLocationData(medBag.MedLocations, medBag.Id);
        }
        public static byte[] ImageToByte(BitmapImage img)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        private static DataTable CreateLocationDataTable()
        {
            DataTable locationDataTable = new DataTable();
            locationDataTable.Columns.Add("MEDBAG_ID",typeof(string));
            locationDataTable.Columns.Add("MEDBAG_LOCNAME",typeof(string));
            locationDataTable.Columns.Add("MEDBAG_X",typeof(double));
            locationDataTable.Columns.Add("MEDBAG_Y",typeof(double));
            locationDataTable.Columns.Add("MEDBAG_WIDTH",typeof(double));
            locationDataTable.Columns.Add("MEDBAG_HEIGHT",typeof(double));
            locationDataTable.Columns.Add("MEDBAG_REALWIDTH", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_REALHEIGHT", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_LOCCONTENT", typeof(string));
            locationDataTable.Columns.Add("MEDBAG_CANVASLEFT", typeof(double));
            locationDataTable.Columns.Add("MEDBAG_CANVASTOP", typeof(double));
            return locationDataTable;
        }

        private static void AddLocationData(MedBagLocation.MedBagLocation medBagLocation, DataTable locationDataTable)
        {
            var row = locationDataTable.NewRow();
            row["MEDBAG_ID"] = medBagLocation.Id.ToString();
            row["MEDBAG_LOCNAME"] = medBagLocation.Name;
            row["MEDBAG_X"] = medBagLocation.PathX;
            row["MEDBAG_Y"] = medBagLocation.PathY;
            row["MEDBAG_WIDTH"] = medBagLocation.Width;
            row["MEDBAG_HEIGHT"] = medBagLocation.Height;
            row["MEDBAG_REALWIDTH"] = medBagLocation.RealWidth;
            row["MEDBAG_REALHEIGHT"] = medBagLocation.RealHeight;
            row["MEDBAG_LOCCONTENT"] = medBagLocation.Content;
            row["MEDBAG_CANVASLEFT"] = medBagLocation.CanvasLeft;
            row["MEDBAG_CANVASTOP"] = medBagLocation.CanvasTop;
            locationDataTable.Rows.Add(row);
        }
    }
}
