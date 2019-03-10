using His_Pos.Database;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Product;
using System.Linq;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;

namespace His_Pos.NewClass.Prescription
{
    public static class PrescriptionDb
    { 
        public static int InsertPrescription(Prescription prescription,List<Pdata> prescriptionDetails) { 
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionMaster", SetPrescriptionMaster(prescription));
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionDetail", SetPrescriptionDetail(prescription, prescriptionDetails)); 
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescription]", parameterList);
            return Convert.ToInt32(table.Rows[0]["DecMasId"].ToString()); 
        }
        
        
        public static void DeleteReserve(string recMasId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "RecMas_Id", recMasId); 
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteReserve]", parameterList);  
        }
        public static void PredictResere(int preMasId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreId", preMasId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[PredictResere]", parameterList);
        }
        public static void AdjustPredictResere(string preMasId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreMasId", preMasId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[AdjustPredictResere]", parameterList);
        } 
        public static void UpdateReserve(Prescription prescription, List<Pdata> prescriptionDetails) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataTable resMaster = SetReserveMaster(prescription);
            resMaster.Rows[0]["ResMas_ID"] = prescription.SourceId;
            DataBaseFunction.AddSqlParameter(parameterList, "ResMaster", resMaster);
            DataBaseFunction.AddSqlParameter(parameterList, "ResDetail", SetReserveionDetail(prescription, prescriptionDetails));
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReserve]", parameterList); 
        }
         
        public static DataTable ProcessInventory(string productID,double amount,string type,string source,string sourcdId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ProId", productID);
            DataBaseFunction.AddSqlParameter(parameterList, "BuckleValue", amount);
            DataBaseFunction.AddSqlParameter(parameterList, "Type", type);
            DataBaseFunction.AddSqlParameter(parameterList, "Source", source);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceID", sourcdId);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductBuckle]", parameterList); 
        }

        public static DataTable GetDeposit(int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "preId", id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[DepositByPreId]", parameterList);
        }
        public static DataTable GetAmountPaySelf(int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "preId", id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[AmountPaySelfByPreId]", parameterList);
        }
        
        public static DataTable ReturnInventory(string productID, double amount, string type, string source, string sourcdId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ProId", productID);
            DataBaseFunction.AddSqlParameter(parameterList, "ReturnValue", amount);
            DataBaseFunction.AddSqlParameter(parameterList, "typename", type);
            DataBaseFunction.AddSqlParameter(parameterList, "sourcename", source);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceID", sourcdId);
           return MainWindow.ServerConnection.ExecuteProc("[Set].[RevertStockValue]", parameterList);
        }
        public static void ProcessCashFlow(string cashFlowName, string source, int sourceId, double total)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Name", cashFlowName);
            DataBaseFunction.AddSqlParameter(parameterList, "Value", total);
            DataBaseFunction.AddSqlParameter(parameterList, "Source", source);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceId", sourceId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCashFlow]", parameterList); 
        }
        public static void ProcessEntry(string entryName, string source,int sourceId,double total)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Name", entryName);
            DataBaseFunction.AddSqlParameter(parameterList, "Value", total);
            DataBaseFunction.AddSqlParameter(parameterList, "Source", source);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceId", sourceId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStockEntry]", parameterList);
        }
       
        public static DataTable GetPrescriptionCountByID(string pharmacistIdnum)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "EmpIdNum", pharmacistIdnum);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCount]", parameterList); 
        }
        public static DataTable GetPaySelfByID(int preId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "preId", preId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[GetPaySelfByID]", parameterList);
        }
        public static DataTable GetPrescriptionId( ) { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionId]");
        }
        
        
        public static DataTable GetSearchPrescriptionsData(DateTime? sDate, DateTime? eDate, AdjustCase adj, Institution ins, MedicalPersonnel pharmacist)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustId", adj is null ? null : adj.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "InstitutionId", ins is null ? null : ins.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "EmpIdNum", pharmacist is null ? null : pharmacist.IdNumber);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionBySearchCondition]", parameterList);
        } 
        public static DataTable GetReservePrescriptionsData(DateTime? sDate, DateTime? eDate, AdjustCase adj, Institution ins, MedicalPersonnel pharmacist)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustId", adj is null ? null : adj.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "InstitutionId", ins is null ? null : ins.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "EmpIdNum", pharmacist is null ? null : pharmacist.IdNumber);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReserveBySearchCondition]", parameterList);
        }
      
        public static DataTable GetPrescriptionsNoGetCardByCusId(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusId", cusId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionsNoGetCardByCusId]", parameterList);
        }

        public static DataTable GetPrescriptionsByCusIdNumber(string cusIdnumber)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusIdNumber", cusIdnumber); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionsByCusIdNumber]", parameterList);
        }
        public static DataTable GetReservePrescriptionByCusId(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusId", cusId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReservePrescriptionByCusId]", parameterList);
        }
        public static DataTable GetRegisterPrescriptionByCusId(int cusId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusId", cusId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[RegisterPrescriptionByCusId]", parameterList);
        } 
        public static void InsertCooperAdjust(Prescription prescription, List<Pdata> prescriptionDetails, string remark) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionId", prescription.Id);
            DataBaseFunction.AddSqlParameter(parameterList, "Meds", SetPrescriptionDetail(prescription, prescriptionDetails));
            DataBaseFunction.AddSqlParameter(parameterList, "Remark", string.IsNullOrEmpty(remark) ? null : remark);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCooperAdjust]", parameterList);
        }
        public static void ImportDeclareXml(List<ImportDeclareXml.ImportDeclareXml.Ddata> ddatas, List<string> declareFiles,string fileId) {
            Customers cs = new Customers();
            cs = cs.SetCustomersByPrescriptions(ddatas);
            int preId = PrescriptionDb.GetPrescriptionId().Rows[0].Field<int>("MaxPreId");
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionMaster", SetImportDeclareXmlMaster(ddatas, declareFiles,preId, cs, fileId));
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionDetail", SetImportDeclareXmlDetail(ddatas, preId));
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[ImportDeclareXml]", parameterList);
        }
        public static void UpdatePrescription(Prescription prescription, List<Pdata> prescriptionDetails)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataTable prescriptionMater = SetPrescriptionMaster(prescription);
            prescriptionMater.Rows[0]["PreMas_ID"] = prescription.Id;
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionMaster", prescriptionMater);
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionDetail", SetPrescriptionDetail(prescription, prescriptionDetails));
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePrescription]", parameterList);
        }
        public static DataTable CheckImportDeclareFileExist(string head) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Head", head);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[CheckImportDeclareFileExist]", parameterList);
        }
        public static void PredictThreeMonthPrescription() {
            MainWindow.ServerConnection.ExecuteProc("[Set].[PredictThreeMonthPrescription]");
        }
        public static DataTable DeletePrescription(int preId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreId", preId);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[DeletePrescription]", parameterList);  
        }
        public static DataTable GetStoreOrderIDByPrescriptionID(int preId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PREMAS_ID", preId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderIDByPrescriptionID]", parameterList);
        } 
        public static bool SendDeclareOrderToSingde(string storId, Prescription p, PrescriptionSendDatas PrescriptionSendData)
        {
            return sendOrderAction(storId, p, PrescriptionSendData, "AddDeclareOrderToPreDrug") == "SUCCESS" ? true : false;
        }
        public static bool UpdateDeclareOrderToSingde(string storId, Prescription p, PrescriptionSendDatas PrescriptionSendData) {
            return sendOrderAction(storId,p, PrescriptionSendData, "UpdateDeclareOrder") == "SUCCESS" ? true : false;
        }
        public static string sendOrderAction(string storId, Prescription p, PrescriptionSendDatas PrescriptionSendData,string sql) {
            string Rx_id = ViewModelMainWindow.CurrentPharmacy.ID; //藥局機構代號 傳輸主KEY
            string Rx_order = Convert.ToDateTime(p.Treatment.AdjustDate).AddYears(-1911).ToString("yyyMMdd"); // 調劑日期(7)病歷號(9)
            string Pt_name = p.Patient.Name; // 藥袋名稱(病患姓名)
            string Upload_data = DateTime.Now.ToString(" yyyy - MM - dd hh:mm:ss "); //更新時間( 2014 - 01 - 24 21:13:03 )
            string Upload_status = string.Empty; //	列印判斷
            string Prt_date = string.Empty; //列印日期
            string Inv_flag = "0"; //轉單處理確認0未處理 1已處理 2不處理
            string Batch_sht = storId; //出貨單號
            string Inv_chk = "0"; //  庫存確認 是1 否0
            string Inv_msg = ""; //庫存確認

            string empty = string.Empty;
            StringBuilder Dtl_data = new StringBuilder(); //  備註text  處方資訊
            //第一行
            Dtl_data.Append(p.Id.ToString().PadLeft(8, '0')); //藥局病歷號
            Dtl_data.Append(p.Patient.Name.PadRight(20 - NewFunction.HowManyChinese(p.Patient.Name), ' ')); //病患姓名 
            Dtl_data.Append(p.Patient.IDNumber.PadRight(10, ' ')); //身分證字號
            Dtl_data.Append(DateTimeExtensions.ConvertToTaiwanCalender((DateTime)p.Patient.Birthday, false)); //出生年月日
            string gender = p.Patient.Gender.Substring(0, 1) == "男" ? "1" : "2";
            Dtl_data.Append(gender.PadRight(1, ' ')); //性別判斷 1男 2女
            Dtl_data.Append(p.Patient.Tel == null ? empty.PadRight(20, ' ') : p.Patient.Tel.PadRight(20, ' ')); //電話
            Dtl_data.AppendLine();
            //第二行   
            Dtl_data.Append(p.Treatment.Institution.ID.PadRight(10, ' ')); //院所代號
            Dtl_data.Append(p.Treatment.Institution.ID.PadRight(10, ' ')); //診治醫師代號 (同院所代號)
            Dtl_data.Append(empty.PadRight(20, ' ')); //空
            Dtl_data.Append(ViewModelMainWindow.CurrentUser.ID.ToString().PadRight(10, ' ')); //藥師代號
            Dtl_data.Append(ViewModelMainWindow.CurrentUser.Name.PadRight(20 - NewFunction.HowManyChinese(ViewModelMainWindow.CurrentUser.Name), ' ')); //藥師姓名 
            Dtl_data.AppendLine();
            //第三行
            Dtl_data.Append(((DateTime)p.Treatment.TreatDate).AddYears(-1911).ToString("yyyMMdd")); //處方日(就診日期)
            Dtl_data.Append(((DateTime)p.Treatment.AdjustDate).AddYears(-1911).ToString("yyyMMdd")); //調劑日期
            Dtl_data.Append(p.Treatment.PrescriptionCase.ID.PadRight(2, ' ')); //案件
            Dtl_data.Append(p.Treatment.Division.ID.PadRight(2, ' ')); //科別
            Dtl_data.Append(p.Treatment.MainDisease.ID.PadRight(10, ' ')); //主診斷
            Dtl_data.Append(string.IsNullOrEmpty(p.Treatment.SubDisease.ID) ? empty.PadRight(10, ' ') : p.Treatment.SubDisease.ID.PadRight(10, ' ')); //次診斷
            Dtl_data.Append(p.Treatment.TempMedicalNumber == null ? empty.PadRight(4, ' ') : p.Treatment.TempMedicalNumber.PadRight(4, ' ')); //卡序 (0001、欠卡、自費)
            Dtl_data.Append("2".PadRight(1, ' ')); //1一般箋 2慢箋
            Dtl_data.Append(p.Treatment.ChronicTotal.ToString().PadRight(1, ' ')); //可調劑次數
            Dtl_data.Append(p.Treatment.ChronicSeq.ToString().PadRight(1, ' ')); //本次調劑次數
            Dtl_data.Append(p.PrescriptionPoint.AmountSelfPay.ToString().PadRight(8, ' ')); //自費金額

            double medCost = 0;
            foreach (Medicine declareMedicine in p.Medicines)
            {
                if (declareMedicine is MedicineNHI || declareMedicine is MedicineSpecialMaterial)
                    medCost += declareMedicine.TotalPrice;
            }
            Dtl_data.Append(medCost.ToString().PadRight(8, ' ')); //藥品費
            string medicalPay = "0";
            if (Convert.ToInt32(p.MedicineDays) <= 13)
                medicalPay = "48";
            if (Convert.ToInt32(p.MedicineDays) >= 14 && Convert.ToInt32(p.MedicineDays) < 28)
                medicalPay = "59";
            if (Convert.ToInt32(p.MedicineDays) >= 28)
                medicalPay = "69";

            Dtl_data.Append(medicalPay.PadRight(4, ' ')); //藥事費
            Dtl_data.Append(p.PrescriptionPoint.CopaymentPoint.ToString().PadRight(4, ' ')); //部分負擔
            Dtl_data.AppendLine();
            //第四行
            int i = 1;
            foreach (Medicine declareMedicine in p.Medicines)
            {
                if (declareMedicine is MedicineNHI || declareMedicine is MedicineSpecialMaterial)
                {
                    if (declareMedicine.ID.Length > 12)
                        Dtl_data.Append(declareMedicine.ID.Substring(0, 12).PadRight(12, ' ')); //健保碼
                    else
                        Dtl_data.Append(declareMedicine.ID.PadRight(12, ' ')); //健保碼

                    Dtl_data.Append(declareMedicine.Dosage == null ? empty.PadRight(8, ' ') : declareMedicine.Dosage.ToString().PadLeft(8, ' ')); //每次使用數量
                    Dtl_data.Append(declareMedicine.Usage.Name == null ? empty.PadRight(16, ' ') : declareMedicine.Usage.Name.PadRight(16, ' ')); //使用頻率
                    Dtl_data.Append(declareMedicine.Days == null ? empty.PadRight(3, ' ') : declareMedicine.Days.ToString().PadRight(3, ' ')); //使用天數
                    Dtl_data.Append(declareMedicine.Amount.ToString().PadRight(8, ' ')); //使用總量
                    if (declareMedicine.ID.Length > 12)
                        Dtl_data.Append(declareMedicine.ID.Split('-')[1].PadRight(6, ' ')); //途徑 (詳見:途徑欄位說明)
                    else
                        Dtl_data.Append(declareMedicine.Position.ID.PadRight(6, ' ')); //途徑 (詳見:途徑欄位說明)

                    if (!declareMedicine.PaySelf)
                        Dtl_data.Append(" ");
                    else
                        Dtl_data.Append(declareMedicine.TotalPrice > 0 ? "Y" : "N".PadRight(1, ' ')); //自費判斷 Y自費收費 N自費不收費

                    Dtl_data.Append(empty.PadRight(1, ' ')); //管藥判斷庫存是否充足 Y是 N 否
                    string amount = string.Empty;
                    foreach (PrescriptionSendData row in PrescriptionSendData)
                    {
                        if (row.MedId == declareMedicine.ID)
                        {
                            amount = row.SendAmount.ToString();
                            break;
                        }
                    }
                    Dtl_data.Append(amount.PadRight(10, ' ')); //訂購量

                }
                if (i < p.Medicines.Count(med => med is MedicineNHI || med is MedicineSpecialMaterial))
                    Dtl_data.AppendLine();
                i++;
            }
            string result = "FAIL";
            DataTable table;
            switch (sql) {
                case "AddDeclareOrderToPreDrug":
                    MainWindow.SingdeConnection.OpenConnection();
                    table = MainWindow.SingdeConnection.ExecuteProc($"call AddDeclareOrderToPreDrug('{Rx_id}', '{storId}', '{p.Patient.Name}','{Dtl_data}','{((DateTime)p.Treatment.AdjustDate).AddYears(-1911).ToString("yyyMMdd")}')");
                    if (table.Rows.Count > 0 && table.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                        result = "SUCCESS";
                    MainWindow.SingdeConnection.CloseConnection();
                    break;
                case "UpdateDeclareOrder":
                    MainWindow.SingdeConnection.OpenConnection();
                    table =  MainWindow.SingdeConnection.ExecuteProc($"call UpdateDeclareOrder('{Rx_id}', '{storId}','{Dtl_data}')");
                    if (table.Rows.Count > 0 && table.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                        result = "SUCCESS";
                    MainWindow.SingdeConnection.CloseConnection();
                    break;
            }
            return result;
        }

        public static void UpdatePrescriptionStatus(PrescriptionStatus prescriptionStatus,int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreId", id);
            DataBaseFunction.AddSqlParameter(parameterList, "IsSendToServer", prescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddSqlParameter(parameterList, "IsGetCard", prescriptionStatus.IsGetCard);
            DataBaseFunction.AddSqlParameter(parameterList, "IsDeclare", prescriptionStatus.IsDeclare);
            DataBaseFunction.AddSqlParameter(parameterList, "IsDeposit", prescriptionStatus.IsDeposit);
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePrescriptionStatus]", parameterList);
        }
        public static DataTable GetPrescriptionByID(int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreId", id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionByPreId]", parameterList);
        }
        public static DataTable GetReservePrescriptionByID(int id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ResId", id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReservePrescriptionByResId]", parameterList);
        }
        public static DataTable GetSearchPrescriptionsSummary(List<int> presId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetIDTable(presId)); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[SearchPrescriptionsSummary]", parameterList);
        }  
        public static DataTable GetSearchReservesSummary(List<int> presId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetIDTable(presId));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[SearchReservesSummary]", parameterList);
        }
        #region WepApi
        internal static void UpdateCooperativePrescriptionIsRead(string DeclareId) {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty },
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateIsReadByDeclareId", keyValues);
        }
        internal static void UpdateCooperativePrescriptionStatus(string DeclareId) {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty },
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateXmlStatus", keyValues);
        }
        public static Prescriptions GetCooperaPrescriptionsDataByCusIdNumber(string pharmcyMedicalNum, string cusIdnum) {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmcyMedicalNum",pharmcyMedicalNum },
                     {"cusIdnum",cusIdnum }
            };
            Prescriptions prescriptions = new Prescriptions();
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByMedicalNum", keyValues);
            XmlSerializer ser = new XmlSerializer(typeof(CooperativePrescription));
            foreach (XmlDocument xmlDocument in table)
            {
                using (TextReader sr = new StringReader(xmlDocument.InnerXml))
                {
                    CooperativePrescription response = (CooperativePrescription)ser.Deserialize(sr);
                    prescriptions.Add(new Prescription(response));
                }
            }
            return prescriptions;
        }
        public static Prescriptions GetCooperaPrescriptionsDataByDate(string pharmcyMedicalNum, DateTime sDate, DateTime eDate) {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"pharmcyMedicalNum",pharmcyMedicalNum },
                     {"sDate",sDate.ToString("yyyy-MM-dd") },
                     {"eDate",eDate.ToString("yyyy-MM-dd") }
                };
            Prescriptions prescriptions = new Prescriptions();
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByDate", keyValues);
            foreach (XmlDocument xmlDocument in table)
            {
                prescriptions.Add(new Prescription(XmlService.Deserialize<CooperativePrescription>(xmlDocument.InnerXml)));
            }
            return prescriptions;
        }
        #endregion
        #region TableSet
        public static DataTable SetPrescriptionMaster(Prescription p) { 
            DataTable prescriptionMasterTable = PrescriptionMasterTable();
            DataRow newRow = prescriptionMasterTable.NewRow();
            newRow["PreMas_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CustomerID", p.Patient.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DeclareFileID", p.DeclareFileID);
            newRow["PreMas_ImportFileID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustCaseID", p.Treatment.AdjustCase.ID);
            newRow["PreMas_SerialNumber"] = DBNull.Value;
            newRow["PreMas_PharmacyID"] = ViewModelMainWindow.CurrentPharmacy.ID;
            newRow["PreMas_MakeUpMarkID"] =  DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PaymentCategoryID", p.Treatment.PaymentCategory?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalNumber", p.Treatment.MedicalNumber);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MainDiseaseID", p.Treatment.MainDisease?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SecondDiseaseID", p.Treatment.SubDisease?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DivisionID", p.Treatment.Division?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentDate", p.Treatment.TreatDate);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentID", p.Treatment.Copayment?.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ApplyPoint", p.PrescriptionPoint.ApplyPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentPoint", p.PrescriptionPoint.CopaymentPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TotalPoint", p.PrescriptionPoint.TotalPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_InstitutionID", p.Treatment.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PrescriptionCaseID", p.Treatment.PrescriptionCase?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustDate", p.Treatment.AdjustDate);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DoctorIDNumber", p.Treatment.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PharmacistIDNumber", p.Treatment.Pharmacist.IdNumber);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialTreatID", p.Treatment.SpecialTreat?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicineDays", p.MedicineDays);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialMaterialPoint", p.PrescriptionPoint.SpecialMaterialPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentPoint", p.PrescriptionPoint.TreatmentPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicinePoint", p.PrescriptionPoint.MedicinePoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicSequence", p.Treatment.ChronicSeq);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicTotal", p.Treatment.ChronicTotal);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServiceID", p.MedicalServiceID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServicePoint", p.PrescriptionPoint.MedicalServicePoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_OldMedicalNumber", p.Treatment.OriginalMedicalNumber);
            if (string.IsNullOrEmpty(XmlService.ToXmlDocument(p.DeclareContent).InnerXml))
                newRow["PreMas_DeclareContent"] = DBNull.Value;
            else
                newRow["PreMas_DeclareContent"] = new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(p.DeclareContent).InnerXml, XmlNodeType.Document, null));
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsSendToServer", p.PrescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsGetCard", p.PrescriptionStatus.IsGetCard);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeclare", p.PrescriptionStatus.IsDeclare);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeposit", p.PrescriptionStatus.IsDeposit);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsAdjust", p.PrescriptionStatus.IsAdjust);
            
            prescriptionMasterTable.Rows.Add(newRow);
            return prescriptionMasterTable; 
        }
        public static DataTable SetPrescriptionDetail(Prescription p, List<Pdata> prescriptionDetails) { //一般藥費
            DataTable prescriptionDetailTable = PrescriptionDetailTable();
            foreach (var pdata in prescriptionDetails)
            {
                DataRow newRow = prescriptionDetailTable.NewRow();
                newRow["PreDet_PrescriptionID"] = DBNull.Value;
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicalOrderID", pdata.P1);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Percentage", pdata.P6);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_SerialNumber", pdata.P10);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Point", pdata.P9);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineID", pdata.P2);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Dosage", pdata.P3);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Usage", pdata.P4);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Position", pdata.P5);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_TotalAmount", pdata.P7);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Price", pdata.P8);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineDays", pdata.P11);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_PaySelf", pdata.PaySelf);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_IsBuckle", pdata.IsBuckle);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_PaySelfValue", pdata.PaySelfValue);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_BuckleAmount", pdata.BuckleAmount);
                prescriptionDetailTable.Rows.Add(newRow);
            }
            return prescriptionDetailTable;
        }
        public static DataTable PrescriptionMasterTable() {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("PreMas_ID", typeof(int));
            masterTable.Columns.Add("PreMas_CustomerID", typeof(int));
            masterTable.Columns.Add("PreMas_DeclareFileID", typeof(int));
            masterTable.Columns.Add("PreMas_ImportFileID", typeof(int));
            masterTable.Columns.Add("PreMas_AdjustCaseID", typeof(string));
            masterTable.Columns.Add("PreMas_SerialNumber", typeof(int));
            masterTable.Columns.Add("PreMas_PharmacyID", typeof(string));
            masterTable.Columns.Add("PreMas_MakeUpMarkID", typeof(string));
            masterTable.Columns.Add("PreMas_PaymentCategoryID", typeof(string));
            masterTable.Columns.Add("PreMas_MedicalNumber", typeof(string));
            masterTable.Columns.Add("PreMas_MainDiseaseID", typeof(string));
            masterTable.Columns.Add("PreMas_SecondDiseaseID", typeof(string));
            masterTable.Columns.Add("PreMas_DivisionID", typeof(string));
            masterTable.Columns.Add("PreMas_TreatmentDate", typeof(DateTime));
            masterTable.Columns.Add("PreMas_CopaymentID", typeof(string));
            masterTable.Columns.Add("PreMas_ApplyPoint", typeof(int));
            masterTable.Columns.Add("PreMas_CopaymentPoint", typeof(short));
            masterTable.Columns.Add("PreMas_TotalPoint", typeof(int));
            masterTable.Columns.Add("PreMas_InstitutionID", typeof(string));
            masterTable.Columns.Add("PreMas_PrescriptionCaseID", typeof(string));
            masterTable.Columns.Add("PreMas_AdjustDate", typeof(DateTime));
            masterTable.Columns.Add("PreMas_DoctorIDNumber", typeof(string));
            masterTable.Columns.Add("PreMas_PharmacistIDNumber", typeof(string));
            masterTable.Columns.Add("PreMas_SpecialTreatID", typeof(string));
            masterTable.Columns.Add("PreMas_MedicineDays", typeof(short));
            masterTable.Columns.Add("PreMas_SpecialMaterialPoint", typeof(int));
            masterTable.Columns.Add("PreMas_TreatmentPoint", typeof(int));
            masterTable.Columns.Add("PreMas_MedicinePoint", typeof(int));
            masterTable.Columns.Add("PreMas_ChronicSequence", typeof(short));
            masterTable.Columns.Add("PreMas_ChronicTotal", typeof(short));
            masterTable.Columns.Add("PreMas_MedicalServiceID", typeof(string));
            masterTable.Columns.Add("PreMas_MedicalServicePoint", typeof(int));
            masterTable.Columns.Add("PreMas_OldMedicalNumber", typeof(string));
            masterTable.Columns.Add("PreMas_DeclareContent", typeof(SqlXml));
            masterTable.Columns.Add("PreMas_IsSendToServer", typeof(bool));
            masterTable.Columns.Add("PreMas_IsGetCard", typeof(bool));
            masterTable.Columns.Add("PreMas_IsDeclare", typeof(bool));
            masterTable.Columns.Add("PreMas_IsDeposit", typeof(bool));
            masterTable.Columns.Add("PreMas_IsAdjust", typeof(bool));  
            return masterTable;
    }
        public static DataTable PrescriptionDetailTable() {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("PreDet_PrescriptionID", typeof(int));
            detailTable.Columns.Add("PreDet_MedicalOrderID", typeof(String));
            detailTable.Columns.Add("PreDet_MedicineID", typeof(String));
            detailTable.Columns.Add("PreDet_Dosage", typeof(float));
            detailTable.Columns.Add("PreDet_Usage", typeof(String));
            detailTable.Columns.Add("PreDet_Position", typeof(String));
            detailTable.Columns.Add("PreDet_Percentage", typeof(int));
            detailTable.Columns.Add("PreDet_TotalAmount", typeof(float));
            detailTable.Columns.Add("PreDet_Price", typeof(double));
            detailTable.Columns.Add("PreDet_Point", typeof(int));
            detailTable.Columns.Add("PreDet_SerialNumber", typeof(int));
            detailTable.Columns.Add("PreDet_MedicineDays", typeof(int));
            detailTable.Columns.Add("PreDet_PaySelf", typeof(int));
            detailTable.Columns.Add("PreDet_IsBuckle", typeof(int));
            detailTable.Columns.Add("PreDet_PaySelfValue", typeof(double));
            detailTable.Columns.Add("PreDet_BuckleAmount", typeof(float)); 
            return detailTable;
        }
        public static DataTable SetReserveMaster(Prescription p) {
            DataTable reserveMasterTable = ReserveMasterTable();
            DataRow newRow = reserveMasterTable.NewRow();
            newRow["ResMas_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CustomerID", p.Patient.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DeclareFileID", p.DeclareFileID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_AdjustCaseID", p.Treatment.AdjustCase.ID);
            newRow["ResMas_SerialNumber"] = DBNull.Value;
            newRow["ResMas_MakeUpMarkID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PaymentCategoryID", p.Treatment.PaymentCategory.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalNumber", p.Treatment.MedicalNumber); 
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MainDiseaseID", p.Treatment.MainDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SecondDiseaseID", p.Treatment.SubDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DivisionID", p.Treatment.Division.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TreatmentDate", p.Treatment.TreatDate);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CopaymentID", p.Treatment.Copayment.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ApplyPoint", p.PrescriptionPoint.ApplyPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CopaymentPoint", p.PrescriptionPoint.CopaymentPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TotalPoint", p.PrescriptionPoint.TotalPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_InstitutionID", p.Treatment.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PrescriptionCaseID", p.Treatment.PrescriptionCase.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_AdjustDate", p.Treatment.AdjustDate);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DoctorIDNumber", p.Treatment.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PharmacistIDNumber", p.Treatment.Pharmacist.IdNumber);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SpecialTreatID", p.Treatment.SpecialTreat?.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicineDays", p.MedicineDays);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SpecialMaterialPoint", p.PrescriptionPoint.SpecialMaterialPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TreatmentPoint", p.PrescriptionPoint.TreatmentPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicinePoint", p.PrescriptionPoint.MedicinePoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ChronicSequence", p.Treatment.ChronicSeq);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ChronicTotal", p.Treatment.ChronicTotal);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalServiceID", p.MedicalServiceID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalServicePoint", p.PrescriptionPoint.MedicalServicePoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_OldMedicalNumber", p.Treatment.OriginalMedicalNumber);
            if (string.IsNullOrEmpty(XmlService.ToXmlDocument(p.DeclareContent).InnerXml))
                newRow["ResMas_DeclareContent"] = DBNull.Value;
            else
                newRow["ResMas_DeclareContent"] = new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(p.DeclareContent).InnerXml, XmlNodeType.Document, null));
            DataBaseFunction.AddColumnValue(newRow, "ResMas_IsSendToServer", p.PrescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_IsRegister", p.PrescriptionStatus.IsRegister);
            reserveMasterTable.Rows.Add(newRow);
            return reserveMasterTable;
        }
        public static DataTable SetReserveionDetail(Prescription p, List<Pdata> prescriptionDetails) { //一般藥費
            
            DataTable reserveDetailTable = ReserveDetailTable();

            foreach (var pdata in prescriptionDetails)
            {
                DataRow newRow = reserveDetailTable.NewRow();
                newRow["ResDet_ReserveID"] = DBNull.Value;
                DataBaseFunction.AddColumnValue(newRow, "ResDet_MedicalOrderID", pdata.P1);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_Percentage", pdata.P6);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_SerialNumber", pdata.P10);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_Point", pdata.P9);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_MedicineID", pdata.P2);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_Dosage", pdata.P3);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_Usage", pdata.P4);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_Position", pdata.P5);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_TotalAmount", pdata.P7);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_Price", pdata.P8);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_MedicineDays", pdata.P11);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_PaySelf", pdata.PaySelf);
                DataBaseFunction.AddColumnValue(newRow, "ResDet_IsBuckle", pdata.IsBuckle);
                
                reserveDetailTable.Rows.Add(newRow); 
            } 
            return reserveDetailTable;
        }
        public static DataTable SetImportDeclareXmlMaster(List<ImportDeclareXml.ImportDeclareXml.Ddata> Ddatas, List<string> declareFiles,int preId,Customers cs, string fileId) {
            DataTable prescriptionMasterTable = PrescriptionMasterTable();
            int filecount = 0;
            foreach(var d in Ddatas)
            {
                DataRow newRow = prescriptionMasterTable.NewRow();
                newRow["PreMas_ID"] = preId;
                DataBaseFunction.AddColumnValue(newRow, "PreMas_CustomerID", cs.Single(c => c.IDNumber == d.D3).ID); 
                newRow["PreMas_DeclareFileID"] = DBNull.Value;
                newRow["PreMas_ImportFileID"] = fileId;
                DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustCaseID", d.D1);
                newRow["PreMas_SerialNumber"] = d.D2;
                newRow["PreMas_PharmacyID"] = ViewModelMainWindow.CurrentPharmacy.ID;
                newRow["PreMas_MakeUpMarkID"] = DBNull.Value;
                DataBaseFunction.AddColumnValue(newRow, "PreMas_PaymentCategoryID", d.D5);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalNumber", d.D7);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MainDiseaseID", d.D8);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_SecondDiseaseID", d.D9);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_DivisionID", d.D13);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentDate", Convert.ToDateTime((Convert.ToInt32(d.D14.Substring(0, 3)) + 1911).ToString() + "/" + d.D14.Substring(3, 2) + "/" + d.D14.Substring(5, 2)));  
                DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentID", ViewModelMainWindow.GetCopayment(d.D15)?.Id );
                DataBaseFunction.AddColumnValue(newRow, "PreMas_ApplyPoint", d.D16);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentPoint",  d.D17);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_TotalPoint", d.D18);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_InstitutionID", d.D21);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_PrescriptionCaseID", d.D22);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustDate", Convert.ToDateTime((Convert.ToInt32(d.D23.Substring(0, 3)) + 1911).ToString() + "/" + d.D23.Substring(3, 2) + "/" + d.D23.Substring(5, 2)));
                DataBaseFunction.AddColumnValue(newRow, "PreMas_DoctorIDNumber", d.D24);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_PharmacistIDNumber", d.D25);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialTreatID", d.D26);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicineDays", d.D30);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialMaterialPoint", d.D31);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentPoint", d.D32);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicinePoint",d.D33);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicSequence", d.D35);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicTotal", d.D36);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServiceID", d.D37);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServicePoint", d.D38);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_OldMedicalNumber", d.D43);
                newRow["PreMas_DeclareContent"] = new SqlXml(new XmlTextReader("<ddata>" + declareFiles[filecount] + "</ddata>", XmlNodeType.Document, null)); 
                DataBaseFunction.AddColumnValue(newRow, "PreMas_IsSendToServer", false);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_IsGetCard", true);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeclare", true);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeposit", false);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_IsAdjust", false);
                
                prescriptionMasterTable.Rows.Add(newRow);
                preId++;
                filecount++;
            } 
            return prescriptionMasterTable;
        }
        public static DataTable SetImportDeclareXmlDetail(List<ImportDeclareXml.ImportDeclareXml.Ddata> Ddatas, int preId) { 
            DataTable prescriptionDetailTable = PrescriptionDetailTable();
            foreach(var d in Ddatas)
            { 
                foreach (var pdata in d.Pdatas)
                {
                    DataRow newRow = prescriptionDetailTable.NewRow();
                    newRow["PreDet_PrescriptionID"] = preId;
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicalOrderID", pdata.P1);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_Percentage", pdata.P6);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_SerialNumber", pdata.P10);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_Point", pdata.P9);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineID", pdata.P2);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_Dosage", pdata.P3);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_Usage", pdata.P4);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_Position", pdata.P5);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_TotalAmount", pdata.P7);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_Price", pdata.P8);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineDays", pdata.P11);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_PaySelf", false);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_IsBuckle", false);
                    prescriptionDetailTable.Rows.Add(newRow);
                }
                preId++;
            } 
            return prescriptionDetailTable;
        }
        public static DataTable ReserveMasterTable() {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("ResMas_ID", typeof(int));
            masterTable.Columns.Add("ResMas_CustomerID", typeof(int));
            masterTable.Columns.Add("ResMas_DeclareFileID", typeof(string));
            masterTable.Columns.Add("ResMas_AdjustCaseID", typeof(string));
            masterTable.Columns.Add("ResMas_SerialNumber", typeof(int));
            masterTable.Columns.Add("ResMas_MakeUpMarkID", typeof(string));
            masterTable.Columns.Add("ResMas_PaymentCategoryID", typeof(string));
            masterTable.Columns.Add("ResMas_MedicalNumber", typeof(string));
            masterTable.Columns.Add("ResMas_MainDiseaseID", typeof(string));
            masterTable.Columns.Add("ResMas_SecondDiseaseID", typeof(string));
            masterTable.Columns.Add("ResMas_DivisionID", typeof(string));
            masterTable.Columns.Add("ResMas_TreatmentDate", typeof(DateTime));
            masterTable.Columns.Add("ResMas_CopaymentID", typeof(string));
            masterTable.Columns.Add("ResMas_ApplyPoint", typeof(int));
            masterTable.Columns.Add("ResMas_CopaymentPoint", typeof(int));
            masterTable.Columns.Add("ResMas_TotalPoint", typeof(int));
            masterTable.Columns.Add("ResMas_InstitutionID", typeof(string));
            masterTable.Columns.Add("ResMas_PrescriptionCaseID", typeof(string));
            masterTable.Columns.Add("ResMas_AdjustDate", typeof(DateTime));
            masterTable.Columns.Add("ResMas_DoctorIDNumber", typeof(string));
            masterTable.Columns.Add("ResMas_PharmacistIDNumber", typeof(string));
            masterTable.Columns.Add("ResMas_SpecialTreatID", typeof(string));
            masterTable.Columns.Add("ResMas_MedicineDays", typeof(int));
            masterTable.Columns.Add("ResMas_SpecialMaterialPoint", typeof(int));
            masterTable.Columns.Add("ResMas_TreatmentPoint", typeof(int));
            masterTable.Columns.Add("ResMas_MedicinePoint", typeof(int));
            masterTable.Columns.Add("ResMas_ChronicSequence", typeof(int));
            masterTable.Columns.Add("ResMas_ChronicTotal", typeof(int));
            masterTable.Columns.Add("ResMas_MedicalServiceID", typeof(string));
            masterTable.Columns.Add("ResMas_MedicalServicePoint", typeof(int));
            masterTable.Columns.Add("ResMas_OldMedicalNumber", typeof(string));
            masterTable.Columns.Add("ResMas_DeclareContent", typeof(SqlXml));
            masterTable.Columns.Add("ResMas_IsSendToServer", typeof(bool));
            masterTable.Columns.Add("ResMas_IsRegister", typeof(bool));
            return masterTable; 
    }
        public static DataTable ReserveDetailTable() {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("ResDet_ReserveID", typeof(int));
            detailTable.Columns.Add("ResDet_MedicalOrderID", typeof(string));
            detailTable.Columns.Add("ResDet_MedicineID", typeof(string));
            detailTable.Columns.Add("ResDet_Dosage", typeof(float));
            detailTable.Columns.Add("ResDet_Usage", typeof(string));
            detailTable.Columns.Add("ResDet_Position", typeof(string));
            detailTable.Columns.Add("ResDet_Percentage", typeof(int));
            detailTable.Columns.Add("ResDet_TotalAmount", typeof(float));
            detailTable.Columns.Add("ResDet_Price", typeof(double));
            detailTable.Columns.Add("ResDet_Point", typeof(int));
            detailTable.Columns.Add("ResDet_SerialNumber", typeof(int));
            detailTable.Columns.Add("ResDet_MedicineDays", typeof(int));
            detailTable.Columns.Add("ResDet_PaySelf", typeof(bool));
            detailTable.Columns.Add("ResDet_IsBuckle", typeof(bool));
            return detailTable;
        }
        private static DataTable IDTable()
        {
            DataTable idTable = new DataTable();
            idTable.Columns.Add("ID", typeof(string));
            return idTable;
        }
        public static DataTable SetIDTable(List<int> IDList)
        {
            DataTable table = IDTable();
            foreach (int id in IDList)
            {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "ID", id);
                table.Rows.Add(newRow);
            }
            return table;
        }
        #endregion
    }
}
