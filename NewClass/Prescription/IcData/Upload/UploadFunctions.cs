using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.RDLC;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.IcData.Upload
{
    public class UploadFunctions
    {
        public static void StartDailyUpload(DataTable dailyUploadTable)
        {
            var icDataUpload = "<?xml version=\"1.0\" encoding=\"big5\" ?><RECS>";
            foreach (DataRow row in dailyUploadTable.Rows)
            {
                icDataUpload += row["UplData_Content"].ToString().Replace("<A18 />", "<A18></A18>");
            }
            icDataUpload += "</RECS>";
            var f = new Function();
            XDocument result = XDocument.Parse(icDataUpload);
            //匯出xml檔案
            f.DailyUpload(result, dailyUploadTable.Rows.Count.ToString());
            MainWindow.ServerConnection.OpenConnection();
            IcDataUploadDb.InsertDailyUploadFile(result);
            MainWindow.ServerConnection.CloseConnection();
            IcDataUploadDb.UpdateDailyUploadData();
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
