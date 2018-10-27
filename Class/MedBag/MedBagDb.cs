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
            var medBags = new ObservableCollection<MedBag>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[GetMedBagData]");
            foreach (DataRow row in table.Rows)
            {
                medBags.Add(new MedBag(row));
            }
            return medBags;
        }

        internal static MedBag GetDefaultMedBagData(MedBagMode medBagMode)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("MEDBAG_MODE", medBagMode)
            };
            var table = dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[GetDefaultMedBag]",parameters);
            var defaultMedBag = new MedBag(table.Rows[0]);
            defaultMedBag.MedLocations = MedBagLocationDb.ObservableGetLocationData(defaultMedBag.Id);
            return defaultMedBag;
        }

        internal static void SaveMedBagData(MedBag medBag)
        {
            var locationDataTable = MedBagLocationDb.AddLocationData(medBag);
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
        }

        internal static void DeleteMedBagData(MedBag medBag)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("MEDBAG_ID", medBag.Id)
            };
            dd.ExecuteProc("[HIS_POS_DB].[MedBagManageView].[DeleteMedBagData]", parameters);
        }

        public static byte[] ImageToByte(BitmapImage img)
        {
            byte[] data;
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
    }
}
