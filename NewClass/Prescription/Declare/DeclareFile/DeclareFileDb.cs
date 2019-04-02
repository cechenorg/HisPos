using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Linq;
using His_Pos.Database;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFile
{
    public class DeclareFileDb
    {
        public static DataTable InsertDeclareFile(XDocument doc,DeclareFilePreview.DeclareFilePreview preview)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_Content",
                new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(doc).InnerXml, XmlNodeType.Document, null)));
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_CreateTime",DateTime.Now);
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_DeclareTime", new DateTime(preview.Date.Year,preview.Date.Month,1));
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_ChronicCount", preview.ChronicCount);
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_NormalCount", preview.NormalCount);
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_SimpleFormCount", preview.SimpleFormCount);
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_TotalPoint",preview.TotalPoint);
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_PharmacyID",preview.PharmacyID);
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_IsDeclared",false);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertDeclareFile]", parameterList);
        }

        public static DataTable CheckFileExist(string pharmacyID,DateTime declareTime)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_PharmacyID", pharmacyID);
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_DeclareTime", declareTime);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckDeclareFileExist]", parameterList);
        }

        public static DataTable UpdateDeclareStatus(int declareFileID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_ID", declareFileID);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateDeclareStatus]", parameterList);
        }
    }
}
