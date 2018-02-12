using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using His_Pos;
using MahApps.Metro.Controls;

namespace His_Pos.Class.Declare
{
    public class DeclareDB
    {
        public void InsertDb(DeclareData declareData, string type = null, string id = null)
        {
            var dateTimeExtensions = new DateTimeExtensions();
            var conn = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("D1", declareData.Prescription.Treatment.AdjustCase.Id));
            parameters.Add(new SqlParameter("D21", declareData.Prescription.Treatment.MedicalInfo.Hospital.Id));
            // parameters.Add(new SqlParameter("D2", DBNull.Value));
            parameters.Add(declareData.Prescription.Treatment.MedicalInfo.TreatmentCase.Id.Equals(string.Empty) ? new SqlParameter("D22", DBNull.Value) : new SqlParameter("D22", declareData.Prescription.Treatment.MedicalInfo.TreatmentCase.Id));
            parameters.Add(new SqlParameter("D23", declareData.Prescription.Treatment.AdjustDate));
            parameters.Add(new SqlParameter("CUS_ID", declareData.Prescription.Treatment.Customer.Id));
            parameters.Add(new SqlParameter("D7", declareData.Prescription.IcCard.MedicalNumber));
            parameters.Add(new SqlParameter("D15", declareData.Prescription.Treatment.Copayment.Id));
            parameters.Add(new SqlParameter("D25", declareData.Prescription.Treatment.MedicalPersonId));
            parameters.Add(new SqlParameter("D16", declareData.DeclarePoint));
            parameters.Add(new SqlParameter("D17", declareData.CopaymentPoint));
            parameters.Add(new SqlParameter("D18", declareData.TotalPoint));
            parameters.Add(declareData.AssistProjectCopaymentPoint==0 ? new SqlParameter("D19", DBNull.Value) : new SqlParameter("D19", declareData.AssistProjectCopaymentPoint));
            parameters.Add(declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id.Equals(string.Empty) ? new SqlParameter("D26", DBNull.Value) : new SqlParameter("D26", declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id));
            parameters.Add(new SqlParameter("D27", DBNull.Value));
            parameters.Add(new SqlParameter("D28", DBNull.Value));
            parameters.Add(new SqlParameter("D29", DBNull.Value));
            parameters.Add(declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id.Equals(string.Empty) ? new SqlParameter("D13", DBNull.Value) : new SqlParameter("D13", declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id));
            parameters.Add(declareData.Prescription.Treatment.TreatmentDate.Equals(string.Empty) ? new SqlParameter("D14", DBNull.Value) : new SqlParameter("D14", declareData.Prescription.Treatment.TreatmentDate));
            parameters.Add(declareData.Prescription.Treatment.PaymentCategory.Id.Equals(string.Empty) ? new SqlParameter("D5", DBNull.Value) : new SqlParameter("D5", declareData.Prescription.Treatment.PaymentCategory.Id));
            parameters.Add(declareData.Prescription.Treatment.MedicalInfo.DiseaseCodes[0].Id.Equals(string.Empty) ? new SqlParameter("D8", DBNull.Value) : new SqlParameter("D8", declareData.Prescription.Treatment.MedicalInfo.DiseaseCodes[0].Id));
            parameters.Add(declareData.Prescription.Treatment.MedicalInfo.DiseaseCodes[1].Id.Equals(string.Empty) ? new SqlParameter("D9", DBNull.Value) : new SqlParameter("D9", declareData.Prescription.Treatment.MedicalInfo.DiseaseCodes[1].Id));
            parameters.Add(new SqlParameter("D10", DBNull.Value));
            parameters.Add(new SqlParameter("D11", DBNull.Value));
            parameters.Add(new SqlParameter("D12", DBNull.Value));
            parameters.Add(new SqlParameter("D4", DBNull.Value));
            parameters.Add(new SqlParameter("D30", declareData.Prescription.Treatment.MedicineDays.ToString()));
            parameters.Add(new SqlParameter("D31", DBNull.Value));
            parameters.Add(new SqlParameter("D32", DBNull.Value));
            parameters.Add(new SqlParameter("D33", DBNull.Value));
            parameters.Add(declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.Id.Equals(string.Empty) ? new SqlParameter("D24", DBNull.Value) : new SqlParameter("D24", declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.Id));
            if (!declareData.Prescription.Treatment.AdjustCase.Id.Equals("2"))
            {
                parameters.Add(new SqlParameter("D35", DBNull.Value));
                parameters.Add(new SqlParameter("D36", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("D35", declareData.ChronicSequence));
                parameters.Add(new SqlParameter("D36", declareData.ChronicTotal));
            }
            parameters.Add(new SqlParameter("D38", declareData.MedicalServicePoint));
            //parameters.Add(new SqlParameter("D40", DBNull.Value));
            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals("2") && Convert.ToInt32(declareData.ChronicSequence) > 1)
                parameters.Add(new SqlParameter("D43", declareData.Prescription.OriginalMedicalNumber));
            else
            {
                parameters.Add(new SqlParameter("D43", DBNull.Value));
            }
            var dtable = new DataTable();
            dtable.Columns.Add("P10", typeof(short));
            dtable.Columns.Add("P1", typeof(string));
            dtable.Columns.Add("P2", typeof(string));
            dtable.Columns.Add("P7", typeof(float));
            dtable.Columns.Add("P8", typeof(decimal));
            dtable.Columns.Add("P9", typeof(int));
            dtable.Columns.Add("P3", typeof(double));
            dtable.Columns.Add("P4", typeof(string));
            dtable.Columns.Add("P5", typeof(string));
            dtable.Columns.Add("P6", typeof(string));
            dtable.Columns.Add("P11", typeof(string));
            dtable.Columns.Add("P12", typeof(string));
            dtable.Columns.Add("P13", typeof(string));
            dtable.Columns.Add("PAY_BY_YOURSELF", typeof(string));
            DataRow row;
            for (var i = 0; i < declareData.DeclareDetails.Count; i++)
            {
                row = dtable.NewRow();
                row["P10"] = Convert.ToInt16(declareData.DeclareDetails[i].Sequence);
                if (declareData.DeclareDetails[i].MedicalOrder != string.Empty) row["P1"] = declareData.DeclareDetails[i].MedicalOrder;
                if (declareData.DeclareDetails[i].MedicalId != string.Empty) row["P2"] = declareData.DeclareDetails[i].MedicalId;
                if (declareData.DeclareDetails[i].Dosage.ToString(CultureInfo.InvariantCulture) != string.Empty) row["P3"] = declareData.DeclareDetails[i].Dosage.ToString(CultureInfo.InvariantCulture);
                if (declareData.DeclareDetails[i].Usage != string.Empty) row["P4"] = declareData.DeclareDetails[i].Usage;
                if (declareData.DeclareDetails[i].Position != string.Empty) row["P5"] = declareData.DeclareDetails[i].Position;
                if (declareData.DeclareDetails[i].Percent.ToString(CultureInfo.InvariantCulture) != string.Empty) row["P6"] = declareData.DeclareDetails[i].Percent.ToString(CultureInfo.InvariantCulture);
                if (declareData.DeclareDetails[i].Total.ToString(CultureInfo.InvariantCulture) != string.Empty) row["P7"] = declareData.DeclareDetails[i].Total.ToString(CultureInfo.InvariantCulture);
                if (declareData.DeclareDetails[i].Price.ToString(CultureInfo.InvariantCulture) != string.Empty) row["P8"] = declareData.DeclareDetails[i].Price.ToString(CultureInfo.InvariantCulture);
                if (declareData.DeclareDetails[i].Point.ToString(CultureInfo.InvariantCulture) != string.Empty) row["P9"] = declareData.DeclareDetails[i].Point.ToString(CultureInfo.InvariantCulture);
                if (declareData.DeclareDetails[i].Days.ToString() != string.Empty) row["P11"] = declareData.DeclareDetails[i].Days.ToString();
                row["PAY_BY_YOURSELF"] = declareData.Prescription.Medicines[i].PaySelf;
                dtable.Rows.Add(row);
            }
            var num = string.Empty;
            var declarecount = declareData.DeclareDetails.Count + 1; //藥事服務醫令序
            var num3 = string.Empty;
            var persent = string.Empty;

            var year = int.Parse(declareData.Prescription.Treatment.Customer.Birthday.Substring(0, 3)) + 1911;
            var cusBirth = year + "/" + declareData.Prescription.Treatment.Customer.Birthday.Substring(3, 2) + "/" +
                              declareData.Prescription.Treatment.Customer.Birthday.Substring(5, 2);
            var currentDate = dateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            var month = GetTimeDiff(cusBirth, currentDate);
            if (month < 6) persent = "160";
            if (month > 6 && month <= 24) persent = "130";
            if (month > 24 && month <= 72) persent = "120";
            DeclareDetail detail = new DeclareDetail(declareData.MedicalServiceCode,Convert.ToDouble(persent), declareData.MedicalServicePoint, declareData.DeclareDetails.Count + 1, currentDate, currentDate);
            row = dtable.NewRow();
            row["P1"] = detail.MedicalOrder;
            row["P2"] = detail.MedicalId;
            row["P3"] = detail.Dosage;
            row["P6"] = detail.Percent;
            row["P7"] = detail.Total;
            if (Convert.ToInt32(detail.Price) % 100 == 0) num = "00000";
            if (Convert.ToInt32(detail.Price) % 100 > 0) num = "0000";
            row["P8"] = num + detail.Price.ToString() + ".00";
            if (detail.Point >= 100) row["P9"] = "00000" + detail.Point.ToString();
            if (detail.Point < 100) row["P9"] = "000000" + detail.Point.ToString();
            if (declarecount < 10) row["P10"] = "00" + declarecount.ToString();
            if (declarecount >= 10 && declarecount < 100) row["P10"] = "0" + declarecount.ToString();
            if (declarecount >= 100 && declarecount < 1000) row["P10"] = declarecount.ToString();
            row["P12"] = detail.StartDate;
            row["P13"] = detail.EndDate;
            var sdetail = new DeclareDetail(row["P2"].ToString(),Convert.ToDouble(row["P6"].ToString()), Convert.ToDouble(row["P8"].ToString()),Convert.ToInt32(row["P10"].ToString()),row["P12"].ToString(),row["P13"].ToString());

            declareData.DeclareDetails.Add(sdetail);
            dtable.Rows.Add(row);
            //new row
            parameters.Add(new SqlParameter("DETAIL", dtable));
            parameters.Add(new SqlParameter("XML", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(CreateToXml(declareData).InnerXml, XmlNodeType.Document, null))
            });

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
        public static int GetTimeDiff(string strFrom, string strTo)
        {
            var dtStart = DateTime.Parse(strFrom);
            var dtEnd = DateTime.Parse(strTo);
            var iMonths = dtEnd.Year * 12 + dtEnd.Month - (dtStart.Year * 12 + dtStart.Month) + 1;
            return iMonths;
        }
        public XmlDocument CreateToXml(DeclareData declareData)
        {
            var xml = new XmlDocument();
            var diseasecodecount = 8;
            var dData = "<ddata><dhead>";
            dData += "<d1>" + declareData.Prescription.Treatment.AdjustCase.Id + "</d1>";
            dData += "<d2></d2>";
            dData += "<d3>" + declareData.Prescription.Treatment.Customer.IcNumber + "</d3>";
            //D4 : 補報原因 非補報免填
            if (declareData.DeclareMakeUp != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d4>" + declareData.DeclareMakeUp + "</d4>";
            if (declareData.Prescription.Treatment.AdjustCase.Id != DBNull.Value.ToString(CultureInfo.InvariantCulture) && (declareData.Prescription.Treatment.AdjustCase.Id.Equals("2") || declareData.Prescription.Treatment.AdjustCase.Id.Equals("D")))
                dData += "<d5>" + declareData.Prescription.Treatment.PaymentCategory.Id + "</d5>";
            dData += "<d6>" + declareData.Prescription.Treatment.Customer.Birthday + "</d6>";
            dData += "<d7>" + declareData.Prescription.IcCard.MedicalNumber + "</d7>";
             //D8 ~ D12 國際疾病代碼
            foreach (var diseasecode in declareData.Prescription.Treatment.MedicalInfo.DiseaseCodes)
            {
                dData += "<d" + diseasecodecount + ">" + diseasecode.Id + "</d" + diseasecodecount + ">";
                diseasecodecount++;
            }
            if (declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d13>" + declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id + "</d13>";
            if (declareData.Prescription.Treatment.TreatmentDate != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d14>" + declareData.Prescription.Treatment.TreatmentDate + "</d14>";
            dData += "<d15>" + declareData.Prescription.Treatment.Copayment.Id + "</d15>";
            dData += "<d16>" + declareData.DeclarePoint + "</d16>";
            dData += "<d17>" + declareData.Prescription.Treatment.Copayment.Point + "</d17>";
            dData += "<d18>" + declareData.TotalPoint + "</d18>";
            if (declareData.AssistProjectCopaymentPoint.ToString(CultureInfo.InvariantCulture) != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d19>" + declareData.AssistProjectCopaymentPoint + "</d19>";
            dData += "<d20>" + declareData.Prescription.Treatment.Customer.Name + "</d20>";
            dData += "<d21>" + declareData.Prescription.Treatment.MedicalInfo.Hospital.Id + "</d21>";
            dData += "<d22>" + declareData.Prescription.Treatment.MedicalInfo.TreatmentCase.Id + "</d22>";
            dData += "<d23>" + declareData.Prescription.Treatment.AdjustDate + "</d23>";
            dData += "<d24>" + declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.Id + "</d24>";
            dData += "<d25>" + declareData.Prescription.Treatment.MedicalPersonId + "</d25>";
            if (declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d26>" + declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id + "</d26>";           
            if (declareData.Prescription.Treatment.MedicineDays != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d30>" + declareData.Prescription.Treatment.MedicineDays + "</d30>";
            if (declareData.SpecailMaterialPoint.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d31>" + declareData.SpecailMaterialPoint + "</d31>";
            if (declareData.DiagnosisPoint.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d32>" + declareData.DiagnosisPoint + "</d32>";
            if (declareData.DrugsPoint.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                dData += "<d33>" + declareData.DrugsPoint + "</d33>";
            //免填 dData += "<d34>" + "" + "</d34>";
            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals("2"))
            {
                dData += "<d35>" + declareData.ChronicSequence + "</d35>";
                dData += "<d36>" + declareData.ChronicTotal + "</d36>";
            }
            //待確認
            /*if (d37.ToString() != DBNull.Value.ToString())*/
            dData += "<d37>" + declareData.MedicalServiceCode + "</d37>";
            /*if (d38.ToString() != DBNull.Value.ToString())*/
            dData += "<d38>" + declareData.MedicalServicePoint + "</d38>";
            //D39~D42免填
            /*if (d44.ToString() != DBNull.Value.ToString())*/
            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals("2") && Convert.ToDecimal(declareData.ChronicSequence) >= 2)
                dData += "<d43>" + declareData.Prescription.OriginalMedicalNumber + "</d43>";
            //待確認 新生兒註記就醫
            dData += "<d44>" + "" + "</d44>";
            //待確認 矯正機關代號
            dData += "<d45>" + "" + "</d45>";
            //特定地區醫療服務 免填
            dData += "</dhead>";
            foreach (var detail in declareData.DeclareDetails)
            {
                var pData = "<pdata>";
                pData += "<p1>" + detail.MedicalOrder + "</p1>";
                pData += "<p2>" + detail.MedicalId + "</p2>";
                pData += "<p7>" + detail.Total + "</p7>";
                pData += "<p8>" + detail.Price + "</p8>";
                pData += "<p9>" + detail.Point + "</p9>";
                if (detail.Dosage.ToString(CultureInfo.InvariantCulture) != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p3>" + detail.Dosage + "</p3>";
                if (detail.Usage != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p4>" + detail.Usage + "</p4>";
                if (detail.Position != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p5>" + detail.Position + "</p5>";
                if (detail.Percent.ToString(CultureInfo.InvariantCulture) != DBNull.Value.ToString(CultureInfo.InvariantCulture)) pData += "<p6>" + detail.Percent + "</p6>";
                pData += "<p10>" + detail.Sequence + "</p10>";
                if (detail.Days.ToString() != DBNull.Value.ToString(CultureInfo.InvariantCulture))
                {
                    pData += "<p11>" + detail.Days + "</p11>";
                    if (Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) < detail.Days)
                        declareData.Prescription.Treatment.MedicineDays = detail.Days.ToString();
                }
                pData += "</pdata>";
                dData += pData;
            }
            dData += "</ddata>";
            xml.LoadXml(dData);
            return xml;
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
    }
}
