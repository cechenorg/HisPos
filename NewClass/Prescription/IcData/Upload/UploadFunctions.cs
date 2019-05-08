using System.Data;
using System.Xml.Linq;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.IcData.Upload
{
    public class UploadFunctions
    {
        public static void StartDailyUpload(DataTable dailyUploadTable)
        {
            var icDataUpload = "<RECS>";
            foreach (DataRow row in dailyUploadTable.Rows)
            {
                icDataUpload += row["UplData_Content"].ToString().Replace("<A18/>", "<A18></A18>");
            }
            icDataUpload += "</RECS>";
            var f = new Function();
            XDocument result = XDocument.Parse(icDataUpload);
            //匯出xml檔案
            f.DailyUpload(result, dailyUploadTable.Rows.Count.ToString());
            
        }

        public static DataTable CheckUpload()
        {
            MainWindow.ServerConnection.OpenConnection();
            var dailyUploadTable = IcDataUploadDb.GetDailyUploadData();
            MainWindow.ServerConnection.CloseConnection();
            return dailyUploadTable;
        }

    }
}
