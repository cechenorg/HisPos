using His_Pos.Class.Person;
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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.RDLC;
// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace His_Pos.Class.Declare
{
    public class DeclareDb
    {
        private Function function = new Function();
        /*
         * 新增DeclareData至資料庫
         */

        public void InsertDb(DeclareData declareData, DeclareTrade declareTrade = null)
        {
            var parameters = new List<SqlParameter>();
            AddParameterDData(parameters, declareData);//加入DData sqlparameters
            var pDataTable = SetPDataTable();//設定PData datatable columns
            AddPData(declareData, pDataTable);//加入PData sqlparameters
            if (declareTrade != null)
            {
                var dataTradeTable = SetDataTradeTable();
                AddTradeData(declareTrade, dataTradeTable);
                parameters.Add(new SqlParameter("DECLARETRADE", dataTradeTable));
            }
            else
                parameters.Add(new SqlParameter("DECLARETRADE", DBNull.Value));
            parameters.Add(new SqlParameter("DETAIL", pDataTable));
            parameters.Add(new SqlParameter("XML", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(SerializeObject<Ddata>(CreatDeclareDataXmlObject(declareData)), XmlNodeType.Document, null))
            });
            CheckInsertDbTypeUpdate(parameters);
        }

        private string SerializeObject<T>(Ddata ddata)
        {
            var xmlSerializer = new XmlSerializer(ddata.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ddata);
                var document = XDocument.Parse(ReportService.PrettyXml(textWriter));
                document.Descendants()
                    .Where(e => e.IsEmpty || String.IsNullOrWhiteSpace(e.Value))
                    .Remove();
                return document.ToString().Replace("<ddata xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<ddata>");
            }
        }

        private Ddata CreatDeclareDataXmlObject(DeclareData declareData)
        {
            var p = declareData.Prescription;
            var c = p.Customer;
            var t = p.Treatment;
            var m = t.MedicalInfo;
            var ic = c.IcCard;
            var year = (int.Parse(c.Birthday.Substring(0, 3)) + 1911).ToString();
            var cusBirth = year + c.Birthday.Substring(3, 6);
            var ddata = new Ddata
            {
                Dhead = new Dhead {D1 = declareData.Prescription.Treatment.AdjustCase.Id},
                Dbody = new Dbody
                {
                    D3 = c.IcNumber,
                    D5 = t.PaymentCategory.Id,
                    D6 = DateTimeExtensions.ToSimpleTaiwanDate(Convert.ToDateTime(cusBirth)),
                    D7 = CheckXmlEmptyValue(ic.MedicalNumber),
                    D8 = CheckXmlEmptyValue(m.MainDiseaseCode.Id),
                    D9 = CheckXmlEmptyValue(m.SecondDiseaseCode.Id),
                    D13 = CheckXmlEmptyValue(m.Hospital.Division.Id),
                    D14 = CheckXmlEmptyValue(DateTimeExtensions.ToSimpleTaiwanDate(t.TreatmentDate)),
                    D15 = CheckXmlEmptyValue(t.Copayment.Id),
                    D16 = declareData.DeclarePoint.ToString(),
                    D17 = declareData.CopaymentPoint.ToString(),
                    D18 = declareData.TotalPoint.ToString(),
                    D19 = CheckXmlEmptyValue(declareData.AssistProjectCopaymentPoint.ToString()),
                    D20 = c.Name,
                    D21 = CheckXmlEmptyValue(m.Hospital.Id),
                    D22 = CheckXmlEmptyValue(m.TreatmentCase.Id),
                    D23 = CheckXmlEmptyValue(DateTimeExtensions.ToSimpleTaiwanDate(t.AdjustDate)),
                    D25 = t.MedicalPersonId,
                    D30 = t.MedicineDays,
                    D31 = declareData.SpecailMaterialPoint.ToString(),
                    D32 = declareData.DiagnosisPoint.ToString(),
                    D33 = declareData.DrugsPoint.ToString(),
                    D35 = CheckXmlEmptyValue(p.ChronicSequence),
                    D36 = CheckXmlEmptyValue(p.ChronicTotal),
                    D37 = declareData.MedicalServiceCode,
                    D38 = declareData.MedicalServicePoint.ToString(),
                    D44 = ic.IcMarks.NewbornsData.Birthday
                }
            };
            if (!string.IsNullOrEmpty(declareData.DeclareMakeUp))
            {
                ddata.Dbody.D4 = declareData.DeclareMakeUp;
            }

            if (!string.IsNullOrEmpty(t.AdjustCase.Id) && (t.AdjustCase.Id.StartsWith("D") || t.AdjustCase.Id.StartsWith("5")))
            {
                if (string.IsNullOrEmpty(m.Hospital.Doctor.Id))
                {
                    m.Hospital.Doctor.Id = m.Hospital.Id;
                }
                ddata.Dbody.D24 = m.Hospital.Doctor.Id;
                ddata.Dbody.D26 = t.MedicalInfo.SpecialCode.Id;
            }

            if (!string.IsNullOrEmpty(p.ChronicSequence))
            {
                if (int.Parse(p.ChronicSequence) >= 2)
                {
                    ddata.Dbody.D43 = p.OriginalMedicalNumber;
                }
            }

            if (declareData.DeclareDetails.Count <= 0) return ddata;
            ddata.Dbody.Pdata = new List<Pdata>();
            var declareCount = 1;
            var specialCount = 0;
            foreach (var detail in declareData.DeclareDetails)
            {
                var pdata = new Pdata
                {
                    P1 = detail.MedicalOrder,
                    P2 = detail.MedicalId,
                    P3 = function.SetStrFormat(detail.Dosage, "{0:0000.00}"),
                    P4 = detail.Usage,
                    P5 = detail.Position,
                    P6 = detail.Percent.ToString(),
                    P7 = function.SetStrFormat(detail.Total, "{0:00000.0}"),
                    P8 = function.SetStrFormat(detail.Price, "{0:0000000.00}"),
                    P9 = function.SetStrFormatInt(Convert.ToInt32(Math.Truncate(Math.Round(detail.Point, 0, MidpointRounding.AwayFromZero))), "{0:D8}"),
                    P10 = function.SetStrFormatInt(declareData.DeclareDetails.Count + 1, "{0:D3}"),
                    P11 = detail.Days.ToString(),
                    P12 = detail.StartDate,
                    P13 = detail.EndDate,
                };
                if (pdata.P1.Equals("D") || pdata.P1.Equals("E") || pdata.P1.Equals("F"))
                {
                    specialCount++;
                    pdata.P15 = specialCount.ToString();
                }
                ddata.Dbody.Pdata.Add(pdata);
                declareCount++;
            }
            return ddata;
        }

        string CheckXmlEmptyValue(string value)
        {
            if(value != null)
                return value.Length > 0 ? value : string.Empty;
            return null;
        }

        /*
         * 加入DData資料之parameters
         */

        private void AddParameterDData(ICollection<SqlParameter> parameters, DeclareData declareData)
        {
            AddParameterTreatment(parameters, declareData);
            //4補報註記 7就醫序號 16申請點數 17部分負擔點數 18合計點數 19行政協助部分負擔點數 31特殊材料明細點數小計 32診療明細點數小計 33用藥明細點數小計 38藥事服務費點數
            var tagsDictionary = new Dictionary<string, string>
            {
                {"D4", declareData.DeclareMakeUp}, {"D7", declareData.Prescription.Customer.IcCard.MedicalNumber},
                {"D16", declareData.DeclarePoint.ToString()}, {"D17", declareData.CopaymentPoint.ToString()},
                {"D18", declareData.TotalPoint.ToString()},{"D19", declareData.AssistProjectCopaymentPoint.ToString()},
                {"D31",declareData.SpecailMaterialPoint.ToString()},{"D32",declareData.DiagnosisPoint.ToString()},
                {"D33",declareData.DrugsPoint.ToString()},{"D37",declareData.MedicalServiceCode},
                {"D38",declareData.MedicalServicePoint.ToString()}
            };
            foreach (var tag in tagsDictionary)
            {
                if (tag.Key == "D4" || tag.Key == "D19" || tag.Key == "D31" || tag.Key == "D32" || tag.Key == "D33")
                    CheckDbNullValue(parameters, tag.Value, tag.Key);
                else
                {
                    parameters.Add(new SqlParameter(tag.Key, tag.Value));
                }
            }
            AddUnusedParameters(parameters);//設定免填欄位Parameters D10 D11 D12 D27 D28 D29
            CheckChronicAdjust(declareData, parameters);//判斷慢箋調劑欄位D35 D36
        }

        /*
         * 加入DeclareData.Treatment資料之parameters
         */

        private void AddParameterTreatment(ICollection<SqlParameter> parameters, DeclareData declareData)
        {
            AddParameterMedicalInfo(parameters, declareData);

            var tagsDictionary = new Dictionary<string, string>
            {
                {"D1", declareData.Prescription.Treatment.AdjustCase.Id},{"D5", declareData.Prescription.Treatment.PaymentCategory.Id},
                {"D14",DateTimeExtensions.ToSimpleTaiwanDate(declareData.Prescription.Treatment.TreatmentDate)},{"D15", declareData.Prescription.Treatment.Copayment.Id},
                {"D23",DateTimeExtensions.ToSimpleTaiwanDate(declareData.Prescription.Treatment.AdjustDate)},{"D25",declareData.Prescription.Treatment.MedicalPersonId},
                {"D30",declareData.Prescription.Treatment.MedicineDays},{"CUS_ID",declareData.Prescription.Customer.Id}
            };
            foreach (var tag in tagsDictionary)
            {
                if (tag.Key == "D5" || tag.Key == "D14")
                    CheckDbNullValue(parameters, tag.Value, tag.Key);
                else
                {
                    parameters.Add(new SqlParameter(tag.Key, tag.Value));
                }
            }
        }

        /*
         * 加入DeclareData.Treatment.MedicalInfo資料之parameters
         */

        private void AddParameterMedicalInfo(ICollection<SqlParameter> parameters, DeclareData declareData)
        {
            var med = declareData.Prescription.Treatment.MedicalInfo;
            var tagsDictionary = new Dictionary<string, string>
            {
                {"D8", med.MainDiseaseCode.Id}, {"D9", med.SecondDiseaseCode.Id},
                {"D13", med.Hospital.Division.Id}, {"D22", med.TreatmentCase.Id},
                {"D24", med.Hospital.Doctor.Id},{"D26",med.SpecialCode.Id},{"D21",declareData.Prescription.Treatment.MedicalInfo.Hospital.Id}
            };
            foreach (var tag in tagsDictionary)
            {
                if (tag.Key == "D21")
                    parameters.Add(new SqlParameter(tag.Key, tag.Value));
                else
                {
                    CheckDbNullValue(parameters, tag.Value, tag.Key);
                }
            }
        }
        private DataTable SetDataTradeTable() {
            var dataTradeTable = new DataTable();
            var columnsDictionary = new Dictionary<string, Type>
            {
                {"CUS_ID", typeof(string)}, {"EMP_ID", typeof(string)},{"PAYSELF", typeof(string)},
                {"DEPOSIT", typeof(string)},{"RECEIVE_MONEY", typeof(string)},{"COPAYMENT",typeof(string)},
                {"PAYMONEY",typeof(string)},{"CHANGE",typeof(string)},{"PAYWAY",typeof(string)}
            };
            foreach (var col in columnsDictionary)
            {
                dataTradeTable.Columns.Add(col.Key, col.Value);
            }
            return dataTradeTable;
        }

        private void AddTradeData(DeclareTrade declareTrade, DataTable tradeTable)
        {
            var row = tradeTable.NewRow();
            var tagsDictionary = new Dictionary<string, string>
                {
                    {"CUS_ID",declareTrade.CusId},
                    {"EMP_ID", declareTrade.EmpId},
                    {"PAYSELF", declareTrade.PaySelf},
                    {"DEPOSIT", declareTrade.Deposit},
                    {"RECEIVE_MONEY", declareTrade.ReceiveMoney},
                    {"COPAYMENT", declareTrade.CopayMent},
                    {"PAYMONEY", declareTrade.PayMoney},
                    {"CHANGE", declareTrade.Change},
                    {"PAYWAY", declareTrade.PayWay},
                };
            foreach (var tag in tagsDictionary)
            {
                switch (tag.Key)
                {
                    default:
                        CheckEmptyDataRow(tradeTable, tag.Value, ref row, tag.Key);
                        break;
                }
            }
            tradeTable.Rows.Add(row);
        }
        private DataTable SetPDataTable()
        {
            var pDataTable = new DataTable();
            var columnsDictionary = new Dictionary<string, Type>
            {
                {"P10", typeof(int)}, {"P1", typeof(string)},{"P2", typeof(string)},
                {"P7", typeof(double)},{"P8", typeof(double)},{"P9",typeof(int)},
                {"P3",typeof(double)},{"P4",typeof(string)},{"P5",typeof(string)},
                {"P6",typeof(string)},{"P11",typeof(string)},{"P12",typeof(string)},
                {"P13",typeof(string)},{"PAY_BY_YOURSELF",typeof(string)}
            };
            foreach (var col in columnsDictionary)
            {
                pDataTable.Columns.Add(col.Key, col.Value);
            }
            return pDataTable;
        }
     
        private void AddPData(DeclareData declareData, DataTable pDataTable)
        {
            for (var i = 0; i < declareData.DeclareDetails.Count; i++)
            {
                var row = pDataTable.NewRow();
                var detail = declareData.DeclareDetails[i];
                var paySelf = declareData.Prescription.Medicines == null ? "0" : declareData.Prescription.Medicines[i].PaySelf ? "1" : "0";
                var tagsDictionary = new Dictionary<string, string>
                {
                    {"P1", detail.MedicalOrder}, {"P2", detail.MedicalId},
                    {"P3", function.SetStrFormat(detail.Dosage,"{0:0000.00}")}, {"P4", detail.Usage},
                    {"P5", detail.Position},{"P6",function.ToInvCulture(detail.Percent)},
                    {"P7",function.SetStrFormat(detail.Total,"{0:00000.0}")},{"P8",function.SetStrFormat(detail.Price,"{0:0000000.00}")},
                    {"P9",function.SetStrFormatInt(Convert.ToInt32(Math.Truncate(Math.Round(detail.Point,0,MidpointRounding.AwayFromZero))),"{0:D8}")},{"P10",detail.Sequence.ToString()},
                    {"P11",detail.Days.ToString()},{"PAY_BY_YOURSELF",paySelf}
                };
                foreach (var tag in tagsDictionary)
                {
                    switch (tag.Key)
                    {
                        case "P10":
                            row[tag.Key] = Convert.ToInt32(tag.Value);
                            break;

                        case "PAY_BY_YOURSELF":
                            row[tag.Key] = tag.Value;
                            break;

                        default:
                            CheckEmptyDataRow(pDataTable, tag.Value, ref row, tag.Key);
                            break;
                    }
                }
                pDataTable.Rows.Add(row);
            }
            AddMedicalServiceCostPData(declareData, pDataTable);
        }

        /*
         * 加入藥事服務費之PData
         */

        private void AddMedicalServiceCostPData(DeclareData declareData, DataTable pDataTable)
        {
            var percent = CountAdditionPercent(declareData);
            var currentDate = DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            var detail = new DeclareDetail(declareData.MedicalServiceCode, percent, declareData.MedicalServicePoint, declareData.DeclareDetails.Count + 1, currentDate, currentDate);
            var pData = pDataTable.NewRow();
            SetMedicalServiceCostDataRow(pData, declareData, detail);
            declareData.DeclareDetails.Add(detail);
            pDataTable.Rows.Add(pData);
        }

        /*
         * 計算支付成數
         */

        private double CountAdditionPercent(DeclareData declareData)
        {
            double percent = 0;
            var cusBirth = declareData.Prescription.Customer.Birthday;
            var month = DateTimeExtensions.CalculateAge(DateTimeExtensions.ToUsDate(cusBirth));
            if (month < 0.5) percent = 160;
            if (month > 0.5 && month <= 2) percent = 130;
            if (month > 2 && month <= 6) percent = 120;
            return percent;
        }

        /*
         * 設定藥事服務費PData之DataRow
         */

        private void SetMedicalServiceCostDataRow(DataRow pData, DeclareData declareData, DeclareDetail detail)
        {
            var declarecount = declareData.DeclareDetails.Count + 1;//藥事服務醫令序
            var tagsDictionary = new Dictionary<string, object>
            {
                {"P1",detail.MedicalOrder},{"P2",detail.MedicalId},
                {"P6",function.ToInvCulture(detail.Percent)},{"P7",function.SetStrFormat(detail.Total,"{0:00000.0}")},
                {"P8",function.SetStrFormat(detail.Price,"{0:0000000.00}")},{"P9",function.SetStrFormatInt(Convert.ToInt32(Math.Truncate(Math.Round(detail.Point,0,MidpointRounding.AwayFromZero))),"{0:D8}")},
                {"P10",function.SetStrFormatInt(declarecount,"{0:D3}")},{"P12",detail.StartDate},{"P13",detail.EndDate}
            };
            foreach (var tag in tagsDictionary)
            {
                pData[tag.Key] = tag.Value;
            }
        }

        /*
         * 判斷慢箋調劑
         */

        private void CheckChronicAdjust(DeclareData declareData, ICollection<SqlParameter> parameters)
        {
            if (!declareData.Prescription.Treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId))//判斷是否為慢箋調劑
            {
                parameters.Add(new SqlParameter("D35", DBNull.Value));
                parameters.Add(new SqlParameter("D36", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("D35", declareData.Prescription.ChronicSequence));
                parameters.Add(new SqlParameter("D36", declareData.Prescription.ChronicTotal));
            }
            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId) && Convert.ToInt32(declareData.Prescription.ChronicSequence) > 1)
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
            var tagList = new List<string>() { "D10", "D11", "D12", "D27", "D28", "D29" };
            foreach (var tag in tagList)
            {
                parameters.Add(new SqlParameter(tag, DBNull.Value));
            }
        }

        /*
         * 判斷InsertDb type為Update
         */

        private void CheckInsertDbTypeUpdate(List<SqlParameter> parameters)
        {
            var conn = new DbConnection(Settings.Default.SQL_global);
            conn.ExecuteProc("[HIS_POS_DB].[SET].[DECLAREDATA]", parameters);
            //if (type == "Update")
            //{
            //    parameters.Add(new SqlParameter("ID", id));
            //    conn.ExecuteProc("[HIS_POS_DB].[SET].[UPDATEDECLAREDATA]", parameters);
            //}
            
        }

        private XmlDocument CreateToXml(DeclareData declareData)
        {
            var xml = new XmlDocument();
            var dData = SetDheadXml(declareData);
            foreach (var detail in declareData.DeclareDetails)
            {
                dData += SetPDataXmlStr(detail, declareData);
            }
            dData += "</ddata>";
            xml.LoadXml(dData);
            return xml;
        }

        private string SetDheadXml(DeclareData declareData)
        {
            var dData = "<ddata><dhead>";
            var treatment = declareData.Prescription.Treatment;
            var medicalInfo = treatment.MedicalInfo;
            var dDataDictionary = SetDheadDictionary(declareData, treatment, medicalInfo);
            foreach (var tag in dDataDictionary)
            {
                if (tag.Value != string.Empty)
                    dData += function.XmlTagCreator(tag.Key, tag.Value);
            }

            if (treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId))
            {
                if (Convert.ToDecimal(declareData.Prescription.ChronicSequence) >= 2)
                    dData += function.XmlTagCreator("d43", declareData.Prescription.OriginalMedicalNumber);
            }
            if (treatment.Copayment.Id == "903")
                dData += function.XmlTagCreator("d44", CheckXmlDbNullValue(declareData.Prescription.Customer.IcCard.IcMarks.NewbornsData.Birthday));//新生兒註記就醫
            dData += "</dhead>";
            return dData;
        }

        /*
         * 設定DData dhead資料並以Dictionary結構回傳
         */

        private Dictionary<string, string> SetDheadDictionary(DeclareData declareData, Treatment treatment, MedicalInfo medicalInfo)
        {
            string d8 = string.Empty, d9 = string.Empty, d35 = declareData.Prescription.ChronicSequence, d36 = declareData.Prescription.ChronicTotal;
            string year = (int.Parse(declareData.Prescription.Customer.Birthday.Substring(0, 3)) + 1911).ToString();
            string cusBirth = year + declareData.Prescription.Customer.Birthday.Substring(3, 6);
            if (medicalInfo.MainDiseaseCode != null)
            {
                d8 = medicalInfo.MainDiseaseCode.Id;
                if (medicalInfo.SecondDiseaseCode != null)
                    d9 = medicalInfo.SecondDiseaseCode.Id;
            }
            return new Dictionary<string, string>
            {
                {"d1",treatment.AdjustCase.Id},{"d2",string.Empty},{"d3",declareData.Prescription.Customer.IcNumber},
                {"d4",CheckXmlDbNullValue(declareData.DeclareMakeUp)},{"d5",CheckXmlDbNullValue(treatment.PaymentCategory.Id)},
                {"d6",DateTimeExtensions.ToSimpleTaiwanDate(Convert.ToDateTime(cusBirth))},{"d7",declareData.Prescription.Customer.IcCard.MedicalNumber},{"d8",d8},{"d9",d9},
                {"d13",CheckXmlDbNullValue(medicalInfo.Hospital.Division.Id)},{"d14",CheckXmlDbNullValue(DateTimeExtensions.ToSimpleTaiwanDate(treatment.TreatmentDate))},
                {"d15",treatment.Copayment.Id},{"d16",declareData.DeclarePoint.ToString()},
                {"d17",treatment.Copayment.Point.ToString()},{"d18",declareData.TotalPoint.ToString()},
                {"d19",CheckXmlDbNullValue(declareData.AssistProjectCopaymentPoint.ToString())},{"d20",declareData.Prescription.Customer.Name},
                {"d21",medicalInfo.Hospital.Id},{"d22",medicalInfo.TreatmentCase.Id},{"d23",DateTimeExtensions.ToSimpleTaiwanDate(treatment.AdjustDate)},
                {"d24",medicalInfo.Hospital.Doctor.Id},{"d25",treatment.MedicalPersonId},
                {"d26",CheckXmlDbNullValue(medicalInfo.SpecialCode.Id)},{"d30",CheckXmlDbNullValue(treatment.MedicineDays)},
                {"d31",CheckXmlDbNullValue(declareData.SpecailMaterialPoint.ToString())},
                {"d32",CheckXmlDbNullValue(declareData.DiagnosisPoint.ToString())},
                {"d33",CheckXmlDbNullValue(declareData.DrugsPoint.ToString())},{"d35",d35},{"d36",d36},
                {"d37",declareData.MedicalServiceCode},{"d38",declareData.MedicalServicePoint.ToString()},
            };
        }

        private string SetPDataXmlStr(DeclareDetail detail, DeclareData declareData)
        {
            var pData = "<pdata>";
            var pDataDictionary = SetPDataDictionary(detail);
            foreach (var tag in pDataDictionary)
            {
                if (tag.Value != string.Empty)
                    pData += function.XmlTagCreator(tag.Key, tag.Value);
            }
            if (detail.Days.ToString() != string.Empty)
            {
                if (Convert.ToInt32(declareData.Prescription.Treatment.MedicineDays) < detail.Days)
                    declareData.Prescription.Treatment.MedicineDays = detail.Days.ToString();
            }
            pData += "</pdata>";
            return pData;
        }

        /*
         * 設定PData資料並以Dictionary結構回傳
         */

        private Dictionary<string, string> SetPDataDictionary(DeclareDetail detail)
        {
            return new Dictionary<string, string>
            {
                {"p1",detail.MedicalOrder},{"p2",detail.MedicalId},{"p3",CheckXmlDbNullValue(function.ToInvCulture(detail.Dosage))},
                {"p4",CheckXmlDbNullValue(detail.Usage)},{"p5",CheckXmlDbNullValue(detail.Position)},{"p6",CheckXmlDbNullValue(function.ToInvCulture(detail.Percent))},
                {"p7",detail.Total.ToString()},{"p8",detail.Price.ToString()},{"p9",detail.Point.ToString()},
                {"p10",detail.Sequence.ToString()},{"p11",CheckXmlDbNullValue(detail.Days.ToString())}
            };
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
            int casid1, casid2, casid3, totalcasid;
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
                        switch (d1)
                        {
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
                            xml.SelectSingleNode("ddata/dhead/d37").InnerText = "05234D";
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
                xmlddata += xml.InnerXml;
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
            xmlsum += "<t11>" + ((int)ncount + (int)scount) + "</t11>"; //申請件數總計
            xmlsum += "<t12>" + ((int)npoint + (int)spoint) + "</t12>"; //申請點數總計
            var table1 = conn.ExecuteProc("[HIS_POS_DB].[GET].[DECLARESDDATE]");
            xmlsum += "<t13>" + table1.Rows[0]["SDATE"] + "</t13>"; //此次連線申報起日期
            xmlsum += "<t14>" + table1.Rows[0]["EDATE"] + "</t14>"; //此次連線申報迄日期
            xmlsum += "</tdata>" + xmlddata + "</pharmacy>";
            var xmlsumx = new XmlDocument();
            xmlsumx.LoadXml(xmlsum);

            //匯出xml檔案
            function.ExportXml(xmlsumx, "匯出申報XML檔案");
        }

        /*
         * 檢查SQLparameter是否為DBNull
         */

        private void CheckDbNullValue(ICollection<SqlParameter> parameters, string value, string paraName)
        {
            if (value == null)
                parameters.Add(new SqlParameter(paraName, DBNull.Value));
            else
            {
                parameters.Add(value.Equals(string.Empty) || value.Equals("0") ? new SqlParameter(paraName, DBNull.Value) : new SqlParameter(paraName, value));
            }
        }

        /*
         * 檢查XmlTag是否為空值
         */

        private string CheckXmlDbNullValue(string value)
        {
            if (value != string.Empty || value != "0")
                return value;
            return string.Empty;
        }

        /*
         *檢查DataRow是否為空值
         */

        private void CheckEmptyDataRow(DataTable dataTable, string value, ref DataRow row, string rowName)
        {
            if (value != string.Empty)
            {
                switch (dataTable.Columns[rowName].DataType.Name)
                {
                    case "String":
                        row[rowName] = value;
                        break;

                    case "Int32":
                        row[rowName] = Convert.ToInt32(value);
                        break;

                    case "Double":
                        row[rowName] = Convert.ToDouble(value);
                        break;
                }
            }
        }
        
    }
}