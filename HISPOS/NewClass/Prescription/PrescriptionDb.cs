using Dapper;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using His_Pos.HisApi;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.ICCard.Upload;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Product.PrescriptionSendData;
using His_Pos.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using DateTimeEx = His_Pos.Service.DateTimeExtensions;

// ReSharper disable TooManyArguments

namespace His_Pos.NewClass.Prescription
{
    public static class PrescriptionDb
    {
        public static DataTable InsertPrescriptionByType(Prescription prescription, List<Pdata> prescriptionDetails)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "type", prescription.Type.ToString());
            DataBaseFunction.AddSqlParameter(parameterList, "warID", prescription.WareHouse is null ? 0 : int.Parse(prescription.WareHouse.ID));
            DataBaseFunction.AddSqlParameter(parameterList, "IsCooperativeVIP", prescription.PrescriptionStatus.IsVIP);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceID", string.IsNullOrEmpty(prescription.SourceId) ? null : prescription.SourceId);
            DataBaseFunction.AddSqlParameter(parameterList, "Remark", string.IsNullOrEmpty(prescription.Remark) ? null : prescription.Remark);
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionMaster", SetPrescriptionMaster(prescription));
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionDetail", SetPrescriptionDetail(prescriptionDetails));
            DataBaseFunction.AddSqlParameter(parameterList, "Emp", ViewModelMainWindow.CurrentUser.ID);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescriptionByType]", parameterList);
        }

        internal static DataTable GetMedBagPrescriptionStructsByType(string type)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "TYPE", type);

            return MainWindow.ServerConnection.ExecuteProc("[Get].[MedBagPrescriptionStructsByType]", parameterList);
        }

        public static void DeleteReserve(string recMasId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "RecMas_Id", recMasId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteReserve]", parameterList);
        }

        public static void UpdateReserve(Prescription prescription, List<Pdata> prescriptionDetails)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataTable resMaster = SetReserveMaster(prescription);
            resMaster.Rows[0]["ResMas_ID"] = prescription.SourceId;
            DataBaseFunction.AddSqlParameter(parameterList, "ResMaster", resMaster);
            DataBaseFunction.AddSqlParameter(parameterList, "ResDetail", SetReserveDetail(prescriptionDetails));
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateReserve]", parameterList);
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

        public static void ProcessCashFlow(string cashFlowName, string source, int sourceId, double total)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Name", cashFlowName);
            DataBaseFunction.AddSqlParameter(parameterList, "Value", total);
            DataBaseFunction.AddSqlParameter(parameterList, "Source", source);
            DataBaseFunction.AddSqlParameter(parameterList, "SourceId", sourceId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCashFlow]", parameterList);
        }

        public static DataTable GetPrescriptionCountByID(string pharmacistIdnum)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "EmpIdNum", pharmacistIdnum);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionCount]", parameterList);
        }

        public static DataTable GetPrescriptionId()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionId]");
        }

        public static DataTable GetSearchPrescriptionsDataRe(Dictionary<string, string> conditionTypes, Dictionary<string, string> conditions, Dictionary<string, DateTime?> dates, AdjustCase adjustCase, List<string> insIDList, Division division)
        {
            var parameterList = new List<SqlParameter>();
            AddTimeIntervalParameters(parameterList, conditionTypes["TimeInterval"], dates["sDate"], dates["eDate"]);
            AddPatientParameters(parameterList, conditionTypes["Patient"], conditions["Patient"]);
            AddMedicineParameters(parameterList, conditionTypes["Medicine"], conditions["Medicine"]);
            DataBaseFunction.AddSqlParameter(parameterList, "PatientBirthday", dates["PatientBirthday"]);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustCase", adjustCase?.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Division", division?.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Institutions", SetStringIDTable(insIDList));
            switch (conditionTypes["TimeInterval"])
            {
                case "預約日":
                    return MainWindow.ServerConnection.ExecuteProc("[Get].[SearchReservePrescriptions]", parameterList);

                default:
                    return MainWindow.ServerConnection.ExecuteProc("[Get].[SearchPrescriptions]", parameterList);
            }
        }

        public static DataTable GetPrescriptionForImportXml()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionForImportXml]");
        }
          
        private static void AddMedicineParameters(List<SqlParameter> parameterList, string conditionType, string medicineCondition)
        {
            switch (conditionType)
            {
                case "藥品代碼":
                    DataBaseFunction.AddSqlParameter(parameterList, "MedicineID", medicineCondition);
                    DataBaseFunction.AddSqlParameter(parameterList, "MedicineName", DBNull.Value);
                    break;

                case "藥品名稱":
                    DataBaseFunction.AddSqlParameter(parameterList, "MedicineID", DBNull.Value);
                    DataBaseFunction.AddSqlParameter(parameterList, "MedicineName", medicineCondition);
                    break;
            }
        }

        private static void AddPatientParameters(List<SqlParameter> parameterList, string patientConditionType, string patientCondition)
        {
            switch (patientConditionType)
            {
                case "姓名":
                    DataBaseFunction.AddSqlParameter(parameterList, "PatientName", patientCondition);
                    DataBaseFunction.AddSqlParameter(parameterList, "PatientIDNumber", DBNull.Value);
                    break;

                case "身分證":
                    DataBaseFunction.AddSqlParameter(parameterList, "PatientName", DBNull.Value);
                    DataBaseFunction.AddSqlParameter(parameterList, "PatientIDNumber", patientCondition);
                    break;
            }
        }

        private static void AddTimeIntervalParameters(List<SqlParameter> parameterList, string dateType, DateTime? sDate, DateTime? eDate)
        {
            switch (dateType)
            {
                case "調劑日":
                    DataBaseFunction.AddSqlParameter(parameterList, "AdjustDateStart", sDate);
                    DataBaseFunction.AddSqlParameter(parameterList, "AdjustDateEnd", eDate);
                    DataBaseFunction.AddSqlParameter(parameterList, "RegisterDateStart", DBNull.Value);
                    DataBaseFunction.AddSqlParameter(parameterList, "RegisterDateEnd", DBNull.Value);
                    break;

                case "登錄日":
                    DataBaseFunction.AddSqlParameter(parameterList, "AdjustDateStart", DBNull.Value);
                    DataBaseFunction.AddSqlParameter(parameterList, "AdjustDateEnd", DBNull.Value);
                    DataBaseFunction.AddSqlParameter(parameterList, "RegisterDateStart", sDate);
                    DataBaseFunction.AddSqlParameter(parameterList, "RegisterDateEnd", eDate);
                    break;

                case "預約日":
                    DataBaseFunction.AddSqlParameter(parameterList, "AdjustDateStart", sDate);
                    DataBaseFunction.AddSqlParameter(parameterList, "AdjustDateEnd", eDate);
                    break;
            }
        }

        public static DataTable GetNoBucklePrescriptions()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[NoBucklePrescriptions]");
        }

        public static DataTable GetSearchPrescriptionsData(DateTime? sDate, DateTime? eDate, string patientName, string patientIDNumber, DateTime? patientBirth, AdjustCase adj, string medID, string medName, Institution ins, Division div)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "CusName", patientName);
            DataBaseFunction.AddSqlParameter(parameterList, "CusIDNum", patientIDNumber);
            DataBaseFunction.AddSqlParameter(parameterList, "CusBirth", patientBirth);
            DataBaseFunction.AddSqlParameter(parameterList, "MedID", string.IsNullOrEmpty(medID) ? null : medID);
            DataBaseFunction.AddSqlParameter(parameterList, "MedName", string.IsNullOrEmpty(medName) ? null : medName);
            DataBaseFunction.AddSqlParameter(parameterList, "InsId", ins?.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustCaseId", adj?.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "DivId", div?.ID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionBySearchCondition]", parameterList);
        }

        public static DataTable GetReservePrescriptionsData(DateTime? sDate, DateTime? eDate, string patientName, string patientIDNumber, DateTime? patientBirth, AdjustCase adj, string medID, string medName, Institution ins, Division div)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "CusName", patientName);
            DataBaseFunction.AddSqlParameter(parameterList, "CusIDNum", patientIDNumber);
            DataBaseFunction.AddSqlParameter(parameterList, "CusBirth", patientBirth);
            DataBaseFunction.AddSqlParameter(parameterList, "MedID", string.IsNullOrEmpty(medID) ? null : medID);
            DataBaseFunction.AddSqlParameter(parameterList, "MedName", string.IsNullOrEmpty(medName) ? null : medName);
            DataBaseFunction.AddSqlParameter(parameterList, "InsId", ins == null ? null : ins.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustCaseId", adj == null ? null : adj.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "DivId", div == null ? null : div.ID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReserveBySearchCondition]", parameterList);
        }

        public static DataTable GetPrescriptionsNoGetCardByCusId(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusId", cusId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionsNoGetCardByCusId]", parameterList);
        }

        public static DataTable GetXmlOfPrescriptionsByCusIDNumber(string cusIdNumber)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusIdNumber", cusIdNumber);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[XmlOfPrescriptionsByCusIDNumber]", parameterList);
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

        public static DataTable GetRegisterPrescriptionByCusId(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusId", cusId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[RegisterPrescriptionByCusId]", parameterList);
        }

        public static void ImportDeclareXml(List<ImportDeclareXml.ImportDeclareXml.Ddata> ddatas, List<string> declareFiles, string fileId)
        {
            Customers cs = new Customers();
            cs = cs.SetCustomersByPrescriptions(ddatas);
            var preId = GetPrescriptionId().Rows[0].Field<int>("MaxPreId");
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionMaster", SetImportDeclareXmlMaster(ddatas, declareFiles, preId, cs, fileId));
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionDetail", SetImportDeclareXmlDetail(ddatas, preId));
            MainWindow.ServerConnection.ExecuteProc("[Set].[ImportDeclareXml]", parameterList);
             
          
        }

        public static DataTable UpdatePrescriptionByType(Prescription prescription, List<Pdata> prescriptionDetails)
        {
            int warID = 0;
            if (ViewModelMainWindow.CooperativeClinicSettings.GetWareHouseByPrescription(prescription.Institution, prescription.AdjustCase.ID) != null)
                warID = int.Parse(ViewModelMainWindow.CooperativeClinicSettings.GetWareHouseByPrescription(prescription.Institution, prescription.AdjustCase.ID).ID);
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataTable prescriptionMater = SetPrescriptionMaster(prescription);
            prescriptionMater.Rows[0]["PreMas_ID"] = prescription.ID;
            DataBaseFunction.AddSqlParameter(parameterList, "type", prescription.Type.ToString());
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionMaster", prescriptionMater);
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionDetail", SetPrescriptionDetail(prescriptionDetails));
            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePrescriptionByType]", parameterList);
        }

        public static DataTable CheckImportDeclareFileExist(string head)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Head", head);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[CheckImportDeclareFileExist]", parameterList);
        }

        public static void PredictThreeMonthPrescription()
        {
            MainWindow.ServerConnection.ExecuteProc("[Set].[PredictThreeMonthPrescription]");
        }

        public static DataTable DeletePrescription(Prescription prescription)
        {
            int warID = 0;
            if (ViewModelMainWindow.CooperativeClinicSettings.GetWareHouseByPrescription(prescription.Institution, prescription.AdjustCase.ID) != null)
                warID = int.Parse(ViewModelMainWindow.CooperativeClinicSettings.GetWareHouseByPrescription(prescription.Institution, prescription.AdjustCase.ID).ID);
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreId", prescription.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "warID", warID);
            DataBaseFunction.AddSqlParameter(parameterList, "type", prescription.Type.ToString());
            return MainWindow.ServerConnection.ExecuteProc("[Set].[DeletePrescription]", parameterList);
        }

        public static DataTable GetStoreOrderIDByPrescriptionID(int preId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PREMAS_ID", preId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StoreOrderIDByPrescriptionID]", parameterList);
        }

        public static bool SendDeclareOrderToSingde(string storId, Prescription p, PrescriptionSendDatas prescriptionSendData)
        {
            return SendOrderAction(storId, p, prescriptionSendData, "AddDeclareOrderToPreDrug") == "SUCCESS";
        }

        public static int UpdateDeclareOrderToSingde(string storId, Prescription p, PrescriptionSendDatas prescriptionSendData)
        {
            switch (SendOrderAction(storId, p, prescriptionSendData, "UpdateDeclareOrder"))
            {
                case "SUCCESS":
                    return 1;

                case "DONE":
                    return 2;

                default:
                    return 0;
            }
        }

        private static string SendOrderAction(string storId, Prescription p, PrescriptionSendDatas prescriptionSendData, string sql)
        {
            var rxOrder = Convert.ToDateTime(p.AdjustDate).AddYears(-1911).ToString("yyyMMdd"); // 調劑日期(7)病歷號(9)
            var dtlData = new StringBuilder(); //  備註text  處方資訊
            AppendPatientData(dtlData, p);
            AppendInstitutionData(dtlData, p);
            AppendTreatmentData(dtlData, p);
            AppendMedicineCost(dtlData, p);
            AppendMedicinesData(dtlData, p, prescriptionSendData);
            var result = "FAIL";
            switch (sql)
            {
                case "AddDeclareOrderToPreDrug":
                    result = AddDeclareOrderToPreDrug(storId, p.Patient.Name, dtlData, p.AdjustDate);
                    break;

                case "UpdateDeclareOrder":
                    result = UpdateDeclareOrder(storId, rxOrder, dtlData);
                    break;
            }
            return result;
        }

        private static string UpdateDeclareOrder(string storId, string rxOrder, StringBuilder dtlData)
        {
            var rxID = ViewModelMainWindow.CurrentPharmacy.ID; //藥局機構代號 傳輸主KEY
            var result = "FAIL";
            var table = MainWindow.SingdeConnection.ExecuteProc($"call UpdateDeclareOrderData('{rxID}','{storId}','{rxOrder}','{dtlData}')");
            if (table.Rows.Count > 0 && table.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                result = "SUCCESS";
            else if (table.Rows.Count > 0 && table.Rows[0].Field<string>("RESULT").Equals("DONE"))
                result = "DONE";
            return result;
        }

        private static string AddDeclareOrderToPreDrug(string storId, string patientName, StringBuilder dtlData, DateTime? adjustDate)
        {
            var rxID = ViewModelMainWindow.CurrentPharmacy.ID; //藥局機構代號 傳輸主KEY
            var result = "FAIL";
            Debug.Assert(adjustDate != null, nameof(adjustDate) + " != null");
            var table = MainWindow.SingdeConnection.ExecuteProc($"call AddDeclareOrderToPreDrug('{rxID}', '{storId}', '{patientName}','{dtlData}','{((DateTime)adjustDate).AddYears(-1911):yyyMMdd}')");
            if (table.Rows.Count > 0 && table.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                result = "SUCCESS";
            return result;
        }

        private static void AppendPatientData(StringBuilder dtlData, Prescription p)
        {
            //第一行
            dtlData.Append(p.ID.ToString().PadLeft(8, '0')); //藥局病歷號
            dtlData.Append(p.Patient.Name.PadRight(20 - NewFunction.HowManyChinese(p.Patient.Name), ' ')); //病患姓名
            dtlData.Append(p.Patient.IDNumber.Substring(0, 2).PadRight(10, '0')); //身分證字號
            Debug.Assert(p.Patient.Birthday != null, "p.Patient.Birthday != null");
            //dtlData.Append(DateTimeExtensions.ConvertToTaiwanCalender((DateTime)p.Patient.Birthday)); //出生年月日
            dtlData.Append("0000000"); //出生年月日
            p.Patient.CheckGender();
            var gender = p.Patient.Gender.Equals("男") ? "1" : "2";
            dtlData.Append(gender.PadRight(1, ' ')); //性別判斷 1男 2女
            AppendPatientTel(dtlData, p.Patient, p);
            dtlData.AppendLine();
        }

        private static void AppendPatientTel(StringBuilder dtlData, Customer patient, Prescription p)
        {
            string patientTel = PrescriptionService.BuildPatientTel(p);
            dtlData.Append(string.IsNullOrEmpty(patientTel) ? string.Empty.PadRight(20, ' ') : patientTel.PadRight(20, ' ')); //電話
        }

        private static void AppendInstitutionData(StringBuilder dtlData, Prescription p)
        {
            //第二行
            dtlData.Append(p.Institution.ID.PadRight(10, ' ')); //院所代號
            dtlData.Append(p.Institution.ID.PadRight(10, ' ')); //診治醫師代號 (同院所代號)
            dtlData.Append(string.Empty.PadRight(20, ' ')); //空
            dtlData.Append(ViewModelMainWindow.CurrentUser.ID.ToString().PadRight(10, ' ')); //藥師代號
            dtlData.Append(ViewModelMainWindow.CurrentUser.Name.PadRight(20 - NewFunction.HowManyChinese(ViewModelMainWindow.CurrentUser.Name), ' ')); //藥師姓名
            dtlData.AppendLine();
        }

        private static void AppendTreatmentData(StringBuilder dtlData, Prescription p)
        {
            //第三行
            Debug.Assert(p.TreatDate != null, "p.TreatDate != null");
            Debug.Assert(p.AdjustDate != null, "p.AdjustDate != null");
            dtlData.Append(((DateTime)p.TreatDate).AddYears(-1911).ToString("yyyMMdd")); //處方日(就診日期)
            dtlData.Append(((DateTime)p.AdjustDate).AddYears(-1911).ToString("yyyMMdd")); //調劑日期
            dtlData.Append(p.PrescriptionCase.ID.PadRight(2, ' ')); //案件
            dtlData.Append(p.Division.ID.PadRight(2, ' ')); //科別
            dtlData.Append(p.MainDisease.ID.PadRight(10, ' ')); //主診斷
            dtlData.Append(string.IsNullOrEmpty(p.SubDisease?.ID) ? string.Empty.PadRight(10, ' ') : p.SubDisease.ID.PadRight(10, ' ')); //次診斷
            dtlData.Append(p.TempMedicalNumber == null ? string.Empty.PadRight(4, ' ') : p.TempMedicalNumber.PadRight(4, ' ')); //卡序 (0001、欠卡、自費)
            dtlData.Append("2".PadRight(1, ' ')); //1一般箋 2慢箋
            dtlData.Append(p.ChronicTotal.ToString().PadRight(1, ' ')); //可調劑次數
            dtlData.Append(p.ChronicSeq.ToString().PadRight(1, ' ')); //本次調劑次數
            var selfPay = p.PrescriptionPoint.AmountSelfPay ?? 0;
            dtlData.Append(selfPay.ToString().PadRight(8, ' ')); //自費金額
        }

        private static void AppendMedicineCost(StringBuilder dtlData, Prescription p)
        {
            double medCost = 0;
            foreach (var declareMedicine in p.Medicines)
            {
                if (declareMedicine is MedicineNHI || declareMedicine is MedicineSpecialMaterial)
                    medCost += declareMedicine.TotalPrice;
            }
            dtlData.Append(medCost.ToString().PadRight(8, ' ')); //藥品費
            var medicalPay = "0";
            if (Convert.ToInt32(p.MedicineDays) <= 13)
                medicalPay = "48";
            if (Convert.ToInt32(p.MedicineDays) >= 14 && Convert.ToInt32(p.MedicineDays) < 28)
                medicalPay = "59";
            if (Convert.ToInt32(p.MedicineDays) >= 28)
                medicalPay = "69";

            dtlData.Append(medicalPay.PadRight(4, ' ')); //藥事費
            dtlData.Append(p.PrescriptionPoint.CopaymentPoint.ToString().PadRight(4, ' ')); //部分負擔
            dtlData.AppendLine();
        }

        private static void AppendMedicinesData(StringBuilder dtlData, Prescription p, PrescriptionSendDatas prescriptionSendData)
        {
            //第四行
            var i = 1;
            foreach (var declareMedicine in p.Medicines.Where(m => !m.AdjustNoBuckle))
            {
                if (declareMedicine is MedicineNHI || declareMedicine is MedicineSpecialMaterial)
                {
                    AppendMedicineData(declareMedicine, dtlData, prescriptionSendData, i - 1);
                }
                if (i < p.Medicines.Count(med => med is MedicineNHI || med is MedicineSpecialMaterial))
                    dtlData.AppendLine();
                i++;
            }
        }

        private static void AppendMedicineData(Medicine.Base.Medicine declareMedicine, StringBuilder dtlData, PrescriptionSendDatas prescriptionSendData, int index)
        {
            dtlData.Append(declareMedicine.ID.Length > 12
                ? declareMedicine.ID.Substring(0, 12).PadRight(12, ' ')
                : declareMedicine.ID.PadRight(12, ' '));//健保碼
            dtlData.Append(declareMedicine.Dosage == null ? string.Empty.PadRight(8, ' ') : ((double)declareMedicine.Dosage).ToString("0.##").PadLeft(8, ' ')); //每次使用數量
            dtlData.Append(string.IsNullOrEmpty(declareMedicine.Usage?.Name) ? string.Empty.PadRight(16, ' ') : declareMedicine.Usage.Name.PadRight(16, ' ')); //使用頻率
            dtlData.Append(declareMedicine.Days == null ? string.Empty.PadRight(3, ' ') : declareMedicine.Days.ToString().PadRight(3, ' ')); //使用天數
            dtlData.Append(declareMedicine.Amount.ToString().PadRight(8, ' ')); //使用總量
            dtlData.Append(declareMedicine.ID.Length > 12
                ? declareMedicine.ID.Split('-')[1].PadRight(6, ' ')
                : declareMedicine.Position?.ID.PadRight(6, ' '));
            AppendAmountAndPaySelf(declareMedicine, dtlData, prescriptionSendData, index);
        }

        private static void AppendAmountAndPaySelf(Medicine.Base.Medicine declareMedicine, StringBuilder dtlData, PrescriptionSendDatas prescriptionSendData, int index)
        {
            if (!declareMedicine.PaySelf)
                dtlData.Append(" ");
            else
                dtlData.Append(declareMedicine.TotalPrice > 0 ? "Y" : "N".PadRight(1, ' ')); //自費判斷 Y自費收費 N自費不收費
            dtlData.Append(string.Empty.PadRight(1, ' ')); //管藥判斷庫存是否充足 Y是 N 否
            var amount = prescriptionSendData[index].SendAmount.ToString();
            dtlData.Append(amount.PadRight(10, ' ')); //訂購量
        }

        public static void UpdatePrescriptionStatus(PrescriptionStatus prescriptionStatus, int id)
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
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionSearchSummary]", parameterList);
        }

        public static DataTable GetSearchReservesSummary(List<int> presId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IDList", SetIDTable(presId));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReservePrescriptionSearchSummary]", parameterList);
        }

        public static DataTable GetXmlOfPrescriptionsByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[XmlOfPrescriptionByDate]", parameterList);
        }
       
        public static void UpdateDeclareContent(int id, XDocument declareContent)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "DecFile_Content", new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(declareContent).InnerXml, XmlNodeType.Document, null)));
            DataBaseFunction.AddSqlParameter(parameterList, "id", id);
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateDeclareFileContent]", parameterList);
        }

        public static void UpdatePrescriptionFromDeclareAdjust(List<DeclarePrescription> declarePrescriptions)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "@PointList", SetPrescriptionDeclarePointAdjust(declarePrescriptions));
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdatePrescriptionDeclarePoint]", parameterList);
        }

        #region WepApi

        internal static void UpdateCooperativePrescriptionIsRead(string DeclareId)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty },
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateIsReadByDeclareId", keyValues);
        }

        internal static void UpdateOrthopedicsStatus(string DeclareId)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                    {"DeclareId",DeclareId },
                     {"CusIdNum",string.Empty },
                     {"DeclareXmlDocument",string.Empty }
                };
            HttpMethod httpMethod = new HttpMethod();
            httpMethod.NonQueryPost(@"http://kaokaodepon.singde.com.tw:59091/api/UpdateXmlStatus", keyValues);
        }

        public static List<XmlDocument> GetOrthopedicsPrescriptions(DateTime sDate, DateTime eDate)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                {"pharmcyMedicalNum",ViewModelMainWindow.CurrentPharmacy.ID },
                {"sDate",sDate.ToString("yyyy-MM-dd") },
                {"eDate",eDate.ToString("yyyy-MM-dd") }
            };
            var httpMethod = new HttpMethod();
            var table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByDate", keyValues);
            return table;
        }

        public static List<XmlDocument> GetOrthopedicsPrescriptionsByCusIdNumber(string idNumber)
        {
            Dictionary<string, string> keyValues;
            keyValues = new Dictionary<string, string> {
                {"pharmcyMedicalNum",ViewModelMainWindow.CurrentPharmacy.ID },
                {"cusIdnum",idNumber }
            };
            HttpMethod httpMethod = new HttpMethod();
            List<XmlDocument> table = httpMethod.Get(@"http://kaokaodepon.singde.com.tw:59091/api/GetXmlByMedicalNum", keyValues);
            return table;
        }

        #endregion WepApi

        #region TableSet

        public static DataTable SetPrescriptionMaster(Prescription p)
        {
            DataTable prescriptionMasterTable = PrescriptionMasterTable();
            DataRow newRow = prescriptionMasterTable.NewRow();
            newRow["PreMas_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CustomerID", p.Patient.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DeclareFileID", p.DeclareFileID);
            newRow["PreMas_ImportFileID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustCaseID", p.AdjustCase.ID);
            newRow["PreMas_SerialNumber"] = DBNull.Value;
            newRow["PreMas_PharmacyID"] = ViewModelMainWindow.CurrentPharmacy.ID;
            newRow["PreMas_MakeUpMarkID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PaymentCategoryID", p.PaymentCategory?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalNumber", p.MedicalNumber);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MainDiseaseID", p.MainDisease?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SecondDiseaseID", p.SubDisease?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DivisionID", p.Division?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentDate", p.TreatDate);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentID", p.Copayment?.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ApplyPoint", p.PrescriptionPoint.ApplyPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentPoint", p.PrescriptionPoint.CopaymentPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PaySelfPoint", p.PrescriptionPoint.AmountSelfPay ?? 0);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DepositPoint", p.PrescriptionPoint.Deposit);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TotalPoint", p.PrescriptionPoint.TotalPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_InstitutionID", p.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PrescriptionCaseID", p.PrescriptionCase?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustDate", p.AdjustDate);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DoctorIDNumber", p.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PharmacistIDNumber", p.Pharmacist.IDNumber);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialTreatID", p.SpecialTreat?.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicineDays", p.MedicineDays);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialMaterialPoint", p.PrescriptionPoint.SpecialMaterialPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentPoint", p.PrescriptionPoint.TreatmentPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicinePoint", p.PrescriptionPoint.MedicinePoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicSequence", p.ChronicSeq);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicTotal", p.ChronicTotal);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServiceID", p.MedicalServiceCode);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServicePoint", p.PrescriptionPoint.MedicalServicePoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServiceEstimate", p.PrescriptionPoint.MedicalServicePoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_OldMedicalNumber", p.OriginalMedicalNumber);
            if (string.IsNullOrEmpty(XmlService.ToXmlDocument(p.DeclareContent).InnerXml))
                newRow["PreMas_DeclareContent"] = DBNull.Value;
            else
                newRow["PreMas_DeclareContent"] = new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(p.DeclareContent).InnerXml, XmlNodeType.Document, null));
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsSendToServer", p.PrescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsGetCard", p.PrescriptionStatus.IsGetCard);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeclare", p.PrescriptionStatus.IsDeclare);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeposit", p.PrescriptionStatus.IsDeposit);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsAdjust", p.PrescriptionStatus.IsAdjust);

            DataBaseFunction.AddColumnValue(newRow, "PreMas_OrigTreatmentDT", null);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedIDCode1", p.OrigTreatmentCode);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedIDCode2", null);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CardNo", null);
            prescriptionMasterTable.Rows.Add(newRow);
            return prescriptionMasterTable;
        }

        public static DataTable SetPrescriptionDetail(List<Pdata> prescriptionDetails)
        { //一般藥費
            DataTable prescriptionDetailTable = PrescriptionDetailTable();
            for (var i = 0; i < prescriptionDetails.Count; i++)
            {
                if (prescriptionDetails[i].Order == 0)
                    prescriptionDetails[i].Order = i + 1;
            }
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
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Order", pdata.Order);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_SendAmount", pdata.SendAmount < 0 ? 0 : pdata.SendAmount);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_AdjustNoBuckle", pdata.AdjustNoBuckle);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_IsClosed", pdata.IsClosed);

                prescriptionDetailTable.Rows.Add(newRow);
            }
            return prescriptionDetailTable;
        }

        public static DataTable PrescriptionMasterTable()
        {
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
            masterTable.Columns.Add("PreMas_PaySelfPoint", typeof(int));
            masterTable.Columns.Add("PreMas_DepositPoint", typeof(int));
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
            masterTable.Columns.Add("PreMas_MedicalServiceEstimate", typeof(int));
            masterTable.Columns.Add("PreMas_OldMedicalNumber", typeof(string));
            masterTable.Columns.Add("PreMas_DeclareContent", typeof(SqlXml));
            masterTable.Columns.Add("PreMas_IsSendToServer", typeof(bool));
            masterTable.Columns.Add("PreMas_IsGetCard", typeof(bool));
            masterTable.Columns.Add("PreMas_IsDeclare", typeof(bool));
            masterTable.Columns.Add("PreMas_IsDeposit", typeof(bool));
            masterTable.Columns.Add("PreMas_IsAdjust", typeof(bool));
            masterTable.Columns.Add("PreMas_OrigTreatmentDT", typeof(string));
            masterTable.Columns.Add("PreMas_MedIDCode1", typeof(string));
            masterTable.Columns.Add("PreMas_MedIDCode2", typeof(string));
            masterTable.Columns.Add("PreMas_CardNo", typeof(string));
            return masterTable;
        }

        public static DataTable PrescriptionDetailTable()
        {
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
            detailTable.Columns.Add("PreDet_Order", typeof(int));
            detailTable.Columns.Add("PreDet_SendAmount", typeof(float));
            detailTable.Columns.Add("PreDet_AdjustNoBuckle", typeof(bool));
            detailTable.Columns.Add("PreDet_IsClosed", typeof(bool));
            return detailTable;
        }

        public static DataTable SetReserveMaster(Prescription p)
        {
            DataTable reserveMasterTable = ReserveMasterTable();
            DataRow newRow = reserveMasterTable.NewRow();
            newRow["ResMas_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CustomerID", p.Patient.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DeclareFileID", p.DeclareFileID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_AdjustCaseID", p.AdjustCase.ID);
            newRow["ResMas_SerialNumber"] = DBNull.Value;
            newRow["ResMas_MakeUpMarkID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PaymentCategoryID", p.PaymentCategory?.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalNumber", p.MedicalNumber);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MainDiseaseID", p.MainDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SecondDiseaseID", p.SubDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DivisionID", p.Division.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TreatmentDate", p.TreatDate);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CopaymentID", p.Copayment.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ApplyPoint", p.PrescriptionPoint.ApplyPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CopaymentPoint", p.PrescriptionPoint.CopaymentPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TotalPoint", p.PrescriptionPoint.TotalPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_InstitutionID", p.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PrescriptionCaseID", p.PrescriptionCase.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_AdjustDate", p.AdjustDate);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DoctorIDNumber", p.Institution.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PharmacistIDNumber", p.Pharmacist.IDNumber);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SpecialTreatID", p.SpecialTreat?.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicineDays", p.MedicineDays);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SpecialMaterialPoint", p.PrescriptionPoint.SpecialMaterialPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TreatmentPoint", p.PrescriptionPoint.TreatmentPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicinePoint", p.PrescriptionPoint.MedicinePoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ChronicSequence", p.ChronicSeq);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ChronicTotal", p.ChronicTotal);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalServiceID", p.MedicalServiceCode);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalServicePoint", p.PrescriptionPoint.MedicalServicePoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_OldMedicalNumber", p.OriginalMedicalNumber);
            if (string.IsNullOrEmpty(XmlService.ToXmlDocument(p.DeclareContent).InnerXml))
                newRow["ResMas_DeclareContent"] = DBNull.Value;
            else
                newRow["ResMas_DeclareContent"] = new SqlXml(new XmlTextReader(XmlService.ToXmlDocument(p.DeclareContent).InnerXml, XmlNodeType.Document, null));
            DataBaseFunction.AddColumnValue(newRow, "ResMas_IsSendToServer", p.PrescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_IsRegister", p.PrescriptionStatus.IsRegister);
            reserveMasterTable.Rows.Add(newRow);
            return reserveMasterTable;
        }

        public static DataTable SetReserveDetail(List<Pdata> prescriptionDetails)
        { //一般藥費
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

        public static DataTable SetImportDeclareXmlMaster(List<ImportDeclareXml.ImportDeclareXml.Ddata> Ddatas, List<string> declareFiles, int preId, Customers cs, string fileId)
        {
            DataTable prescriptionMasterTable = PrescriptionMasterTable();
            int filecount = 0;
            foreach (var d in Ddatas)
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
                DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentID", ViewModelMainWindow.GetCopayment(d.D15)?.Id);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_ApplyPoint", d.D16);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentPoint", d.D17);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_PaySelfPoint", 0);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_DepositPoint", 0);
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
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicinePoint", d.D33);
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
                DataBaseFunction.AddColumnValue(newRow, "PreMas_IsAdjust", false);

                prescriptionMasterTable.Rows.Add(newRow);
                preId++;
                filecount++;
            }
            return prescriptionMasterTable;
        }

        public static DataTable SetImportDeclareXmlDetail(List<ImportDeclareXml.ImportDeclareXml.Ddata> Ddatas, int preId)
        {
            DataTable prescriptionDetailTable = PrescriptionDetailTable();
            foreach (var d in Ddatas)
            {
                int count = 1;
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
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_Order", count);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_SendAmount", 0);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_AdjustNoBuckle", false);
                    DataBaseFunction.AddColumnValue(newRow, "PreDet_IsClosed", false);
                    prescriptionDetailTable.Rows.Add(newRow);
                    count++;
                }
                preId++;
            }
            return prescriptionDetailTable;
        }

        public static DataTable ReserveMasterTable()
        {
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

        public static DataTable ReserveDetailTable()
        {
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

        private static DataTable InstitutionIDTable()
        {
            DataTable idTable = new DataTable();
            idTable.Columns.Add("InstitutionID", typeof(string));
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

        public static DataTable SetStringIDTable(List<string> IDList)
        {
            var table = InstitutionIDTable();
            foreach (var id in IDList)
            {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "InstitutionID", id);
                table.Rows.Add(newRow);
            }
            return table;
        }

        public static DataTable PrescriptionDeclarePointAdjustTable()
        {
            DataTable detailTable = new DataTable();
            detailTable.Columns.Add("PreMas_ID", typeof(int));
            detailTable.Columns.Add("PreMas_SerialNumber", typeof(int));
            detailTable.Columns.Add("PreMas_PharmacistIDNumber", typeof(string));
            detailTable.Columns.Add("PreMas_ApplyPoint", typeof(int));
            detailTable.Columns.Add("PreMas_TotalPoint", typeof(int));
            detailTable.Columns.Add("PreMas_MedicalServicePoint", typeof(int));
            detailTable.Columns.Add("PreMas_DeclareContent", typeof(SqlXml));
            detailTable.Columns.Add("PreMas_MedicalServiceID", typeof(string));
            detailTable.Columns.Add("PreMas_IsDeclare", typeof(bool));
            return detailTable;
        }

        public static DataTable SetPrescriptionDeclarePointAdjust(List<DeclarePrescription> declarePrescriptions)
        {
            DataTable table = PrescriptionDeclarePointAdjustTable();
            foreach (var d in declarePrescriptions)
            {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "PreMas_ID", d.ID);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_SerialNumber", d.SerialNumber);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_PharmacistIDNumber", d.Pharmacist.IDNumber);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_ApplyPoint", d.ApplyPoint);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_TotalPoint", d.TotalPoint);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServicePoint", d.MedicalServicePoint);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_DeclareContent", d.DeclareContent);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServiceID", d.MedicalServiceID);
                DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeclare", d.IsDeclare);
                table.Rows.Add(newRow);
            }
            return table;
        }

        #endregion TableSet

        public static DataTable GetNotAdjustPrescriptionCount(DateTime start, DateTime end, string pharmacyId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", start);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", end);
            DataBaseFunction.AddSqlParameter(parameterList, "pharmacyID", pharmacyId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[NotAdjustCount]", parameterList);
        }

        public static DataTable DuplicatePrescriptions(DateTime startDate, DateTime endDate)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", startDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionSameDeclare]", parameterList);
        }

        public static DataTable CheckSameDeclarePrescription(Prescription current)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusID", current.Patient.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "TreatDate", current.TreatDate);
            DataBaseFunction.AddSqlParameter(parameterList, "Institution", current.Institution.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "Division", current.Division.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "AdjustCase", current.AdjustCase.ID);
            if (current.ChronicTotal is null)
            {
                DataBaseFunction.AddSqlParameter(parameterList, "ChronicTotal", DBNull.Value);
                DataBaseFunction.AddSqlParameter(parameterList, "ChronicSeq", DBNull.Value);
            }
            else
            {
                DataBaseFunction.AddSqlParameter(parameterList, "ChronicTotal", current.ChronicTotal);
                DataBaseFunction.AddSqlParameter(parameterList, "ChronicSeq", current.ChronicSeq);
            }
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CountSameDeclarePrescription]", parameterList);
        }

        public static DataTable GetEditedRecords(string prescriptionID)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionID", prescriptionID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionEditRecord]", parameterList);
        }

        public static DataTable GetOrderByPrescriptionID(int preID)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionID", preID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionOrderByPrescriptionID]", parameterList);
        }

        public static DataTable GetReserveByPrescription(Prescription p)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "CusID", p.Patient.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "InstitutionID", p.Institution.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "DivisionID", p.Division.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "MainDiseaseID", p.MainDisease.ID);
            DataBaseFunction.AddSqlParameter(parameterList, "TreatDate", p.TreatDate);
            DataBaseFunction.AddSqlParameter(parameterList, "ChronicSequence", p.ChronicSeq);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ReserveByPrescription]", parameterList);
        }

        public static void InsertPrescriptionPointEditRecord(int preID, int medicalServiceDiff, int medicineDiff, int paySelfDiff)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PreMasID", preID);
            DataBaseFunction.AddSqlParameter(parameterList, "MedicalServiceDiff", medicalServiceDiff);
            DataBaseFunction.AddSqlParameter(parameterList, "MedicineDiff", medicineDiff);
            DataBaseFunction.AddSqlParameter(parameterList, "PaySelfDiff", paySelfDiff);
            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescriptionPointEditRecord]", parameterList);
        }

        //寫入StoOrdPrescriptionID於[StoreOrder].[Master]
        internal static void InsertStoOrdPrescriptionID(int iD)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StoOrd_PrescriptionID", iD);

            MainWindow.ServerConnection.ExecuteProc("[Set].[InsertStoOrdPrescriptionID]", parameterList);
        }

        internal static void DeleteStoreOrder(int iD)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "StoOrd_PrescriptionID", iD);

            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStoreOrderByPrescriptionID]", parameterList);
        }

        public static void UploadData2(Prescription p, IcDataUploadService.Rec rec1)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                string sql = string.Empty;
                sql = string.Format("Update [{0}].[HIS].[PrescriptionMaster] Set [PreMas_OrigTreatmentDT] = '{1}', [PreMas_MedIDCode1] = '{2}', [PreMas_MedIDCode2] = '{3}', [PreMas_CardNo] = '{4}', [PreMas_ModifyEmpID] = '{5}', [PreMas_ModifyTime] = GETDATE(), PreMas_SecuritySign = '{6}' Where [PreMas_ID] = {7} \n", Properties.Settings.Default.SystemSerialNumber, p.OrigTreatmentDT, p.OrigTreatmentCode, p.TreatmentCode, p.Card.CardNumber, ViewModelMainWindow.CurrentUser.ID, p.Card.MedicalNumberData.SecuritySignature, p.ID);
                
                if (p.PrescriptionSign != null)
                {
                    for (int i = 0; i < p.Medicines.Count; i++)
                    {
                        sql += string.Format("Update [{0}].[HIS].[PrescriptionDetail] Set PreDet_SecuritySign = '{1}' Where PreDet_PrescriptionID = {2} And PreDet_Number = {3} \n", Properties.Settings.Default.SystemSerialNumber,
                                   p.PrescriptionSign[i],
                                   p.ID,
                                   i + 1
                                   );
                    }
                }
                
                
                
                SQLServerConnection.DapperQuery((conn) =>
                {
                    _ = conn.Query<int>(sql, commandType: CommandType.Text);
                });
                scope.Complete();
            }
                
        }
        public static string GetSignXml(int id)
        {
            string result = string.Empty;
            string sql = string.Format("Select UplData_Content From {0}.His.UploadData Where UplData_PrescriptionID = {1}", 
                Properties.Settings.Default.SystemSerialNumber, id);
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.QueryFirst<string>(sql, commandType: CommandType.Text);
            });
            return result;
        }

        public static DataTable GetDuplicateExport(DateTime sdate, DateTime edate, string pharmacyID)
        {
            DataTable result = null;

            string sql = string.Format(
                @"DECLARE @sDate date,@eDate date,@pharmacyID nvarchar(15),@CooFlag int	
                    SET @pharmacyID = '{1}'
                    SET @sDate = '{2}'
                    SET @eDate = '{3}'
                    
                    
                SELECT @CooFlag = COUNT(*)
                FROM [{0}].[HIS].[CooperativeClinic]
                WHERE [CooCli_ClinicType] = 1 /*合作診所:骨科*/
                    AND (@sDate BETWEEN CooCli_StartDate AND Isnull(CooCli_EndDate, GETDATE())
                    OR @eDate BETWEEN CooCli_StartDate AND Isnull(CooCli_EndDate, GETDATE()) );

                SELECT PreMas_PharmacyID	/* 藥局機構代號 */
	                ,PreMas_ID			/* 處方單號 */
                    ,Cus_Name				/* 病患姓名 */
	                ,PreMas_InstitutionID	/* 釋出院所機構代號 */
	                ,Ins_Name				/* 釋出院所 */
	                ,PreMas_DivisionID	/* 科別代號 */
	                ,Div_Name				/* 科別 */
	                ,PreMas_AdjustDate	/* 調劑日期 */
	                ,Emp_Name				/* 調劑藥師 */
	                ,PreMas_MedicinePoint	/* 藥品點數 */
                    ,PreMas_SpecialMaterialPoint	/* 特材點數 */
	                ,PreMas_MedicalServicePoint	/* 藥服費 */
	                ,PreMas_TotalPoint	/* 總點數 */
	                ,PreMas_AdjustCaseID	/* 案件代號 */
	                ,Adj_Name				/* 調劑案件 */
	                ,PreMas_ApplyPoint	/* 申請點數 */
	                ,PreMas_CopaymentPoint/* 部分負擔 */
	                ,PreMas_InsertTime	/* 調劑時間 */
                INTO #tempDeclarePres
                FROM [{0}].[HIS].[PrescriptionMaster] p
                INNER JOIN [{0}].[Customer].[Master] c ON p.PreMas_CustomerID = c.Cus_ID
                INNER JOIN HIS_POS_Server.DataSource.Institution i ON p.PreMas_InstitutionID = i.Ins_ID
                INNER JOIN HIS_POS_Server.DataSource.Division d ON p.PreMas_DivisionID = d.Div_ID
                INNER JOIN [{0}].[Employee].[Master] e ON p.PreMas_PharmacistIDNumber = e.Emp_IDNumber
                INNER JOIN HIS_POS_Server.DataSource.AdjustCase a ON  p.PreMas_AdjustCaseID = a.Adj_ID
                WHERE p.PreMas_AdjustDate Between @sDate and @eDate
                AND p.PreMas_IsDeposit = 0
                AND p.PreMas_AdjustCaseID <> '0'
                AND p.PreMas_EnableStatus <> 0
                AND p.PreMas_IsAdjust = 1
                AND p.PreMas_PharmacyID = @pharmacyID

                IF ISNULL(@CooFlag,0) > 0
                BEGIN
	                SELECT p.*,CASE WHEN ISNULL(CashFlow_SourceID,'') <> '' THEN 'Y' ELSE 'N' END AS CooVIP /* 合作診所員眷註記 */
	                FROM #tempDeclarePres p
	                LEFT JOIN (	SELECT CashFlow_SourceID
				                FROM [{0}].[Report].[CashFlow]
				                WHERE CashFlow_Source = 'PreMasId'
					            AND CashFlow_IsEnable = 1
					            AND CashFlow_Name IN ('部分負擔','部分負擔免收')
					            AND CashFlow_SourceID IN (SELECT PreMas_ID FROM #tempDeclarePres)
				    GROUP BY CashFlow_SourceID
				    HAVING SUM(CashFlow_Value) = 0
	            ) r ON cast(p.PreMas_ID as nvarchar(15)) = r.CashFlow_SourceID
	            ORDER BY PreMas_AdjustDate,PreMas_InsertTime
                END
                ELSE
                BEGIN
	                SELECT *
	                FROM #tempDeclarePres
	                ORDER BY PreMas_AdjustDate,PreMas_InsertTime
                END

                DROP TABLE #tempDeclarePres
                ", Properties.Settings.Default.SystemSerialNumber, pharmacyID, sdate.ToString("yyyy-MM-dd"), edate.ToString("yyyy-MM-dd"));

            SQLServerConnection.DapperQuery((conn) =>
            {
                var dapper = conn.Query(sql, commandType: CommandType.Text);
                string json = JsonConvert.SerializeObject(dapper);//序列化成JSON
                result = JsonConvert.DeserializeObject<DataTable>(json);//反序列化成DataTable
            });
            return result;
        }
    }
}