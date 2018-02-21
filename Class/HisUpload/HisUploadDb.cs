using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using His_Pos.HisApi;

namespace His_Pos.Class.HisUpload
{
    class HisUploadDb
    {
        private DbConnection conn = new DbConnection(Properties.Settings.Default.SQL_global);
        List<SqlParameter> parameters = new List<SqlParameter>();
        public void InsertDb(HisUploadData hisUploadData) {
            parameters.Clear();
            parameters.Add(new SqlParameter("@XML",hisUploadData.CreateToXml()));
            conn.ExecuteProc("[HIS_POS_DB].[SET].[UPLOADDATA]",parameters);
        }
        public XmlDocument ExportUploadData()
        {
            parameters.Clear();
            XmlDocument xml = null;
            string sxml = @"<?xml version=""1.0"" encoding=""Big5""?><RECS>";
            DataTable table = conn.ExecuteProc("[HIS_POS_DB].[GET].[UNUPLOADDATA]", parameters);
            foreach (DataRow row in table.Rows) {
                sxml += row["UPLOADDATA_CONTENT"].ToString();
            }
            sxml += "</RECS>";
            xml.LoadXml(sxml);
            return xml;
        }
        public void HisUploadData() {
            Function function = new Function();
            XmlDocument xml = ExportUploadData();
            string filepath = function.ExportXml(xml,"匯出健保資料XML檔案");
            StringBuilder pUploadFileName = new StringBuilder();
            pUploadFileName.Append(filepath);
            StringBuilder fFileSize = new StringBuilder();
            long length = new System.IO.FileInfo(filepath).Length;
            fFileSize.Append(length);
            StringBuilder pNumber = new StringBuilder();
            StringBuilder pBuffer = new StringBuilder();
            int iBufferLen = xml.InnerText.Length;
            HisApiBase.csOpenCom(0);
           int res = HisApiBase.csUploadData(pUploadFileName, fFileSize, pNumber, pBuffer, ref iBufferLen);
            HisApiBase.csCloseCom();
        }

    }
}
