using His_Pos.Service;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace His_Pos.NewClass.Prescription.ICCard.Upload
{
    public class UploadFunctions
    {
        public static void StartDailyUpload(DataTable dailyUploadTable)
        {
            var icDataUpload = dailyUploadTable.Rows.Cast<DataRow>().Aggregate("<RECS>", (current, row) => current + row["UplData_Content"].ToString().Replace("<A18/>", "<A18></A18>"));
            icDataUpload += "</RECS>";
            var f = new Function();
            var result = XDocument.Parse(icDataUpload);
            //匯出xml檔案
            f.DailyUpload(result, dailyUploadTable.Rows.Count.ToString());
        }

        public static void StartDailyUpload100(DataTable dailyUploadTable)
        {
            var icDataUpload = dailyUploadTable.Rows.Cast<DataRow>().Aggregate("<RECS>", (current, row) => current + row["UplData_Content"].ToString().Replace("<A18/>", "<A18></A18>"));
            icDataUpload += "</RECS>";
            var f = new Function();
            var result = XDocument.Parse(icDataUpload);
            //匯出xml檔案
            f.DailyUploadWithoutMessage(result, dailyUploadTable.Rows.Count.ToString());
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