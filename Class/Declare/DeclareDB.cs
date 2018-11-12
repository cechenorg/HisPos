using His_Pos.Class.Product;
using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.RDLC;
using His_Pos.H2_STOCK_MANAGE.ProductPurchase;
using His_Pos.Interface;

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
            parameters.Add(new SqlParameter("HISDECMAS_GETCARD", declareData.Prescription.IsGetIcCard));
            parameters.Add(new SqlParameter("HISDECMAS_DECLARE", declareData.Prescription.Declare));
            var conn = new DbConnection(Settings.Default.SQL_global);
            var table =  conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[InsertDeclareData]", parameters);
            return table.Rows[0][0].ToString();//回傳DesMasId
        }

        internal static ObservableCollection<DeclareDataDetailOverview.PurchaseDeclareDataOverview> GetPurchaseDeclareDataOverviews(string storeOrderId)
        {
            ObservableCollection<DeclareDataDetailOverview.PurchaseDeclareDataOverview> collection = new ObservableCollection<DeclareDataDetailOverview.PurchaseDeclareDataOverview>();

            var conn = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STOORDER_ID", storeOrderId));
            var table = conn.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetPurchaseDeclareDataOverview]", parameters);

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new DeclareDataDetailOverview.PurchaseDeclareDataOverview(row));
            }

            return collection;
        }

        public string InsertPrescribeData(DeclareData declareData)
        {
            var parameters = new List<SqlParameter>();
            var prescribeDataTable = SetPrescribeDataTable();
            AddPrescribeData(declareData,prescribeDataTable);
            
            parameters.Add(new SqlParameter("CusId", string.IsNullOrEmpty(declareData.Prescription.Customer.Id)? declareData.Prescription.Customer.Id:"0"));
            parameters.Add(new SqlParameter("TreatDate", declareData.Prescription.Treatment.AdjustDate));
            parameters.Add(new SqlParameter("EmpId", declareData.Prescription.Pharmacy.MedicalPersonnel.Id));
            parameters.Add(new SqlParameter("PrescribeDetail", prescribeDataTable));
            var conn = new DbConnection(Settings.Default.SQL_global);
            var table = conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[InsertPrescribeData]", parameters);
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
            DeclareFileDb.SetDeclareFileByPharmacyId(file, declareDate,declareData,DeclareFileType.LOG_IN);
        }
        public void UpdateDeclareData(DeclareData declareData)
        {
            var conn = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            AddParameterDData(parameters, declareData); //加入DData sqlparameters
            var pDataTable = SetUpdatePDataTable(); //設定PData datatable columns
            AddPData(declareData, pDataTable); //加入PData sqlparameters
            parameters.Add(new SqlParameter("ID", declareData.DecMasId));
            var xmlStr = declareData.SerializeObject<Ddata>();
            var errorStr = declareData.Prescription.EList.SerializeObject<ErrorList>();
            if (string.IsNullOrEmpty(errorStr))
                errorStr = "<ErrorPrescription></ErrorPrescription>";
            parameters.Add(new SqlParameter("EMP_ID", MainWindow.CurrentUser.Id));
            parameters.Add(new SqlParameter("DETAIL", pDataTable));
            parameters.Add(new SqlParameter("XML", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(xmlStr, XmlNodeType.Document, null))
            });

            parameters.Add(new SqlParameter("HISDECMAS_ERRORMSG", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader((string)errorStr, XmlNodeType.Document, null))
            });
            conn.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[UpdateDeclareData]", parameters);
        }

        public List<Ddata> SortDdataByCaseId(Pharmacy p)
        {
            var sorted = new List<Ddata>();
            var result = p.Ddata.GroupBy(d => d.Dhead.D1);
            foreach (var group in result)
            {
                sorted.AddRange(group);
            }
            return sorted;
        }

        private List<Ddata> GetDdataList(DateTime declareDate)
        {
            if (PrescriptionDB.GetPrescriptionXmlByDate(declareDate).Count == 0) return new List<Ddata>();
            //依調劑藥師分組
            var ddatas = PrescriptionDB.GetPrescriptionXmlByDate(declareDate).GroupBy(d => d.Dbody.D25)
                .ToList();
            var result = new List<Ddata>();
            //每位調劑藥師處方排序
            foreach (var group in ddatas)
            {
                var tmpGroup = group.OrderBy(d => d.Dbody.D38);//依藥事服務費點數排序
                var count = 1;
                foreach (var tmpG in tmpGroup)
                {
                    //每人每日1 - 80件內 => 診療項目代碼: 05202B . 支付點數 : 48
                    if (count < 80)
                    {
                        if (!string.IsNullOrEmpty(tmpG.Dbody.D36) && int.Parse(tmpG.Dbody.D30)>=14 && int.Parse(tmpG.Dbody.D30) <= 27)
                        {
                            tmpG.Dbody.D37 = "05206B";
                            tmpG.Dbody.D38 = "59";
                        }
                        else if (!string.IsNullOrEmpty(tmpG.Dbody.D36) && int.Parse(tmpG.Dbody.D30) >= 28)
                        {
                            tmpG.Dbody.D37 = "05210B";
                            tmpG.Dbody.D38 = "69";
                        }
                        else if (!string.IsNullOrEmpty(tmpG.Dbody.D36) && int.Parse(tmpG.Dbody.D30) <= 13)
                        {
                            tmpG.Dbody.D37 = "05223B";
                            tmpG.Dbody.D38 = "48";
                        }
                        else
                        {
                            tmpG.Dbody.D37 = "0502B";
                            tmpG.Dbody.D38 = "48";
                        }
                    }
                    //每人每日81 - 100件內 => 診療項目代碼: 05234D . 支付點數 : 15
                    else if (count < 100 && count >= 80)
                    {
                            tmpG.Dbody.D37 = "05234D";
                            tmpG.Dbody.D38 = "15";
                    }
                    //每人每日100件以上 => 診療項目代碼: 0502B . 支付點數 : 0
                    else
                    { 
                        tmpG.Dbody.D37 = "0502B";
                        tmpG.Dbody.D38 = "0";
                    }
                    count++;
                }
                result.AddRange(tmpGroup);
            }
            return result;
        }

        private string GetDateStr(DateTime d, bool now)
        {
            if (now)
                return d.Year - 1911 + d.Month.ToString().PadLeft(2, '0') + "01";
            return d.Year - 1911 + d.Month.ToString().PadLeft(2, '0') + d.Day.ToString().PadLeft(2, '0');
        }

        public int CountPrescriptionByCase(List<Ddata> listDdata, int caseType)
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
            foreach (var declareDetail in declareData.Prescription.Medicines)
            {
                if (declareDetail is DeclareMedicine med)
                {
                    if (!med.IsBuckle) continue;
                    parameters.Add(new SqlParameter("BUCKLE_VALUE", ((IProductDeclare)(DeclareMedicine)declareDetail).Amount));
                }
                else
                {
                    parameters.Add(new SqlParameter("BUCKLE_VALUE", ((IProductDeclare)(PrescriptionOTC)declareDetail).Amount));
                }
                parameters.Clear();
                parameters.Add(new SqlParameter("MAS_ID", decMasId));
                parameters.Add(new SqlParameter("PRO_ID", declareDetail.Id));
                
                parameters.Add(new SqlParameter("BUCKLE_STATUS", "1"));
                parameters.Add(new SqlParameter("WAY", way));
                parameters.Add(new SqlParameter("PROWAR_ID", "1"));
                
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
                    tag.Key == "D33" || tag.Key == "D37")
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
                        {"D14", declareData.Prescription.Treatment.TreatmentDate.ToString("yyyy-MM-dd") },
                        {"D15", declareData.Prescription.Treatment.Copayment.Id},
                        {"D23", declareData.Prescription.Treatment.AdjustDate.ToString("yyyy-MM-dd") },
                        {"D25", declareData.Prescription.Pharmacy.MedicalPersonnel.IcNumber},
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
                        {"D24", med.Hospital.Doctor.IcNumber},
                        {"D26", string.IsNullOrEmpty(med.SpecialCode.Id)?string.Empty:med.SpecialCode.Id},
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

            if (string.IsNullOrEmpty(declareTrade.EmpId))
                row["EMP_ID"] = DBNull.Value;
            else
                row["EMP_ID"] = declareTrade.EmpId;

            if (string.IsNullOrEmpty(declareTrade.PaySelf))
                row["PAYSELF"] = DBNull.Value;
            else
                row["PAYSELF"] = Convert.ToInt32(declareTrade.PaySelf);

            if (string.IsNullOrEmpty(declareTrade.Deposit))
                row["DEPOSIT"] = DBNull.Value;
            else
                row["DEPOSIT"] = Convert.ToInt32(declareTrade.Deposit);

            if (string.IsNullOrEmpty(declareTrade.ReceiveMoney))
                row["RECEIVE_MONEY"] = DBNull.Value;
            else
                row["RECEIVE_MONEY"] = Convert.ToInt32(declareTrade.ReceiveMoney);

            if (string.IsNullOrEmpty(declareTrade.CopayMent))
                row["COPAYMENT"] = DBNull.Value;
            else
                row["COPAYMENT"] = Convert.ToInt32(declareTrade.CopayMent);

            if (string.IsNullOrEmpty(declareTrade.PayMoney))
                row["PAYMONEY"] = DBNull.Value;
            else
                row["PAYMONEY"] = Convert.ToInt32(declareTrade.PayMoney);

            if (string.IsNullOrEmpty(declareTrade.Change))
                row["CHANGE"] = DBNull.Value;
            else
                row["CHANGE"] = Convert.ToInt32(declareTrade.Change);

            if (string.IsNullOrEmpty(declareTrade.PayWay))
                row["PAYWAY"] = DBNull.Value;
            else
                row["PAYWAY"] = declareTrade.PayWay;
            tradeTable.Rows.Add(row);
        }

        private DataTable SetCustomerTable()
        {
            var CustomerTable = new DataTable();
            CustomerTable.Columns.Add("CUS_NAME", typeof(string));
            CustomerTable.Columns.Add("CUS_BIRTH", typeof(DateTime));
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
            declareMasterTable.Columns.Add("D14", typeof(DateTime));
            declareMasterTable.Columns.Add("D15", typeof(string));
            declareMasterTable.Columns.Add("D16", typeof(int));
            declareMasterTable.Columns.Add("D17", typeof(int));
            declareMasterTable.Columns.Add("D18", typeof(int));
            declareMasterTable.Columns.Add("D19", typeof(int));
            declareMasterTable.Columns.Add("D21", typeof(string));
            declareMasterTable.Columns.Add("D22", typeof(string));
            declareMasterTable.Columns.Add("D23", typeof(DateTime));
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
            declareMasterTable.Columns.Add("XML", typeof(SqlXml));


            return declareMasterTable;
        }

        private void AddDeclareMaster(DeclareData declareData, DataTable declareMaster)
        {
            string d8 = string.Empty,
                d9 = string.Empty,
                d35 = declareData.Prescription.ChronicSequence,
                d36 = declareData.Prescription.ChronicTotal;
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
            row["D6"] = DateTimeExtensions.ConvertToTaiwanCalender(declareData.Prescription.Customer.Birthday, false);
            row["D7"] = declareData.Prescription.Customer.IcCard.MedicalNumber;
            row["D8"] = d8;
            row["D9"] = d9;
            row["CUS_ID"] = declareData.Prescription.Customer.IcCard.IcNumber; //身分證
            row["D11"] = string.Empty;
            row["D12"] = string.Empty;
            row["D13"] =
                CheckXmlDbNullValue(declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.Id);
            row["D14"] = declareData.Prescription.Treatment.TreatmentDate;
            row["D15"] = declareData.Prescription.Treatment.Copayment.Id;
            row["D16"] = declareData.DeclarePoint.ToString();
            row["D17"] = declareData.Prescription.Treatment.Copayment.Point.ToString();
            row["D18"] = declareData.TotalPoint.ToString();
            row["D19"] = CheckXmlDbNullValue(declareData.AssistProjectCopaymentPoint.ToString());
            row["D21"] = declareData.Prescription.Treatment.MedicalInfo.Hospital.Id;
            row["D22"] = declareData.Prescription.Treatment.MedicalInfo.TreatmentCase.Id;
            row["D23"] = declareData.Prescription.Treatment.AdjustDate;  
            row["D24"] = declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.IcNumber;
            row["D25"] = declareData.Prescription.Pharmacy.MedicalPersonnel.IcNumber;
            row["D26"] = string.IsNullOrEmpty(declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id)?string.Empty: declareData.Prescription.Treatment.MedicalInfo.SpecialCode.Id;
            row["D27"] = string.Empty;
            row["D28"] = string.Empty;
            row["D29"] = string.Empty;
            row["D30"] = CheckXmlDbNullValue(declareData.Prescription.Treatment.MedicineDays);
            row["D31"] = CheckXmlDbNullValue(declareData.SpecailMaterialPoint.ToString());
            row["D32"] = CheckXmlDbNullValue(declareData.DiagnosisPoint.ToString());
            row["D33"] = CheckXmlDbNullValue(declareData.DrugsPoint.ToString());
            if (string.IsNullOrEmpty(d35))
                row["D35"] = DBNull.Value;
            else
                row["D35"] = d35;
            if (string.IsNullOrEmpty(d36))
                row["D36"] = DBNull.Value;
            else
                row["D36"] = d36;
            row["D37"] = declareData.MedicalServiceCode;
            row["D38"] = declareData.MedicalServicePoint.ToString();
            row["D43"] = string.IsNullOrEmpty(declareData.Prescription.OriginalMedicalNumber)? "" : declareData.Prescription.OriginalMedicalNumber;

            var xmlStr = declareData.SerializeObject<Ddata>(); 
            row["XML"] = new SqlXml(new XmlTextReader(xmlStr, XmlNodeType.Document, null));
            declareMaster.Rows.Add(row);
        }
        private DataTable SetUpdatePDataTable()
        {
            var pDataTable = new DataTable();
            var columnsDictionary = new Dictionary<string, Type>
                    {
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
                if (declareData.Prescription.Medicines[i] is DeclareMedicine med)
                {
                    var row = pDataTable.NewRow();
                    var detail = declareData.DeclareDetails[i];
                    if (detail.PaySelf) continue;
                    detail.Usage = declareData.Prescription.Medicines == null
                        ? detail.Usage
                        : med.UsageName;
                    var paySelf = /*declareData.Prescription.Medicines == null ? "0" :*/
                        med.PaySelf ? "1" : "0";
                    //if (!String.IsNullOrEmpty(declareData.DecMasId))
                    //    row["DecMasId"] = declareData.DecMasId;


                    var function = new Function();
                    row["P1"] = detail.MedicalOrder;
                    row["P2"] = detail.MedicalId;
                    row["P3"] = function.SetStrFormat(detail.Dosage, "{0:0000.00}");
                    row["P4"] = detail.Usage;
                    row["P5"] = detail.Position;
                    row["P6"] = function.ToInvCulture(detail.Percent);
                    row["P7"] = function.SetStrFormat(detail.Total, "{0:00000.0}");
                    row["P8"] = function.SetStrFormat(detail.Price, "{0:0000000.00}");
                    row["P9"] = function.SetStrFormatInt(Convert.ToInt32(Math.Truncate(Math.Round(detail.Point, 0, MidpointRounding.AwayFromZero))), "{0:D8}");
                    row["P10"] = detail.Sequence.ToString();
                    row["P11"] = detail.Days.ToString();
                    row["PAY_BY_YOURSELF"] = paySelf;

                    pDataTable.Rows.Add(row);
                }
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
            var count = 0;
            foreach (var d in declareData.DeclareDetails)
            {
                if(d.PaySelf) continue;
                count++;
            }
            var percent = CountAdditionPercent(declareData);
            var currentDate = DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            var detail = new DeclareDetail("1", declareData.DayPayCode, percent,
                declareData.MedicalServicePoint, count + 1, currentDate,
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
                if (declareData.Prescription.Medicines[i] is DeclareMedicine med)
                {
                    var row = pDataTable.NewRow();
                    var detail = declareData.DeclareDetails[i];
                    detail.Usage = declareData.Prescription.Medicines == null
                        ? detail.Usage
                        : med.UsageName;
                    var paySelf = declareData.Prescription.Medicines == null ? "0" :
                        med.PaySelf ? "1" : "0";

                    row["DecMasId"] = declareData.DecMasId;
                    row["P1"] = detail.MedicalOrder;
                    row["P2"] = detail.MedicalId;
                    row["P3"] = function.SetStrFormat(detail.Dosage, "{0:0000.00}");
                    row["P4"] = detail.Usage;
                    row["P5"] = detail.Position;
                    row["P6"] = function.ToInvCulture(detail.Percent);
                    row["P7"] = function.SetStrFormat(detail.Total, "{0:00000.0}");
                    row["P8"] = function.SetStrFormat(detail.Price, "{0:0000000.00}");
                    row["P9"] = function.SetStrFormatInt(Convert.ToInt32(Math.Truncate(Math.Round(detail.Point, 0, MidpointRounding.AwayFromZero))), "{0:D8}");
                    row["P10"] = detail.Sequence.ToString();
                    row["P11"] = detail.Days.ToString();
                    row["PAY_BY_YOURSELF"] = paySelf;
                    pDataTable.Rows.Add(row);
                }
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
            var month = DateTimeExtensions.CalculateAge(declareData.Prescription.Customer.Birthday);
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
                parameters.Add(new SqlParameter("D43", declareData.Prescription.OriginalMedicalNumber == null ? "" : declareData.Prescription.OriginalMedicalNumber));
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
            var tagList = new List<string> { "D10", "D11", "D12", "D27", "D28", "D29" };
            foreach (var tag in tagList)
            {
                parameters.Add(new SqlParameter(tag, DBNull.Value));
            }
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

        public void InsertDailyUpload(string dataXml)
        {
            var parameters = new List<SqlParameter>();
            var conn = new DbConnection(Settings.Default.SQL_global);
            parameters.Add(new SqlParameter("ID", DBNull.Value));
            parameters.Add(new SqlParameter("UPLOAD_DATA", dataXml));
            parameters.Add(new SqlParameter("UPLOAD_STATUS", 0));
            parameters.Add(new SqlParameter("CREATE_DATE", DateTime.Now.ToShortDateString()));
            conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[InsertDailyUploadData]", parameters);
        }

        public void StartDailyUpload()
        {
            var parameters = new List<SqlParameter>();
            var conn = new DbConnection(Settings.Default.SQL_global);
            parameters.Add(new SqlParameter("CREATE_DATE", DateTime.Now.ToShortDateString()));
            var dailyUploadTable = conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetDailyUploadDataByDate]", parameters);
            var icDataUpload = new IcRecordList {RecordList = new List<IcRecord>()};

            foreach (DataRow row in dailyUploadTable.Rows)
            {
                icDataUpload.RecordList.Add(XmlService.Deserialize<IcRecord>(row["UPLOADDATA_CONTENT"].ToString()));
            }
            var f = new Function();
            XDocument result;
            var xmlSerializer = new XmlSerializer(icDataUpload.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, icDataUpload);
                var document = XDocument.Parse(ReportService.PrettyXml(textWriter));
                var root = XElement.Parse(document.ToString());
                document = XDocument.Load(root.CreateReader());
                document.Root?.RemoveAttributes();
                result = document;
            }
            //匯出xml檔案
            f.DailyUpload(result);
            InsertDailyUploadFile(result);
        }

        private void InsertDailyUploadFile(XDocument fileContent)
        {
            var parameters = new List<SqlParameter>();
            var conn = new DbConnection(Settings.Default.SQL_global);
            parameters.Add(new SqlParameter("ID", DBNull.Value));
            parameters.Add(new SqlParameter("UPLOAD_DATE", DateTime.Now.ToShortDateString()));
            parameters.Add(new SqlParameter("FILE_CONTENT", fileContent.ToString()));
            conn.ExecuteProc("[HIS_POS_DB].[MainWindowView].[InsertDailyUploadFileData]", parameters);
        }
        public void CheckPredictChronicExist(string decMasId) { //判斷是否有重複預約慢箋 
            var parameters = new List<SqlParameter>();
            var conn = new DbConnection(Settings.Default.SQL_global);
            parameters.Add(new SqlParameter("DecMasId", decMasId));
            conn.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[CheckPredictChronicExist]", parameters);
        }

        private DataTable SetPrescribeDataTable()
        {
            var importPDataTable = new DataTable();
            importPDataTable.Columns.Add("PRO_ID", typeof(string));
            importPDataTable.Columns.Add("CUSPRESCRIBE_QTY", typeof(string));
            importPDataTable.Columns.Add("CUSPRESCRIBE_PRICE", typeof(string));
            return importPDataTable;
        }

        private void AddPrescribeData(DeclareData declareData, DataTable pDataTable)
        {
            foreach (var detail in declareData.DeclareDetails)
            {
                var row = pDataTable.NewRow();
                row["PRO_ID"] = detail.MedicalId;
                row["CUSPRESCRIBE_QTY"] = detail.Total;
                row["CUSPRESCRIBE_PRICE"] = short.Parse(Math.Ceiling(detail.Total*detail.Price).ToString());
                pDataTable.Rows.Add(row);
            }
        }
    }
}