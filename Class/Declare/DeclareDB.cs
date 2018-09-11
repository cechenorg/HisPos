using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

        public string InsertDeclareData(DeclareData declareData)
        {
            var parameters = new List<SqlParameter>();
            AddParameterDData(parameters, declareData); //加入DData sqlparameters
            var pDataTable = SetPDataTable(); //設定PData datatable columns
            AddPData(declareData, pDataTable); //加入PData sqlparameters
            
               
            var xmlStr = declareData.SerializeObject<Ddata>();
            var errorStr = declareData.Prescription.EList.SerializeObject<ErrorList>();
            if (string.IsNullOrEmpty(errorStr))
                errorStr = "<ErrorPrescription></ErrorPrescription>";
            parameters.Add(new SqlParameter("DETAIL", pDataTable));
            parameters.Add(new SqlParameter("XML", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(xmlStr, XmlNodeType.Document, null))
            });

            parameters.Add(new SqlParameter("HISDECMAS_ERRORMSG", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader((string) errorStr, XmlNodeType.Document, null))
            });
            var conn = new DbConnection(Settings.Default.SQL_global);
           DataTable table =  conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[InsertDeclareData]", parameters);
            return table.Rows[0][0].ToString();//回傳DesMasId
        }
        public void InsertDeclareTrade(string decMasId,DeclareTrade declareTrade) {
            var dataTradeTable = SetDataTradeTable();
            AddTradeData(declareTrade, dataTradeTable);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DECMAS_ID", decMasId));
            parameters.Add(new SqlParameter("DECLARETRADE", dataTradeTable));
            var conn = new DbConnection(Settings.Default.SQL_global);
            conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[InsertTradeData]", parameters);
        }

        public void UpdateDeclareFile(DeclareData declareData)
        {
            var declareDate = declareData.Prescription.Treatment.AdjustDate;
            var p = new Pharmacy
            {
                Ddata = GetDdataList(declareData.Prescription.Treatment.AdjustDate)
            };
            var sortedCaseList = SortDdataByCaseId(p);

            var tdata = new Tdata
            {
                T1 = "30",
                T2 = MainWindow.CurrentPharmacy.Id,
                T3 = (declareDate.Year - 1911) + declareDate.Month.ToString().PadLeft(2, '0'),
                T4 = "2",
                T5 = "1",
                T6 = (DateTime.Now.Year - 1911) + DateTime.Now.Month.ToString().PadLeft(2, '0'),
                T7 = CountPrescriptionByCase(sortedCaseList, 1).ToString(),
                T8 = sortedCaseList.Where(d => !d.Dhead.D1.Equals("2")).Sum(d => int.Parse(d.Dbody.D16)).ToString(),
                T9 = CountPrescriptionByCase(sortedCaseList, 2).ToString(),
                T10 = sortedCaseList.Where(d => d.Dhead.D1.Equals("2")).Sum(d => int.Parse(d.Dbody.D16)).ToString(),
                T11 = sortedCaseList.Count.ToString()
            };
            tdata.T12 = (int.Parse(tdata.T8) + int.Parse(tdata.T10)).ToString();
            tdata.T13 = GetDateStr(DateTime.Now, true);
            tdata.T14 = GetDateStr(declareDate, false);
            p.Tdata = tdata;
            p.Ddata = sortedCaseList;

            var file = DeclareFileDb.GetDeclareFileTypeLogIn(declareDate);
            file.FileContent = p;
            file.ErrorPrescriptionList = new ErrorPrescriptions {ErrorList = new List<ErrorList>()};
            file.ErrorPrescriptionList.ErrorList = PrescriptionDB.GetPrescriptionErrorLists(declareDate);
            DeclareFileDb.SetDeclareFileByPharmacyId(file, declareDate, DeclareFileType.LOG_IN);
        }
        public void UpdateDeclareData(DeclareData declareData, DeclareTrade declareTrade = null)
        {
            var conn = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            AddParameterDData(parameters, declareData); //加入DData sqlparameters
            var pDataTable = SetUpdatePDataTable(); //設定PData datatable columns
            AddPData(declareData, pDataTable); //加入PData sqlparameters
        }

        private List<Ddata> SortDdataByCaseId(Pharmacy p)
        {
            var normalCaseList = p.Ddata.OrderBy(d => d.Dbody.D38);
            var sorted = new List<Ddata>();
            var result = normalCaseList.GroupBy(d => d.Dhead.D1);
            foreach (var group in result)
            {
                sorted.AddRange(@group);
            }
            return sorted;
        }

        private List<Ddata> GetDdataList(DateTime declareDate)
        {
            if (PrescriptionDB.GetPrescriptionXmlByDate(declareDate).Count == 0) return new List<Ddata>();
            //依照藥事服務費點數排序
            var ddatas = PrescriptionDB.GetPrescriptionXmlByDate(declareDate).OrderBy(d => d.Dbody.D38)
                .ToList();
            /*
             * 藥事服務費每人每日81 - 100件內 => 診療項目代碼: 05234D . 支付點數 : 15
             *          每人每日100件以上 => 診療項目代碼: 0502B . 支付點數 : 0
             */
            for (var i = 0; i < ddatas.Count; i++)
            {
                if (i < 80)
                {
                    ddatas[i].Dbody.D37 = "0502B";
                    ddatas[i].Dbody.D38 = "48";
                }
                else if (i < 100 && i >= 80)
                {
                    ddatas[i].Dbody.D37 = "05234D";
                    ddatas[i].Dbody.D38 = "15";
                }
                else
                {
                    ddatas[i].Dbody.D37 = "0502B";
                    ddatas[i].Dbody.D38 = "0";
                }
            }
            return ddatas;
        }

        private string GetDateStr(DateTime d, bool now)
        {
            if (now)
                return d.Year - 1911 + d.Month.ToString().PadLeft(2, '0') + "01";
            return d.Year - 1911 + d.Month.ToString().PadLeft(2, '0') + d.Day.ToString().PadLeft(2, '0');
        }

        private int CountPrescriptionByCase(List<Ddata> listDdata, int caseType)
        {
            List<Ddata> normalCaseDdata = new List<Ddata>();
            if (caseType == 1)
            {
                normalCaseDdata = listDdata.Where(d =>
                    d.Dhead.D1.Equals("1") || d.Dhead.D1.Equals("3") || d.Dhead.D1.Equals("4") || d.Dhead.D1.Equals("5") ||
                    d.Dhead.D1.Equals("D")).ToList();
            }
            else
            {
                normalCaseDdata = listDdata.Where(d => d.Dhead.D1.Equals("2")).ToList();
            }
            return normalCaseDdata.Count;
        }

        /*
         * 匯入申報檔
         */
        public void ImportDeclareData(ObservableCollection<DeclareData> declareDataCollection, string decId)
        {
            var conn = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();

            var customerTable = SetCustomerTable();
            var declareMasterTable = SetDeclareMasterTable();
            var pDataTable = SetImportPDataTable(); //設定PData datatable columns
            
            foreach (DeclareData declaredata in declareDataCollection)
            {
                AddDeclareMaster(declaredata, declareMasterTable);
                AddImportPData(declaredata, pDataTable); //加入PData sqlparameters
                AddCustomer(declaredata, customerTable);
            }

            parameters.Add(new SqlParameter("DecId", decId));
            parameters.Add(new SqlParameter("DeclareMasterTable", declareMasterTable));
            parameters.Add(new SqlParameter("DETAIL", pDataTable));
            parameters.Add(new SqlParameter("Customer", customerTable));
            conn.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[ImportDeclareMaster]", parameters);
        }

        public string CheckXmlFileExist(XmlDocument xml)
        {
            var conn = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("XML", xml.SelectSingleNode("pharmacy/tdata").InnerXml));
            var table = conn.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[CheckXMLFileExist]",
                parameters);
            if (table.Rows[0]["IsCheck"].ToString() == "0")
                return string.Empty;
            else
                return table.Rows[0]["DECID"].ToString();
        }

        /*
         * 藥品扣庫
         */
        public void InsertInventoryDb(DeclareData declareData, string way,string decMasId)
        {
            var parameters = new List<SqlParameter>();
            var conn = new DbConnection(Settings.Default.SQL_global);
            foreach (DeclareMedicine declareDetail in declareData.Prescription.Medicines)
            {
                parameters.Clear();
                parameters.Add(new SqlParameter("MAS_ID", decMasId));
                parameters.Add(new SqlParameter("PRO_ID", declareDetail.Id));
                parameters.Add(new SqlParameter("BUCKLE_VALUE", declareDetail.Amount));
                parameters.Add(new SqlParameter("BUCKLE_STATUS", "1"));
                parameters.Add(new SqlParameter("WAY", way));
                conn.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[InsertDeclareDetailBuckle]",
                    parameters);
            }
        }

        public int GetMaxDecMasId()
        {
            var conn = new DbConnection(Settings.Default.SQL_global);
            var table = conn.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[GetMaxDecMasId]");
            return Convert.ToInt32(table.Rows[0][0].ToString());
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
                        {"D4", declareData.DeclareMakeUp},
                        {"D7", declareData.Prescription.Customer.IcCard.MedicalNumber},
                        {"D16", declareData.DeclarePoint.ToString()},
                        {"D17", declareData.CopaymentPoint.ToString()},
                        {"D18", declareData.TotalPoint.ToString()},
                        {"D19", declareData.AssistProjectCopaymentPoint.ToString()},
                        {"D31", declareData.SpecailMaterialPoint.ToString()},
                        {"D32", declareData.DiagnosisPoint.ToString()},
                        {"D33", declareData.DrugsPoint.ToString()},
                        {"D37", declareData.MedicalServiceCode},
                        {"D38", declareData.MedicalServicePoint.ToString()}
                    };
            foreach (var tag in tagsDictionary)
            {
                if (tag.Key == "D4" || tag.Key == "D19" || tag.Key == "D31" || tag.Key == "D32" ||
                    tag.Key == "D33")
                    CheckDbNullValue(parameters, tag.Value, tag.Key);
                else
                {
                    parameters.Add(new SqlParameter(tag.Key, tag.Value));
                }
            }

            AddUnusedParameters(parameters); //設定免填欄位Parameters D10 D11 D12 D27 D28 D29
            CheckChronicAdjust(declareData, parameters); //判斷慢箋調劑欄位D35 D36
        }

        /*
         * 加入DeclareData.Treatment資料之parameters
         */

        private void AddParameterTreatment(ICollection<SqlParameter> parameters, DeclareData declareData)
        {
            AddParameterMedicalInfo(parameters, declareData);

            var tagsDictionary = new Dictionary<string, string>
                    {
                        {"D1", declareData.Prescription.Treatment.AdjustCase.Id},
                        {"D5", declareData.Prescription.Treatment.PaymentCategory.Id},
                        {
                            "D14",
                            DateTimeExtensions.ToSimpleTaiwanDate((DateTime)declareData.Prescription.Treatment.TreatmentDate)
                        },
                        {"D15", declareData.Prescription.Treatment.Copayment.Id},
                        {"D23", DateTimeExtensions.ToSimpleTaiwanDate((DateTime)declareData.Prescription.Treatment.AdjustDate)},
                        {"D25", declareData.Prescription.Treatment.MedicalPersonId},
                        {"D30", declareData.Prescription.Treatment.MedicineDays},
                        {"CUS_ID", declareData.Prescription.Customer.Id}
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
                        {"D8", med.MainDiseaseCode.Id},
                        {"D9", med.SecondDiseaseCode.Id},
                        {"D13", med.Hospital.Division.Id},
                        {"D22", med.TreatmentCase.Id},
                        {"D24", med.Hospital.Doctor.Id},
                        {"D26", med.SpecialCode.Id},
                        {"D21", declareData.Prescription.Treatment.MedicalInfo.Hospital.Id}
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

        private DataTable SetDataTradeTable()
        {
            var dataTradeTable = new DataTable();
            var columnsDictionary = new Dictionary<string, Type>
                    {
                        {"CUS_ID", typeof(int)},
                        {"EMP_ID", typeof(string)},
                        {"PAYSELF", typeof(int)},
                        {"DEPOSIT", typeof(int)},
                        {"RECEIVE_MONEY", typeof(int)},
                        {"COPAYMENT", typeof(int)},
                        {"PAYMONEY", typeof(int)},
                        {"CHANGE", typeof(int)},
                        {"PAYWAY", typeof(string)}
                    };
            foreach (var col in columnsDictionary)
            {
                dataTradeTable.Columns.Add(col.Key, col.Value);
            }

            return dataTradeTable;
        }

        private void AddTradeData(DeclareTrade declareTrade, DataTable tradeTable)
        {
            if (declareTrade == null) return;
            var row = tradeTable.NewRow();

            row["CUS_ID"] = Convert.ToInt32(declareTrade.CusId);

            if (String.IsNullOrEmpty(declareTrade.EmpId))
                row["EMP_ID"] = DBNull.Value;
            else
                row["EMP_ID"] = declareTrade.EmpId;

            if (String.IsNullOrEmpty(declareTrade.PaySelf))
                row["PAYSELF"] = DBNull.Value;
            else
                row["PAYSELF"] = Convert.ToInt32(declareTrade.PaySelf);

            if (String.IsNullOrEmpty(declareTrade.Deposit))
                row["DEPOSIT"] = DBNull.Value;
            else
                row["DEPOSIT"] = Convert.ToInt32(declareTrade.Deposit);

            if (String.IsNullOrEmpty(declareTrade.ReceiveMoney))
                row["RECEIVE_MONEY"] = DBNull.Value;
            else
                row["RECEIVE_MONEY"] = Convert.ToInt32(declareTrade.ReceiveMoney);

            if (String.IsNullOrEmpty(declareTrade.CopayMent))
                row["COPAYMENT"] = DBNull.Value;
            else
                row["COPAYMENT"] = Convert.ToInt32(declareTrade.CopayMent);

            if (String.IsNullOrEmpty(declareTrade.PayMoney))
                row["PAYMONEY"] = DBNull.Value;
            else
                row["PAYMONEY"] = Convert.ToInt32(declareTrade.PayMoney);

            if (String.IsNullOrEmpty(declareTrade.Change))
                row["CHANGE"] = DBNull.Value;
            else
                row["CHANGE"] = Convert.ToInt32(declareTrade.Change);

            if (String.IsNullOrEmpty(declareTrade.PayWay))
                row["PAYWAY"] = DBNull.Value;
            else
                row["PAYWAY"] = declareTrade.PayWay;
            tradeTable.Rows.Add(row);
        }

        private DataTable SetCustomerTable()
        {
            var CustomerTable = new DataTable();
            CustomerTable.Columns.Add("CUS_NAME", typeof(string));
            CustomerTable.Columns.Add("CUS_BIRTH", typeof(string));
            CustomerTable.Columns.Add("CUS_IDNUM", typeof(string));
            CustomerTable.Columns.Add("CUS_GENDER", typeof(string));
            CustomerTable.Columns.Add("CUS_QNAME", typeof(string));
            return CustomerTable;
        }

        private void AddCustomer(DeclareData declareData, DataTable customerTable)
        {
            var row = customerTable.NewRow();
            row["CUS_NAME"] = declareData.Prescription.Customer.Name;
            row["CUS_BIRTH"] = declareData.Prescription.Customer.Birthday;
            row["CUS_IDNUM"] = declareData.Prescription.Customer.IcCard.IcNumber;
            row["CUS_GENDER"] = declareData.Prescription.Customer.Gender == true ? "1" : "0";
            Function function = new Function();
            row["CUS_QNAME"] = function.ChangeNameToEnglish((string)declareData.Prescription.Customer.Name);
            customerTable.Rows.Add(row);
        }

        private DataTable SetDeclareMasterTable()
        {
            var declareMasterTable = new DataTable();

            declareMasterTable.Columns.Add("DecMasId", typeof(string));
            declareMasterTable.Columns.Add("D1", typeof(string));
            declareMasterTable.Columns.Add("D4", typeof(string));
            declareMasterTable.Columns.Add("D5", typeof(string));
            declareMasterTable.Columns.Add("D6", typeof(string));
            declareMasterTable.Columns.Add("D7", typeof(string));
            declareMasterTable.Columns.Add("D8", typeof(string));
            declareMasterTable.Columns.Add("D9", typeof(string));
            declareMasterTable.Columns.Add("D10", typeof(string));
            declareMasterTable.Columns.Add("D11", typeof(string));
            declareMasterTable.Columns.Add("D12", typeof(string));
            declareMasterTable.Columns.Add("D13", typeof(string));
            declareMasterTable.Columns.Add("CUS_ID", typeof(string));
            declareMasterTable.Columns.Add("D14", typeof(string));
            declareMasterTable.Columns.Add("D15", typeof(string));
            declareMasterTable.Columns.Add("D16", typeof(int));
            declareMasterTable.Columns.Add("D17", typeof(int));
            declareMasterTable.Columns.Add("D18", typeof(int));
            declareMasterTable.Columns.Add("D19", typeof(int));
            declareMasterTable.Columns.Add("D21", typeof(string));
            declareMasterTable.Columns.Add("D22", typeof(string));
            declareMasterTable.Columns.Add("D23", typeof(string));
            declareMasterTable.Columns.Add("D24", typeof(string));
            declareMasterTable.Columns.Add("D25", typeof(string));
            declareMasterTable.Columns.Add("D26", typeof(string));
            declareMasterTable.Columns.Add("D27", typeof(string));
            declareMasterTable.Columns.Add("D28", typeof(string));
            declareMasterTable.Columns.Add("D29", typeof(string));
            declareMasterTable.Columns.Add("D30", typeof(int));
            declareMasterTable.Columns.Add("D31", typeof(int));
            declareMasterTable.Columns.Add("D32", typeof(int));
            declareMasterTable.Columns.Add("D33", typeof(int));
            declareMasterTable.Columns.Add("D35", typeof(int));
            declareMasterTable.Columns.Add("D36", typeof(int));
            declareMasterTable.Columns.Add("D37", typeof(string));
            declareMasterTable.Columns.Add("D38", typeof(int));
            declareMasterTable.Columns.Add("D43", typeof(string));
            declareMasterTable.Columns.Add("XML", typeof(string));
            return declareMasterTable;
        }

        private void AddDeclareMaster(DeclareData declareData, DataTable declareMaster)
        {
            string d8 = string.Empty,
                d9 = string.Empty,
                d35 = declareData.Prescription.ChronicSequence,
                d36 = declareData.Prescription.ChronicTotal;
            string year = (int.Parse(declareData.Prescription.Customer.Birthday.Substring(0, 3)) + 1911)
                .ToString();
            string cusBirth = year + declareData.Prescription.Customer.Birthday.Substring(3, 6);
            if (declareData.Prescription.Treatment.MedicalInfo.MainDiseaseCode != null)
            {
                d8 = declareData.Prescription.Treatment.MedicalInfo.MainDiseaseCode.Id;
                if (declareData.Prescription.Treatment.MedicalInfo.SecondDiseaseCode != null)
                    d9 = declareData.Prescription.Treatment.MedicalInfo.SecondDiseaseCode.Id;
            }

            var row = declareMaster.NewRow();

            row["DecMasId"] = declareData.DecMasId;
            row["D1"] = declareData.Prescription.Treatment.AdjustCase.Id;
            row["D4"] = CheckXmlDbNullValue(declareData.DeclareMakeUp);
            row["D5"] = CheckXmlDbNullValue(declareData.Prescription.Treatment.PaymentCategory.Id);
            row["D6"] = DateTimeExtensions.ToSimpleTaiwanDate(Convert.ToDateTime(cusBirth));
            row["D7"] = declareData.Prescription.Customer.IcCard.MedicalNumber;
            row["D8"] = d8;
            row["D9"] = d9;
            row["CUS_ID"] = declareData.Prescription.Customer.IcCard.IcNumber; //身分證
            row["D11"] = string.Empty;
            row["D12"] = string.Empty;
            row["D13"] =
                CheckXmlDbNullValue(declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id);
            row["D14"] =
                CheckXmlDbNullValue(
                    DateTimeExtensions.ToSimpleTaiwanDate((DateTime)declareData.Prescription.Treatment.TreatmentDate));
            row["D15"] = declareData.Prescription.Treatment.Copayment.Id;
            row["D16"] = declareData.DeclarePoint.ToString();
            row["D17"] = declareData.Prescription.Treatment.Copayment.Point.ToString();
            row["D18"] = declareData.TotalPoint.ToString();
            row["D19"] = CheckXmlDbNullValue(declareData.AssistProjectCopaymentPoint.ToString());
            row["D21"] = declareData.Prescription.Treatment.MedicalInfo.Hospital.Id;
            row["D22"] = declareData.Prescription.Treatment.MedicalInfo.TreatmentCase.Id;
            row["D23"] = DateTimeExtensions.ToSimpleTaiwanDate((DateTime)declareData.Prescription.Treatment.AdjustDate);
            row["D24"] = declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.Id;
            row["D25"] = declareData.Prescription.Treatment.MedicalPersonId;
            row["D26"] = CheckXmlDbNullValue(declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id);
            row["D27"] = string.Empty;
            row["D28"] = string.Empty;
            row["D29"] = string.Empty;
            row["D30"] = CheckXmlDbNullValue(declareData.Prescription.Treatment.MedicineDays);
            row["D31"] = CheckXmlDbNullValue(declareData.SpecailMaterialPoint.ToString());
            row["D32"] = CheckXmlDbNullValue(declareData.DiagnosisPoint.ToString());
            row["D33"] = CheckXmlDbNullValue(declareData.DrugsPoint.ToString());
            if (String.IsNullOrEmpty(d35))
                row["D35"] = DBNull.Value;
            else
                row["D35"] = d35;
            if (String.IsNullOrEmpty(d36))
                row["D36"] = DBNull.Value;
            else
                row["D36"] = d36;
            row["D37"] = declareData.MedicalServiceCode;
            row["D38"] = declareData.MedicalServicePoint.ToString();
            row["D43"] = "";
            row["XML"] = CreateToXml(declareData).InnerXml.ToString();
            declareMaster.Rows.Add(row);
        }
        private DataTable SetUpdatePDataTable()
        {
            var pDataTable = new DataTable();
            var columnsDictionary = new Dictionary<string, Type>
                    {
                        {"DecMasId", typeof(string)},
                        {"P10", typeof(int)},
                        {"P1", typeof(string)},
                        {"P2", typeof(string)},
                        {"P7", typeof(double)},
                        {"P8", typeof(double)},
                        {"P9", typeof(int)},
                        {"P3", typeof(double)},
                        {"P4", typeof(string)},
                        {"P5", typeof(string)},
                        {"P6", typeof(string)},
                        {"P11", typeof(string)},
                        {"P12", typeof(string)},
                        {"P13", typeof(string)},
                        {"PAY_BY_YOURSELF", typeof(string)}
                    };
            foreach (var col in columnsDictionary)
            {
                pDataTable.Columns.Add(col.Key, col.Value);
            }

            return pDataTable;
        }
        private DataTable SetPDataTable()
        {
            var importPDataTable = new DataTable();
            importPDataTable.Columns.Add("P10", typeof(int));
            importPDataTable.Columns.Add("P1", typeof(string));
            importPDataTable.Columns.Add("P2", typeof(string));
            importPDataTable.Columns.Add("P7", typeof(double));
            importPDataTable.Columns.Add("P8", typeof(double));
            importPDataTable.Columns.Add("P9", typeof(int));
            importPDataTable.Columns.Add("P3", typeof(double));
            importPDataTable.Columns.Add("P4", typeof(string));
            importPDataTable.Columns.Add("P5", typeof(string));
            importPDataTable.Columns.Add("P6", typeof(string));
            importPDataTable.Columns.Add("P11", typeof(string));
            importPDataTable.Columns.Add("P12", typeof(string));
            importPDataTable.Columns.Add("P13", typeof(string));
            importPDataTable.Columns.Add("PAY_BY_YOURSELF", typeof(string));
            return importPDataTable;
        }

        private DataTable SetImportPDataTable()
        {

            var importPDataTable = new DataTable();
            importPDataTable.Columns.Add("DecMasId", typeof(string));
            importPDataTable.Columns.Add("P10", typeof(int));
            importPDataTable.Columns.Add("P1", typeof(string));
            importPDataTable.Columns.Add("P2", typeof(string));
            importPDataTable.Columns.Add("P7", typeof(double));
            importPDataTable.Columns.Add("P8", typeof(double));
            importPDataTable.Columns.Add("P9", typeof(int));
            importPDataTable.Columns.Add("P3", typeof(double));
            importPDataTable.Columns.Add("P4", typeof(string));
            importPDataTable.Columns.Add("P5", typeof(string));
            importPDataTable.Columns.Add("P6", typeof(string));
            importPDataTable.Columns.Add("P11", typeof(string));
            importPDataTable.Columns.Add("P12", typeof(string));
            importPDataTable.Columns.Add("P13", typeof(string));
            importPDataTable.Columns.Add("PAY_BY_YOURSELF", typeof(string));
            return importPDataTable;
        }

        private void AddPData(DeclareData declareData, DataTable pDataTable)
        {
            for (var i = 0; i < declareData.DeclareDetails.Count; i++)
            {
                var row = pDataTable.NewRow();
                var detail = declareData.DeclareDetails[i];
                detail.Usage = declareData.Prescription.Medicines == null
                    ? detail.Usage
                    : declareData.Prescription.Medicines[i].UsageName;
                var paySelf = /*declareData.Prescription.Medicines == null ? "0" :*/
                    declareData.Prescription.Medicines[i].PaySelf ? "1" : "0";
                if (!String.IsNullOrEmpty(declareData.DecMasId))
                    row["DecMasId"] = declareData.DecMasId;
                

                Function function = new Function();
                row["P1"] = detail.MedicalOrder;
                row["P2"] = detail.MedicalId;
                row["P3"] = function.SetStrFormat(detail.Dosage, "{0:0000.00}");
                row["P4"] = detail.Usage;
                row["P5"] = detail.Position;
                row["P6"] = function.ToInvCulture(detail.Percent);
                row["P7"] = function.SetStrFormat(detail.Total, "{0:00000.0}");
                row["P8"] = function.SetStrFormat(detail.Price, "{0:0000000.00}");
                row["P9"] = function.SetStrFormatInt( Convert.ToInt32(Math.Truncate(Math.Round(detail.Point, 0, MidpointRounding.AwayFromZero))), "{0:D8}");
                row["P10"] = detail.Sequence.ToString();
                row["P11"] = detail.Days.ToString();
                row["PAY_BY_YOURSELF"] = paySelf;

                pDataTable.Rows.Add(row);
            }

            if (declareData.Prescription.Treatment.AdjustCase.Id == "3")
            {
                foreach (DataRow p in pDataTable.Rows)
                {
                    p["P8"] = function.SetStrFormat(0.0, "{0:0000000.00}");
                    p["P9"] = function.SetStrFormatInt(0, "{0:D8}");
                }
                AddDayPayCodePData(declareData, pDataTable);
            }
            AddMedicalServiceCostPData(declareData, pDataTable);
        }

        private void AddDayPayCodePData(DeclareData declareData, DataTable pDataTable)
        {
            var percent = CountAdditionPercent(declareData);
            var currentDate = DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            var detail = new DeclareDetail("1", declareData.DayPayCode, percent,
                declareData.MedicalServicePoint, declareData.DeclareDetails.Count + 1, currentDate,
                currentDate);
            var pData = pDataTable.NewRow();
            SetMedicalServiceCostDataRow(pData, declareData, detail);
            declareData.DeclareDetails.Add(detail);
            pDataTable.Rows.Add(pData);
        }

        private void AddImportPData(DeclareData declareData, DataTable pDataTable)
        {
            for (var i = 0; i < declareData.DeclareDetails.Count; i++)
            {
                var row = pDataTable.NewRow();
                var detail = declareData.DeclareDetails[i];
                detail.Usage = declareData.Prescription.Medicines == null
                    ? detail.Usage
                    : declareData.Prescription.Medicines[i].UsageName;
                var paySelf = declareData.Prescription.Medicines == null ? "0" :
                    declareData.Prescription.Medicines[i].PaySelf ? "1" : "0";

                row["DecMasId"] = declareData.DecMasId;
                row["P1"] = detail.MedicalOrder;
                row["P2"] = detail.MedicalId;
                row["P3"] = function.SetStrFormat(detail.Dosage, "{0:0000.00}");
                row["P4"] = detail.Usage;
                row["P5"] = detail.Position;
                row["P6"] = function.ToInvCulture(detail.Percent);
                row["P7"] = function.SetStrFormat(detail.Total, "{0:00000.0}");
                row["P8"] = function.SetStrFormat(detail.Price, "{0:0000000.00}");
                row["P9"] = function.SetStrFormatInt( Convert.ToInt32(Math.Truncate(Math.Round(detail.Point, 0, MidpointRounding.AwayFromZero))), "{0:D8}");
                row["P10"] = detail.Sequence.ToString();
                row["P11"] = detail.Days.ToString();
                row["PAY_BY_YOURSELF"] = paySelf;
                
                pDataTable.Rows.Add(row);
            }
        }
        /*
         * 加入藥事服務費之PData
         */

        private void AddMedicalServiceCostPData(DeclareData declareData, DataTable pDataTable)
        {
            double percent = 100;
            var currentDate = DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            var detail = new DeclareDetail("9", declareData.MedicalServiceCode, percent,
                declareData.MedicalServicePoint, declareData.DeclareDetails.Count + 1, currentDate,
                currentDate);
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
            double percent = 100;
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
            var declarecount = declareData.DeclareDetails.Count + 1; //藥事服務醫令序
            if (!string.IsNullOrEmpty(declareData.DecMasId)) pData["DecMasId"] = declareData.DecMasId;


                pData["P1"] = detail.MedicalOrder;
                pData["P2"] = detail.MedicalId;
                pData["P6"] = function.ToInvCulture(detail.Percent);
                pData["P7"] = function.SetStrFormat(detail.Total, "{0:00000.0}");
                pData["P8"] = function.SetStrFormat(detail.Price, "{0:0000000.00}");
                pData["P9"] = function.SetStrFormatInt(  Convert.ToInt32(Math.Truncate(Math.Round(detail.Point, 0,  MidpointRounding.AwayFromZero))), "{0:D8}");
                pData["P10"] = function.SetStrFormatInt(declarecount, "{0:D3}");
                pData["P12"] = detail.StartDate;
                pData["P13"] = detail.EndDate;
        }

        /*
         * 判斷慢箋調劑
         */

        private void CheckChronicAdjust(DeclareData declareData, ICollection<SqlParameter> parameters)
        {
            if (!declareData.Prescription.Treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId)
            ) //判斷是否為慢箋調劑
            {
                parameters.Add(new SqlParameter("D35", DBNull.Value));
                parameters.Add(new SqlParameter("D36", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("D35", declareData.Prescription.ChronicSequence));
                parameters.Add(new SqlParameter("D36", declareData.Prescription.ChronicTotal));
            }

            if (declareData.Prescription.Treatment.AdjustCase.Id.Equals(Resources.ChronicAdjustCaseId) &&
                Convert.ToInt32(declareData.Prescription.ChronicSequence) > 1)
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
                if (Convert.ToDecimal((string)declareData.Prescription.ChronicSequence) >= 2)
                    dData += function.XmlTagCreator("d43", (string)declareData.Prescription.OriginalMedicalNumber);
            }

            if (treatment.Copayment.Id == "903")
                dData += function.XmlTagCreator("d44",
                    CheckXmlDbNullValue(declareData.Prescription.Customer.IcCard.IcMarks.NewbornsData
                        .Birthday)); //新生兒註記就醫
            dData += "</dhead>";
            return dData;
        }

        /*
         * 設定DData dhead資料並以Dictionary結構回傳
         */

        private Dictionary<string, string> SetDheadDictionary(DeclareData declareData, Treatment treatment,
            MedicalInfo medicalInfo)
        {
            string d8 = string.Empty,
                d9 = string.Empty,
                d35 = declareData.Prescription.ChronicSequence,
                d36 = declareData.Prescription.ChronicTotal;
            string year = (int.Parse(declareData.Prescription.Customer.Birthday.Substring(0, 3)) + 1911)
                .ToString();
            string cusBirth = year + declareData.Prescription.Customer.Birthday.Substring(3, 6);
            if (medicalInfo.MainDiseaseCode != null)
            {
                d8 = medicalInfo.MainDiseaseCode.Id;
                if (medicalInfo.SecondDiseaseCode != null)
                    d9 = medicalInfo.SecondDiseaseCode.Id;
            }

            return new Dictionary<string, string>
                    {
                        {"d1", treatment.AdjustCase.Id},
                        {"d2", string.Empty},
                        {"d3", declareData.Prescription.Customer.IcNumber},
                        {"d4", CheckXmlDbNullValue(declareData.DeclareMakeUp)},
                        {"d5", CheckXmlDbNullValue(treatment.PaymentCategory.Id)},
                        {"d6", DateTimeExtensions.ToSimpleTaiwanDate(Convert.ToDateTime(cusBirth))},
                        {"d7", declareData.Prescription.Customer.IcCard.MedicalNumber},
                        {"d8", d8},
                        {"d9", d9},
                        {"d13", CheckXmlDbNullValue(medicalInfo.Hospital.Division.Id)},
                        {"d14", CheckXmlDbNullValue(DateTimeExtensions.ToSimpleTaiwanDate(treatment.TreatmentDate))},
                        {"d15", treatment.Copayment.Id},
                        {"d16", declareData.DeclarePoint.ToString()},
                        {"d17", treatment.Copayment.Point.ToString()},
                        {"d18", declareData.TotalPoint.ToString()},
                        {"d19", CheckXmlDbNullValue(declareData.AssistProjectCopaymentPoint.ToString())},
                        {"d20", declareData.Prescription.Customer.Name},
                        {"d21", medicalInfo.Hospital.Id},
                        {"d22", medicalInfo.TreatmentCase.Id},
                        {"d23", DateTimeExtensions.ToSimpleTaiwanDate(treatment.AdjustDate)},
                        {"d24", medicalInfo.Hospital.Doctor.Id},
                        {"d25", treatment.MedicalPersonId},
                        {"d26", CheckXmlDbNullValue(medicalInfo.SpecialCode.Id)},
                        {"d30", CheckXmlDbNullValue(treatment.MedicineDays)},
                        {"d31", CheckXmlDbNullValue(declareData.SpecailMaterialPoint.ToString())},
                        {"d32", CheckXmlDbNullValue(declareData.DiagnosisPoint.ToString())},
                        {"d33", CheckXmlDbNullValue(declareData.DrugsPoint.ToString())},
                        {"d35", d35},
                        {"d36", d36},
                        {"d37", declareData.MedicalServiceCode},
                        {"d38", declareData.MedicalServicePoint.ToString()},
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
                if (Convert.ToInt32((string)declareData.Prescription.Treatment.MedicineDays) < detail.Days)
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
                        {"p1", detail.MedicalOrder},
                        {"p2", detail.MedicalId},
                        {"p3", CheckXmlDbNullValue(function.ToInvCulture(detail.Dosage))},
                        {"p4", CheckXmlDbNullValue(detail.Usage)},
                        {"p5", CheckXmlDbNullValue(detail.Position)},
                        {"p6", CheckXmlDbNullValue(function.ToInvCulture(detail.Percent))},
                        {"p7", detail.Total.ToString()},
                        {"p8", detail.Price.ToString()},
                        {"p9", detail.Point.ToString()},
                        {"p10", detail.Sequence.ToString()},
                        {"p11", CheckXmlDbNullValue(detail.Days.ToString())}
                    };
        }

        public void ExportSortDeclareData(string sdate, string edate)
        {
            Function function = new Function();
            var conn = new DbConnection(Properties.Settings.Default.SQL_local);
            var param = new List<SqlParameter>();
            param.Add(new SqlParameter("SDATE", sdate));
            param.Add(new SqlParameter("EDATE", edate));
            var datetable = conn.ExecuteProc("[HIS_POS_DB].[GET].[UNEXPORTDECLAREDATE]", param); //取得未申報檔案
            param.Clear();
            param.Add(new SqlParameter("SDATE", sdate));
            param.Add(new SqlParameter("EDATE", edate));
            var medpertable = conn.ExecuteProc("[HIS_POS_DB].[GET].[MEDCALPERSON]", param); //取得現有藥師
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
                    param.Add(new SqlParameter("MEDPERSON",
                        medperrow["HISDECMAS_MEDICALPERSONNEL"].ToString()));
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
                            xml.SelectSingleNode("ddata/dhead/d18").InnerText =
                                (Convert.ToInt32(d18) - Convert.ToInt32(d38) + 18).ToString();
                            xml.SelectSingleNode("ddata/dhead/d16").InnerText =
                                (Convert.ToInt32(d16) - Convert.ToInt32(d38) + 18).ToString();
                            gcount++;
                            gpoint += Convert.ToInt32(d38) - 18;
                            xml.SelectSingleNode("ddata/dhead/d37").InnerText = "05234D";
                            xml.SelectSingleNode("ddata/dhead/d38").InnerText = "18";
                        }

                        if (totalcasid > 100)
                        {
                            xml.SelectSingleNode("ddata/dhead/d18").InnerText =
                                (Convert.ToInt32(d18) - Convert.ToInt32(d38)).ToString();
                            xml.SelectSingleNode("ddata/dhead/d16").InnerText =
                                (Convert.ToInt32(d16) - Convert.ToInt32(d38)).ToString();
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
                    } //foreach
                } //foreach medperrow
            } //foreach

            //取得當月排序DECLAREDAta 分配d2
            table = conn.ExecuteProc("[HIS_POS_DB].[GET].[PROCESSDECLAREDATA]");
            var catcount = 1;

            for (var i = 0; i < table.Rows.Count; i++)
            {
                xml.LoadXml(table.Rows[i]["HISDECMAS_DETXML"].ToString());
                if (i > 0 && table.Rows[i]["HISCASCAT_ID"].ToString() ==
                    table.Rows[i - 1]["HISCASCAT_ID"].ToString()) catcount++;
                if (i > 0 && table.Rows[i]["HISCASCAT_ID"].ToString() !=
                    table.Rows[i - 1]["HISCASCAT_ID"].ToString()) catcount = 1;
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
            xmlsum += "<t7>" + ncount + "</t7>"; //一般案件申請件數
            xmlsum += "<t8>" + npoint + "</t8>"; //一般案件申請點數
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
                parameters.Add(value.Equals(string.Empty) || value.Equals("0")
                    ? new SqlParameter(paraName, DBNull.Value)
                    : new SqlParameter(paraName, value));
            }
        }

        /*
         * 檢查XmlTag是否為空值
         */

        private string CheckXmlDbNullValue(string value)
        {
            if (value != string.Empty || value != "0" || value != null)
                return value;
            return string.Empty;
        }

        /*
         *檢查DataRow是否為空值
         */

        private void CheckEmptyDataRow(DataTable dataTable, string value, ref DataRow row, string rowName)
        {
            if (value != string.Empty )
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
        public void SetSameGroupChronic(string decMasId,string continueNum) { //同GROUP慢箋預約

            var parameters = new List<SqlParameter>();
            var conn = new DbConnection(Settings.Default.SQL_global);
                parameters.Add(new SqlParameter("DecMasId", decMasId));
                parameters.Add(new SqlParameter("COUTINUENUM", continueNum));
                conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[SetSameGroupChronic]", parameters);
        }
        public void SetNewGroupChronic(string decMasId)
        { //同GROUP慢箋預約

            var parameters = new List<SqlParameter>();
            var conn = new DbConnection(Settings.Default.SQL_global);
            parameters.Add(new SqlParameter("DecMasId", decMasId));
            conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[SetNewGroupChronic]", parameters);
        }
    }
}