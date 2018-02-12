using His_Pos.Class.Person;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;

namespace His_Pos.Class.Declare
{
    public class DeclareDb
    {
        public void InsertDb(DeclareData declareData, string type = null, string id = null)
        {
            var parameters = new List<SqlParameter>();
            AddParameterDData(parameters,declareData);
            var pDataTable = new DataTable();
            SetPDataTable(pDataTable);
            AddPData(declareData,pDataTable);
            AddMedicalServiceCostPData(declareData, pDataTable);
            parameters.Add(new SqlParameter("DETAIL", pDataTable));
            parameters.Add(new SqlParameter("XML", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(CreateToXml(declareData).InnerXml, XmlNodeType.Document, null))
            });
            CheckInsertDbTypeUpdate(parameters,id, type);
        }
        /*
         * 加入DData資料之parameters
         */
        private void AddParameterDData(ICollection<SqlParameter> parameters,DeclareData declareData)
        {
            AddParameterTreatment(parameters, declareData);
            CheckDbNullValue(parameters, declareData.DeclareMakeUp,"D4");//D4補報註記
            CheckDbNullValueInt(parameters, declareData.SpecailMaterialPoint,"D31");//特殊材料明細點數小計
            CheckDbNullValueInt(parameters, declareData.DiagnosisPoint, "D32");//診療明細點數小計
            CheckDbNullValueInt(parameters, declareData.DrugsPoint,"D33");//用藥明細點數小計
            parameters.Add(new SqlParameter("D7", declareData.Prescription.IcCard.MedicalNumber));//就醫序號
            parameters.Add(new SqlParameter("D16", declareData.DeclarePoint));//申請點數
            parameters.Add(new SqlParameter("D17", declareData.CopaymentPoint));//部分負擔點數
            parameters.Add(new SqlParameter("D18", declareData.TotalPoint));//合計點數
            parameters.Add(declareData.AssistProjectCopaymentPoint == 0 ? new SqlParameter("D19", DBNull.Value) : new SqlParameter("D19", declareData.AssistProjectCopaymentPoint));
            AddUnusedParameters(parameters);//設定免填欄位Parameters
            CheckChronicAdjust(declareData,parameters);//判斷慢箋調劑欄位
            parameters.Add(new SqlParameter("D38", declareData.MedicalServicePoint));//藥事服務費點數
        }
        /*
         * 加入DeclareData.Treatment資料之parameters
         */
        private void AddParameterTreatment(ICollection<SqlParameter> parameters, DeclareData declareData)
        {
            AddParameterMedicalInfo(parameters, declareData);
            parameters.Add(new SqlParameter("D1", declareData.Prescription.Treatment.AdjustCase.Id));
            CheckDbNullValue(parameters, declareData.Prescription.Treatment.PaymentCategory.Id, "D5");
            parameters.Add(new SqlParameter("CUS_ID", declareData.Prescription.Treatment.Customer.Id));
            CheckDbNullValue(parameters, declareData.Prescription.Treatment.TreatmentDate, "D14");
            parameters.Add(new SqlParameter("D15", declareData.Prescription.Treatment.Copayment.Id));
            parameters.Add(new SqlParameter("D23", declareData.Prescription.Treatment.AdjustDate));
            parameters.Add(new SqlParameter("D25", declareData.Prescription.Treatment.MedicalPersonId));
            parameters.Add(new SqlParameter("D30", declareData.Prescription.Treatment.MedicineDays));
        }
        /*
         * 加入DeclareData.Treatment.MedicalInfo資料之parameters
         */
        private void AddParameterMedicalInfo(ICollection<SqlParameter> parameters, DeclareData declareData)
        {
            var med = declareData.Prescription.Treatment.MedicalInfo;
            var valueList = new List<string>() { med.DiseaseCodes[0].Id, med.DiseaseCodes[1].Id, med.Hospital.Division.Id, med.TreatmentCase.Id, med.Hospital.Doctor.Id, med.SpecialCode.Id};
            var paraNameList = new List<string>() { "D8", "D9", "D13", "D22", "D24", "D26"};
            parameters.Add(new SqlParameter("D21", declareData.Prescription.Treatment.MedicalInfo.Hospital.Id));
            for (var i = 0; i < valueList.Count; i++)
            {
                CheckDbNullValue(parameters,valueList[i],paraNameList[i]);
            }
        }

        private void SetPDataTable(DataTable pDataTable)
        {
            pDataTable.Columns.Add("P10", typeof(short));
            pDataTable.Columns.Add("P1", typeof(string));
            pDataTable.Columns.Add("P2", typeof(string));
            pDataTable.Columns.Add("P7", typeof(float));
            pDataTable.Columns.Add("P8", typeof(decimal));
            pDataTable.Columns.Add("P9", typeof(int));
            pDataTable.Columns.Add("P3", typeof(double));
            pDataTable.Columns.Add("P4", typeof(string));
            pDataTable.Columns.Add("P5", typeof(string));
            pDataTable.Columns.Add("P6", typeof(string));
            pDataTable.Columns.Add("P11", typeof(string));
            pDataTable.Columns.Add("P12", typeof(string));
            pDataTable.Columns.Add("P13", typeof(string));
            pDataTable.Columns.Add("PAY_BY_YOURSELF", typeof(string));
        }

        private void AddPData(DeclareData declareData,DataTable pDataTable)
        {
            for (var i = 0; i < declareData.DeclareDetails.Count; i++)
            {
                var row = pDataTable.NewRow();
                var detail = declareData.DeclareDetails[i];
                var valueList = new List<string>() { detail.MedicalOrder,detail.MedicalId,ToInvCulture(detail.Dosage),detail.Usage,
                    detail.Position, ToInvCulture(detail.Percent),ToInvCulture(detail.Total),ToInvCulture(detail.Price),ToInvCulture(detail.Point),detail.Days.ToString()
                };
                var rowNameList = new List<string>() { "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P11" };
                row["P10"] = Convert.ToInt16(detail.Sequence);
                for (var j = 0; j < valueList.Count; i++)
                {
                    CheckEmptyDataRow(valueList[j],row,rowNameList[j]);
                }
                row["PAY_BY_YOURSELF"] = declareData.Prescription.Medicines[i].PaySelf;
                pDataTable.Rows.Add(row);
            }
        }
        /*
         * 加入藥事服務費之PData
         */
        private void AddMedicalServiceCostPData(DeclareData declareData,DataTable pDataTable)
        {
            var dateTimeExtensions = new DateTimeExtensions();
            var percent = CountAdditionPercent(declareData);
            var currentDate = dateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            var detail = new DeclareDetail(declareData.MedicalServiceCode, percent, declareData.MedicalServicePoint, declareData.DeclareDetails.Count + 1, currentDate, currentDate);
            var pData = pDataTable.NewRow();
            SetMedicalServiceCostDataRow(pData, declareData,detail);
            declareData.DeclareDetails.Add(detail);
            pDataTable.Rows.Add(pData);
        }
        /*
         * 計算支付成數
         */
        private double CountAdditionPercent(DeclareData declareData)
        {
            double percent = 0;
            var dateTimeExtensions = new DateTimeExtensions();
            var cusBirth = declareData.Prescription.Treatment.Customer.Birthday;
            var month = dateTimeExtensions.CalculateAge(dateTimeExtensions.ToUsDate(cusBirth));
            if (month < 0.5) percent = 160;
            if (month > 0.5 && month <= 2) percent = 130;
            if (month > 2 && month <= 6) percent = 120;
            return percent;
        }
        /*
         * 設定藥事服務費PData之DataRow
         */
        private void SetMedicalServiceCostDataRow(DataRow pData,DeclareData declareData,DeclareDetail detail)
        {
            var declarecount = declareData.DeclareDetails.Count + 1; //藥事服務醫令序
            pData["P1"] = detail.MedicalOrder;
            pData["P2"] = detail.MedicalId;
            pData["P3"] = detail.Dosage;
            pData["P6"] = detail.Percent;
            pData["P7"] = string.Format("{0:00000.0}", detail.Total);//五位整數，一位小數
            pData["P8"] = string.Format("{0:0000000.00}", detail.Price);//七位整數，二位小數
            pData["P9"] = string.Format("{0:D8}", Math.Round(detail.Point, 1));//八位整數，小數點四捨五入
            pData["P10"] = string.Format("{0:D3}", declarecount);
            pData["P12"] = detail.StartDate;
            pData["P13"] = detail.EndDate;
        }
        /*
         * 判斷慢箋調劑
         */
        private void CheckChronicAdjust(DeclareData declareData,ICollection<SqlParameter> parameters)
        {
            if (!declareData.Prescription.Treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId))//判斷是否為慢箋調劑
            {
                parameters.Add(new SqlParameter("D35", DBNull.Value));
                parameters.Add(new SqlParameter("D36", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("D35", declareData.ChronicSequence));
                parameters.Add(new SqlParameter("D36", declareData.ChronicTotal));
            }
            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId) && Convert.ToInt32(declareData.ChronicSequence) > 1)
                parameters.Add(new SqlParameter("D43", declareData.Prescription.OriginalMedicalNumber));
            else
            {
                parameters.Add(new SqlParameter("D43", DBNull.Value));
            }
        }
        /*
         * 加入免填欄位Parameters
         */
        private void AddUnusedParameters(ICollection<SqlParameter> parameters)
        {
            parameters.Add(new SqlParameter("D10", DBNull.Value));
            parameters.Add(new SqlParameter("D11", DBNull.Value));
            parameters.Add(new SqlParameter("D12", DBNull.Value));
            parameters.Add(new SqlParameter("D27", DBNull.Value));
            parameters.Add(new SqlParameter("D28", DBNull.Value));
            parameters.Add(new SqlParameter("D29", DBNull.Value));
        }
        /*
         * 判斷InsertDb type為Update
         */
        private void CheckInsertDbTypeUpdate(List<SqlParameter> parameters, string id, string type = null)
        {
            var conn = new DbConnection(Settings.Default.SQL_global);
            if (type == "Update")
            {
                parameters.Add(new SqlParameter("ID", id));
                conn.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEDECLAREDATA]", parameters);
            }
            else
            {
                conn.ExecuteProc("[HIS_POS_DB].[SET].[DECLAREDATA]", parameters);
            }
        }

        private XmlDocument CreateToXml(DeclareData declareData)
        {
            var xml = new XmlDocument();
            var dData = SetDheadXml(declareData);
            foreach (var detail in declareData.DeclareDetails)
            {
                dData += SetPDataXmlStr(detail,declareData);
            }
            dData += "</ddata>";
            xml.LoadXml(dData);
            return xml;
        }

        private string SetDheadXml(DeclareData declareData)
        {
            var diseaseCodeCount = 8;
            var treatment = declareData.Prescription.Treatment;
            var medicalInfo = treatment.MedicalInfo;
            var dData = "<ddata><dhead>";
            dData += "<d1>" + treatment.AdjustCase.Id + "</d1>";
            dData += "<d2></d2>";
            dData += "<d3>" + treatment.Customer.IcNumber + "</d3>";
            dData += CheckXmlDbNullValue(declareData.DeclareMakeUp, "d4");
            dData += CheckXmlDbNullValue(treatment.PaymentCategory.Id, "d5");
            dData += "<d6>" + treatment.Customer.Birthday + "</d6>";
            dData += "<d7>" + declareData.Prescription.IcCard.MedicalNumber + "</d7>";
            //D8 ~ D12 國際疾病代碼
            foreach (var diseasecode in medicalInfo.DiseaseCodes)
            {
                dData += "<d" + diseaseCodeCount + ">" + diseasecode.Id + "</d" + diseaseCodeCount + ">";
                diseaseCodeCount++;
            }
            dData += CheckXmlDbNullValue(medicalInfo.Hospital.Division.Id, "d13");
            dData += CheckXmlDbNullValue(treatment.TreatmentDate, "d14");
            dData += "<d15>" + treatment.Copayment.Id + "</d15>";
            dData += "<d16>" + declareData.DeclarePoint + "</d16>";
            dData += "<d17>" + treatment.Copayment.Point + "</d17>";
            dData += "<d18>" + declareData.TotalPoint + "</d18>";
            CheckXmlDbNullValue(declareData.AssistProjectCopaymentPoint.ToString(), "d19");
            dData += "<d20>" + treatment.Customer.Name + "</d20>";
            dData += "<d21>" + medicalInfo.Hospital.Id + "</d21>";
            dData += "<d22>" + medicalInfo.TreatmentCase.Id + "</d22>";
            dData += "<d23>" + treatment.AdjustDate + "</d23>";
            dData += "<d24>" + medicalInfo.Hospital.Doctor.Id + "</d24>";
            dData += "<d25>" + treatment.MedicalPersonId + "</d25>";
            CheckXmlDbNullValue(medicalInfo.SpecialCode.Id, "d26");
            CheckXmlDbNullValue(treatment.MedicineDays, "d30");
            dData += "<d30>" + treatment.MedicineDays + "</d30>";
            CheckXmlDbNullValue(declareData.SpecailMaterialPoint.ToString(),"d31");
            CheckXmlDbNullValue(declareData.DiagnosisPoint.ToString(), "d32");
            CheckXmlDbNullValue(declareData.DrugsPoint.ToString(), "d33");
            if (treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId))
            {
                dData += "<d35>" + declareData.ChronicSequence + "</d35>";
                dData += "<d36>" + declareData.ChronicTotal + "</d36>";
            }
            dData += "<d37>" + declareData.MedicalServiceCode + "</d37>";
            dData += "<d38>" + declareData.MedicalServicePoint + "</d38>";
            if (treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId))
            {
                if (Convert.ToDecimal(declareData.ChronicSequence) >= 2)
                    dData += "<d43>" + declareData.Prescription.OriginalMedicalNumber + "</d43>";
            }
            if (treatment.Copayment.Id == "903")
                dData += CheckXmlDbNullValue(declareData.Prescription.IcCard.IcMarks.NewbornsData.Birthday, "d44");//新生兒註記就醫
            dData += "</dhead>";
            return dData;
        }

        private string SetPDataXmlStr(DeclareDetail detail,DeclareData declareData)
        {
            var pData = "<pdata>";
            pData += "<p1>" + detail.MedicalOrder + "</p1>";
            pData += "<p2>" + detail.MedicalId + "</p2>";
            pData += "<p7>" + detail.Total + "</p7>";
            pData += "<p8>" + detail.Price + "</p8>";
            pData += "<p9>" + detail.Point + "</p9>";
            if (ToInvCulture(detail.Dosage) != string.Empty) pData += "<p3>" + detail.Dosage + "</p3>";
            if (detail.Usage != string.Empty) pData += "<p4>" + detail.Usage + "</p4>";
            if (detail.Position != string.Empty) pData += "<p5>" + detail.Position + "</p5>";
            if (ToInvCulture(detail.Percent) != string.Empty) pData += "<p6>" + detail.Percent + "</p6>";
            pData += "<p10>" + detail.Sequence + "</p10>";
            if (detail.Days.ToString() != string.Empty)
            {
                pData += "<p11>" + detail.Days + "</p11>";
                if (Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) < detail.Days)
                    declareData.Prescription.Treatment.MedicineDays = detail.Days.ToString();
            }
            pData += "</pdata>";
            return pData;
        }

        public void ExportSortDeclareData(string sdate, string edate)
        {
            Function function = new Function();
            var conn = new DbConnection(Properties.Settings.Default.SQL_local);
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("SDATE", sdate));
            param.Add(new SqlParameter("EDATE", edate));
            var datetable = conn.ExecuteProc("[HIS_POS_DB].[GET].[UNEXPORTDECLAREDATE]", param);//取得未申報檔案
            param.Clear();
            param.Add(new SqlParameter("SDATE", sdate));
            param.Add(new SqlParameter("EDATE", edate));
            var medpertable = conn.ExecuteProc("[HIS_POS_DB].[GET].[MEDCALPERSON]", param);//取得現有藥師
            var table = new DataTable();
            var xml = new XmlDocument();
            int casid1, casid2, casid3,totalcasid;
            var ncount = 0; //一般處方件數
            var npoint = 0; //一般處方點數
            var scount = 0; //慢性處方件數
            var spoint = 0; //慢性處方點數
            var gcount = 0; //被降低之點數
            var gpoint = 0; //被降低之件數
            var twc = new TaiwanCalendar();
            var year = twc.GetYear(DateTime.Now).ToString();
            var month = function.GetDateFormat(twc.GetMonth(DateTime.Now).ToString());
            var day = function.GetDateFormat(twc.GetDayOfMonth(DateTime.Now).ToString());
            var xmlsum = @"<?xml version=""1.0"" encoding=""Big5""?><pharmacy><tdata>";
            var xmlddata = string.Empty;
            var d1 = string.Empty;
            var d16 = string.Empty;
            var d18 = string.Empty;
            var d38 = string.Empty;
            foreach (DataRow row in datetable.Rows)
            {
                foreach (DataRow medperrow in medpertable.Rows)
                {
                    param.Clear();
                    param.Add(new SqlParameter("DATE", row["HISDECMAS_PRESCRIPTIONDATE"].ToString()));
                    param.Add(new SqlParameter("MEDPERSON", medperrow["HISDECMAS_MEDICALPERSONNEL"].ToString()));
                    table = conn.ExecuteProc("[HIS_POS_DB].[GET].[DECLAREDATABYDATE]", param);
                    casid1 = 0;
                    casid2 = 0;
                    casid3 = 0;
                    totalcasid = casid1 + casid2 + casid3;
                    foreach (DataRow datarow in table.Rows)
                    {
                        param.Clear();
                        param.Add(new SqlParameter("ID", datarow["CUS_ID"].ToString()));
                        table = conn.ExecuteProc("[HIS_POS_DB].[GET].[CUSDATA]", param);
                        var cus = new Customer();
                        cus.Id = table.Rows[0]["CUS_ID"].ToString();
                        cus.Name = table.Rows[0]["CUS_NAME"].ToString();
                        cus.Qname = table.Rows[0]["CUS_QNAME"].ToString();
                        cus.Birthday = table.Rows[0]["CUS_BIRTH"].ToString();
                        cus.ContactInfo.Address = table.Rows[0]["CUS_ADDR"].ToString();
                        cus.ContactInfo.Tel = table.Rows[0]["CUS_TEL"].ToString();
                        cus.IcNumber = table.Rows[0]["CUS_IDNUM"].ToString();
                        cus.ContactInfo.Email = table.Rows[0]["CUS_EMAIL"].ToString();
                        cus.Gender = Convert.ToBoolean(table.Rows[0]["CUS_GENDER"].ToString());

                        xml.LoadXml(datarow["HISDECMAS_DETXML"].ToString());
                        d1 = xml.SelectSingleNode("ddata/dhead/d1").InnerText;
                        d16 = xml.SelectSingleNode("ddata/dhead/d16").InnerText;
                        d18 = xml.SelectSingleNode("ddata/dhead/d18").InnerText;
                        d38 = xml.SelectSingleNode("ddata/dhead/d38").InnerText;
                        switch (d1) {
                            case "1":
                                casid1++;
                                break;
                            case "2":
                                casid2++;
                                break;
                            case "3":
                                casid3++;
                                break;
                        }
                      
                        if (totalcasid > 80 && totalcasid <= 100)
                        {
                            xml.SelectSingleNode("ddata/dhead/d18").InnerText = (Convert.ToInt32(d18) - Convert.ToInt32(d38) + 18).ToString();
                            xml.SelectSingleNode("ddata/dhead/d16").InnerText = (Convert.ToInt32(d16) - Convert.ToInt32(d38) + 18).ToString();
                            gcount++;
                            gpoint += Convert.ToInt32(d38) - 18;
                            xml.SelectSingleNode("ddata/dhead/d38").InnerText = "18";
                        }
                        if (totalcasid > 100)
                        {
                            xml.SelectSingleNode("ddata/dhead/d18").InnerText = (Convert.ToInt32(d18) - Convert.ToInt32(d38)).ToString();
                            xml.SelectSingleNode("ddata/dhead/d16").InnerText = (Convert.ToInt32(d16) - Convert.ToInt32(d38)).ToString();
                            gcount++;
                            gpoint += Convert.ToInt32(d38);
                            xml.SelectSingleNode("ddata/dhead/d38").InnerText = "0";
                        }
                        //取得處方點數
                        if (d1 == "1" || d1 == "3" || d1 == "4" || d1 == "5" || d1 == "D")
                        {
                            ncount++;
                            npoint += Convert.ToInt32(d18);
                        }
                        if (d1 == "2")
                        {
                            scount++;
                            spoint += Convert.ToInt32(d18);
                        }
                        param.Clear();
                        param.Add(new SqlParameter("MASID", datarow["HISDECMAS_ID"].ToString()));
                        param.Add(new SqlParameter("ORDERID", "0"));
                        param.Add(new SqlParameter("xml", SqlDbType.Xml)
                        {
                            Value = new SqlXml(new XmlTextReader(xml.InnerXml, XmlNodeType.Document, null))
                        });
                        conn.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEDECLAREFLAG]", param);
                    }//foreach
                }//foreach medperrow
            }//foreach
            //取得當月排序DECLAREDAta 分配d2
            table = conn.ExecuteProc("[HIS_POS_DB].[GET].[PROCESSDECLAREDATA]");
            var catcount = 1;

            for (var i = 0; i < table.Rows.Count; i++)
            {
                xml.LoadXml(table.Rows[i]["HISDECMAS_DETXML"].ToString());
                if (i > 0 && table.Rows[i]["HISCASCAT_ID"].ToString() == table.Rows[i - 1]["HISCASCAT_ID"].ToString()) catcount++;
                if (i > 0 && table.Rows[i]["HISCASCAT_ID"].ToString() != table.Rows[i - 1]["HISCASCAT_ID"].ToString()) catcount = 1;
                xml.SelectSingleNode("ddata/dhead/d2").InnerText = catcount.ToString();
                xmlddata += xml.InnerXml.ToString();
                param.Clear();
                param.Add(new SqlParameter("MASID", table.Rows[i]["HISDECMAS_ID"].ToString()));
                param.Add(new SqlParameter("ORDERID", catcount));
                param.Add(new SqlParameter("xml", SqlDbType.Xml)
                {
                    Value = new SqlXml(new XmlTextReader(xml.InnerXml, XmlNodeType.Document, null))
                });
                conn.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEDECLAREFLAG]", param);
            }

            //開始編成TDATA
            xmlsum += "<t1>" + "30" + "</t1>"; //特約藥局：請填30
            xmlsum += "<t2>" + "30" + "</t2>"; //服務機構代碼
            xmlsum += "<t3>" + year + month + "</t3>"; //費用年月
            xmlsum += "<t4>" + "2" + "</t4>"; //申報方式 2:媒體 3:連線
            xmlsum += "<t5>" + "1" + "</t5>"; //申報類別 1:送核 2:補報
            xmlsum += "<t6>" + year + month + day + "</t6>"; //申報日期
            xmlsum += "<t7>" + ncount + "</t7>";//一般案件申請件數
            xmlsum += "<t8>" + npoint + "</t8>";  //一般案件申請點數
            xmlsum += "<t9>" + scount + "</t9>"; //慢性病連續處方調劑案件申請件數
            xmlsum += "<t10>" + spoint + "</t10>"; //慢性病連續處方調劑案件申請點數
            xmlsum += "<t11>" + ((int)ncount + (int)scount).ToString() + "</t11>"; //申請件數總計
            xmlsum += "<t12>" + ((int)npoint + (int)spoint).ToString() + "</t12>"; //申請點數總計
            var table1 = conn.ExecuteProc("[HIS_POS_DB].[GET].[DECLARESDDATE]");
            xmlsum += "<t13>" + table1.Rows[0]["SDATE"] + "</t13>"; //此次連線申報起日期
            xmlsum += "<t14>" + table1.Rows[0]["EDATE"] + "</t14>"; //此次連線申報迄日期
            xmlsum += "</tdata>" + xmlddata + "</pharmacy>";
            var xmlsumx = new XmlDocument();
            xmlsumx.LoadXml(xmlsum);

            //匯出xml檔案
            function.ExportXml(xmlsumx,"匯出申報XML檔案");
        }
        /*
         * 檢查SQLparameter是否為DBNull
         */
        private void CheckDbNullValue(ICollection<SqlParameter> parameters,string value,string paraName)
        {
            parameters.Add(value.Equals(string.Empty) ? new SqlParameter(paraName, DBNull.Value) : new SqlParameter(paraName, value));
        }
        /*
         * 檢查SQLparameter是否為DBNull(int)
         */
        private void CheckDbNullValueInt(ICollection<SqlParameter> parameters, int value, string paraName)
        {
            parameters.Add(value == 0 ? new SqlParameter(paraName, DBNull.Value) : new SqlParameter(paraName, value));
        }
        /*
         * 檢查XmlTag是否為空值
         */
        private string CheckXmlDbNullValue(string value,string tagName)
        {
            if (value != string.Empty || value != "0")
               return "<"+tagName+">" + value + "</"+ tagName+">";
            return string.Empty;
        }
        /*
         *檢查DataRow是否為空值
         */
        private void CheckEmptyDataRow(string value ,DataRow row,string rowName)
        {
            if (value != string.Empty)
                row[rowName] = value;
        }

        private string ToInvCulture(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
