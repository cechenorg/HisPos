﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Linq;
using His_Pos.Database;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.IcData.Upload
{
    public class IcDataUploadDb
    {
        public static DataTable InsertDailyUploadData(int id,string rec,DateTime create)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rec);
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreMasId", id);
            DataBaseFunction.AddSqlParameter(parameterList, "Content", new SqlXml(new XmlTextReader(xml.InnerXml, XmlNodeType.Document, null)));
            DataBaseFunction.AddSqlParameter(parameterList, "CreateTime", create);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertUploadData]", parameterList);
        }
        public static DataTable GetDailyUploadData()
        {
            var table = new DataTable();
            table = MainWindow.ServerConnection.ExecuteProc("[Get].[UploadData]");
            return table;
        }
        public static void UpdateDailyUploadData()
        {
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateUploadData]");
        }
        public static void InsertDailyUploadFile(XDocument xml)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Content", new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(xml).InnerXml, XmlNodeType.Document, null)));
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertUploadDataHisotry]", parameterList);
        }
    }
}
