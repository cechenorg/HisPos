﻿using His_Pos.Database;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace His_Pos.NewClass.Cooperative.XmlOfPrescription
{
   public static class XmlOfPrescriptionDb
    {
        public static void Insert(List<string> cusIDNumbers, List<XDocument> xmls, string typeName) {
            DataTable dataTable = SetCooperativeClinicPrescription(cusIDNumbers, xmls, typeName);
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CooperativeClinicPrescriptions", dataTable);  
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCooperativeClinicPrescription]", parameterList);
        }
        public static DataTable SetCooperativeClinicPrescription(List<string> cusIDNumbers, List<XDocument> xmls,string typeName) {
            DataTable cooperativeClinicTable = CooperativeClinicPrescriptionTable(); 
            for (int i = 0; i < cusIDNumbers.Count ; i++) {
                DataRow newRow = cooperativeClinicTable.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "Cus_IDNumber", cusIDNumbers[i]);
                DataBaseFunction.AddColumnValue(newRow, "CooCli_XML", new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(xmls[i]).InnerXml, XmlNodeType.Document, null))); 
                DataBaseFunction.AddColumnValue(newRow, "CooCli_Type", typeName);
                cooperativeClinicTable.Rows.Add(newRow);
            }
            return cooperativeClinicTable;
        }
        public static DataTable CooperativeClinicPrescriptionTable() {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("Cus_IDNumber", typeof(string));
            masterTable.Columns.Add("CooCli_XML", typeof(SqlXml));
            masterTable.Columns.Add("CooCli_Type", typeof(string));
            return masterTable;
        }
    }
}
