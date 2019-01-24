using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using His_Pos.Database;

namespace His_Pos.NewClass.Prescription.IcData.Upload
{
    public class IcDataUploadDb
    {
        public static void InsertDailyUploadData(int id,string rec,DateTime create)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rec);
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreMasId", id);
            DataBaseFunction.AddSqlParameter(parameterList, "Content", new SqlXml(new XmlTextReader(xml.InnerXml, XmlNodeType.Document, null)));
            DataBaseFunction.AddSqlParameter(parameterList, "CreateTime", create);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertUploadData]", parameterList);
        }
        public static DataTable GetDailyUploadData()
        {
            var table = new DataTable();
            table = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertUploadData]");
            return table;
        }
        public static void UpdateDailyUploadData()
        {

        }
    }
}
