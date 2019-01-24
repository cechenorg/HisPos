using System;
using System.Data;

namespace His_Pos.NewClass.Prescription.IcData.Upload
{
    public class IcDataUploadDb
    {
        public static void InsertDailyUploadData(int id,string rec,DateTime create)
        {

        }
        public static DataTable GetDailyUploadData()
        {
            var table = new DataTable();
            UpdateDailyUploadData();
            return table;
        }
        public static void UpdateDailyUploadData()
        {

        }
    }
}
