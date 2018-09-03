using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.PrescriptionInquire;
using His_Pos.Properties;
using His_Pos.RDLC;
using His_Pos.Service;

namespace His_Pos.Class.Declare
{
    public class DeclareFileDb
    {
        public static ObservableCollection<DeclareFile> GetDeclareFilesData()
        {
            ObservableCollection<DeclareFile> declareFiles = new ObservableCollection<DeclareFile>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[DeclareFileExportView].[GetDeclareFilesData]", dbConnection);
            foreach (DataRow declareFile in divisionTable.Rows)
            {
                declareFiles.Add(new DeclareFile(declareFile));
            }
            return declareFiles;
        }

        public static void SetDeclareFileByPharmacyId(DeclareFile file,DateTime date,DeclareFileType type)
        {
            int[] sequence = { 0, 0, 0, 0};
            var p = file.FileContent;
            foreach (var t in p.Ddata)
            {
                switch (t.Dhead.D1)
                {
                    case "1":
                        sequence[0]++;
                        t.Dhead.D2 = sequence[0].ToString();
                        break;
                    case "2":
                        sequence[1]++;
                        t.Dhead.D2 = sequence[1].ToString();
                        break;
                    case "3":
                        sequence[2]++;
                        t.Dhead.D2 = sequence[2].ToString();
                        break;
                    case "5":
                        sequence[3]++;
                        t.Dhead.D2 = sequence[3].ToString();
                        break;
                    case "D":
                        sequence[4]++;
                        t.Dhead.D2 = sequence[4].ToString();
                        break;
                }
            }
            var xmlStr = SerializeDeclareFile(p);
            var parameters = new List<SqlParameter>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            parameters.Add(new SqlParameter("SEND_DATE", date));
            parameters.Add(new SqlParameter("PHARMACY_ID", p.Tdata.T2));
            parameters.Add(new SqlParameter("FILE_CONTENT", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(xmlStr, XmlNodeType.Document, null))
            });
            parameters.Add(new SqlParameter("ERROR", file.ErrorPrescriptionList));
            parameters.Add(new SqlParameter("CHRONIC_COUNT", int.Parse(p.Tdata.T9)));
            parameters.Add(new SqlParameter("NORMAL_COUNT", int.Parse(p.Tdata.T7)));
            parameters.Add(new SqlParameter("TOTAL_POINT", int.Parse(p.Tdata.T12)));
            parameters.Add(new SqlParameter("TOTAL_POINT", int.Parse(p.Tdata.T12)));
            parameters.Add(new SqlParameter("HAS_ERROR", file.HasError));
            parameters.Add(type.Equals(DeclareFileType.UPDATE)
                ? new SqlParameter("IS_DECLARED", file.IsDeclared)
                : new SqlParameter("IS_DECLARED", DBNull.Value));
            dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[UpdateDeclareFile]", parameters);
        }

        public static DeclareFile GetDeclareFileTypeLogIn(DateTime declareTime)
        {
            DeclareFile file = new DeclareFile();
            var parameters = new List<SqlParameter>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            parameters.Add(new SqlParameter("SEND_DATE", declareTime));
            parameters.Add(new SqlParameter("PHARMACY_ID", MainWindow.CurrentPharmacy.Id));
            var fileTable = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetDeclareFileData]", parameters);
            if (fileTable.Rows.Count > 0)
                file = new DeclareFile(fileTable.Rows[0]);
            return file;
        }

        private static string SerializeDeclareFile(Pharmacy p)
        {
            var serializer = new XmlSerializer(typeof(Pharmacy));
            StringWriter declareFileXml;
            using (var stringWriter = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    serializer.Serialize(writer, p);
                    declareFileXml = stringWriter;
                }
            }
            var document = XDocument.Parse(ReportService.PrettyXml(declareFileXml));
            document.Descendants()
                .Where(e => e.IsEmpty || string.IsNullOrWhiteSpace(e.Value))
                .Remove();
            return document.ToString().Replace("<pharmacy xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">","<pharmacy>");
        }
    }
}
