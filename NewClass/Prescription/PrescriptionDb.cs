using His_Pos.Database;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Prescription.DeclareFile;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach; 
using System.Xml.Linq;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Institution;

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
        public static int InsertReserve(Prescription prescription, List<Pdata> prescriptionDetails) { 
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ResMaster", SetReserveMaster(prescription));
            DataBaseFunction.AddSqlParameter(parameterList, "ResDetail", SetReserveionDetail(prescription, prescriptionDetails));
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertReserve]", parameterList);
            return Convert.ToInt32(table.Rows[0]["DecMasId"].ToString());
        }
        public static void DeleteReserve(string recMasId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "@ecMas_Id", recMasId); 
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteReserve]", parameterList);  
        }
        public static void PredictResere(string recMasId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ReserveId", recMasId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[PredictResere]", parameterList);
        } 
        
        public static void ProcessInventory(string productID,double amount)
        {
            
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

        public static XmlDocument ToXmlDocument(XDocument xDocument) {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
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
        public static DataTable GetSearchPrescriptionsData(DateTime? sDate, DateTime? eDate, string patient, AdjustCase adj, Institution ins, MedicalPersonnel pharmacist)
        {
            var table = new DataTable();
            return table;
        }
        #endregion
        #region TableSet
        public static DataTable SetPrescriptionMaster(Prescription p) {
            DataTable prescriptionMasterTable = PrescriptionMasterTable();
            DataRow newRow = prescriptionMasterTable.NewRow();
            newRow["PreMas_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CustomerID", p.Patient.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DeclareFileID", p.DeclareFileID);
            newRow["PreMas_ImportFileID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustCaseID", p.Treatment.AdjustCase.Id);
            newRow["PreMas_SerialNumber"] = DBNull.Value;
            newRow["PreMas_PharmacyID"] = ViewModelMainWindow.CurrentPharmacy.Id;
            newRow["PreMas_MakeUpMarkID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PaymentCategoryID", p.Treatment.PaymentCategory.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalNumber", p.Treatment.MedicalNumber);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MainDiseaseID", p.Treatment.MainDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SecondDiseaseID", p.Treatment.SubDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DivisionID", p.Treatment.Division.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentDate", p.Treatment.TreatDate);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentID", p.Treatment.Copayment.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ApplyPoint", p.PrescriptionPoint.ApplyPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CopaymentPoint", p.PrescriptionPoint.CopaymentPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TotalPoint", p.PrescriptionPoint.TotalPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_InstitutionID", p.Treatment.Institution.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PrescriptionCaseID", p.Treatment.PrescriptionCase.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustDate", p.Treatment.AdjustDate);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DoctorIDNumber", p.Treatment.Institution.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PharmacistIDNumber", p.Treatment.Pharmacist.IdNumber);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialTreatID", p.Treatment.SpecialTreat.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicineDays", p.MedicineDays);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SpecialMaterialPoint", p.PrescriptionPoint.SpecialMaterialPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_TreatmentPoint", p.PrescriptionPoint.TreatmentPoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicinePoint", p.PrescriptionPoint.MedicinePoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicSequence", p.Treatment.ChronicSeq);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_ChronicTotal", p.Treatment.ChronicTotal);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServiceID", p.MedicalServiceID);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalServicePoint", p.PrescriptionPoint.MedicalServicePoint);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_OldMedicalNumber", p.Treatment.OriginalMedicalNumber);
            if (string.IsNullOrEmpty(ToXmlDocument(p.DeclareContent).InnerXml))
                newRow["PreMas_DeclareContent"] = DBNull.Value;
            else
                newRow["PreMas_DeclareContent"] = new SqlXml(new XmlTextReader(ToXmlDocument(p.DeclareContent).InnerXml, XmlNodeType.Document, null));
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsSendToServer", p.PrescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsGetCard", p.PrescriptionStatus.IsGetCard);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeclare", p.PrescriptionStatus.IsDeclare);
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
            masterTable.Columns.Add("PreMas_AdjustCaseID", typeof(String));
            masterTable.Columns.Add("PreMas_SerialNumber", typeof(int));
            masterTable.Columns.Add("PreMas_PharmacyID", typeof(string));
            masterTable.Columns.Add("PreMas_MakeUpMarkID", typeof(String));
            masterTable.Columns.Add("PreMas_PaymentCategoryID", typeof(String));
            masterTable.Columns.Add("PreMas_MedicalNumber", typeof(String));
            masterTable.Columns.Add("PreMas_MainDiseaseID", typeof(String));
            masterTable.Columns.Add("PreMas_SecondDiseaseID", typeof(String));
            masterTable.Columns.Add("PreMas_DivisionID", typeof(String));
            masterTable.Columns.Add("PreMas_TreatmentDate", typeof(DateTime));
            masterTable.Columns.Add("PreMas_CopaymentID", typeof(String));
            masterTable.Columns.Add("PreMas_ApplyPoint", typeof(int));
            masterTable.Columns.Add("PreMas_CopaymentPoint", typeof(int));
            masterTable.Columns.Add("PreMas_TotalPoint", typeof(int));
            masterTable.Columns.Add("PreMas_InstitutionID", typeof(String));
            masterTable.Columns.Add("PreMas_PrescriptionCaseID", typeof(String));
            masterTable.Columns.Add("PreMas_AdjustDate", typeof(DateTime));
            masterTable.Columns.Add("PreMas_DoctorIDNumber", typeof(String));
            masterTable.Columns.Add("PreMas_PharmacistIDNumber", typeof(String));
            masterTable.Columns.Add("PreMas_SpecialTreatID", typeof(String));
            masterTable.Columns.Add("PreMas_MedicineDays", typeof(int));
            masterTable.Columns.Add("PreMas_SpecialMaterialPoint", typeof(int));
            masterTable.Columns.Add("PreMas_TreatmentPoint", typeof(int));
            masterTable.Columns.Add("PreMas_MedicinePoint", typeof(int));
            masterTable.Columns.Add("PreMas_ChronicSequence", typeof(int));
            masterTable.Columns.Add("PreMas_ChronicTotal", typeof(int));
            masterTable.Columns.Add("PreMas_MedicalServiceID", typeof(String));
            masterTable.Columns.Add("PreMas_MedicalServicePoint", typeof(int));
            masterTable.Columns.Add("PreMas_OldMedicalNumber", typeof(String));
            masterTable.Columns.Add("PreMas_DeclareContent", typeof(SqlXml));
            masterTable.Columns.Add("PreMas_IsSendToServer", typeof(bool));
            masterTable.Columns.Add("PreMas_IsGetCard", typeof(bool));
            masterTable.Columns.Add("PreMas_IsDeclare", typeof(bool));
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
            return detailTable;
        }
        public static DataTable SetReserveMaster(Prescription p) {
            DataTable reserveMasterTable = ReserveMasterTable();
            DataRow newRow = reserveMasterTable.NewRow();
            newRow["ResMas_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CustomerID", p.Patient.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DeclareFileID", p.DeclareFileID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_AdjustCaseID", p.Treatment.AdjustCase.Id);
            newRow["ResMas_SerialNumber"] = DBNull.Value;
            newRow["ResMas_MakeUpMarkID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PaymentCategoryID", p.Treatment.PaymentCategory.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalNumber", p.Treatment.MedicalNumber); 
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MainDiseaseID", p.Treatment.MainDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SecondDiseaseID", p.Treatment.SubDisease.ID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DivisionID", p.Treatment.Division.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TreatmentDate", p.Treatment.TreatDate);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CopaymentID", p.Treatment.Copayment.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ApplyPoint", p.PrescriptionPoint.ApplyPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_CopaymentPoint", p.PrescriptionPoint.CopaymentPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TotalPoint", p.PrescriptionPoint.TotalPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_InstitutionID", p.Treatment.Institution.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PrescriptionCaseID", p.Treatment.PrescriptionCase.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_AdjustDate", p.Treatment.AdjustDate);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_DoctorIDNumber", p.Treatment.Institution.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_PharmacistIDNumber", p.Treatment.Pharmacist.IdNumber);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SpecialTreatID", p.Treatment.SpecialTreat.Id);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicineDays", p.MedicineDays);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_SpecialMaterialPoint", p.PrescriptionPoint.SpecialMaterialPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_TreatmentPoint", p.PrescriptionPoint.TreatmentPoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicinePoint", p.PrescriptionPoint.MedicinePoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ChronicSequence", p.Treatment.ChronicSeq);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_ChronicTotal", p.Treatment.ChronicTotal);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalServiceID", p.MedicalServiceID);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_MedicalServicePoint", p.PrescriptionPoint.MedicalServicePoint);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_OldMedicalNumber", p.Treatment.OriginalMedicalNumber);
            if (string.IsNullOrEmpty(ToXmlDocument(p.DeclareContent).InnerXml))
                newRow["ResMas_DeclareContent"] = DBNull.Value;
            else
                newRow["ResMas_DeclareContent"] = new SqlXml(new XmlTextReader(ToXmlDocument(p.DeclareContent).InnerXml, XmlNodeType.Document, null));
            DataBaseFunction.AddColumnValue(newRow, "ResMas_IsSendToServer", p.PrescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddColumnValue(newRow, "ResMas_IsRegister", p.PrescriptionStatus.IsRegister);
            reserveMasterTable.Rows.Add(newRow);
            return reserveMasterTable;
        }
        public static DataTable SetReserveionDetail(Prescription p, List<Pdata> prescriptionDetails) { //一般藥費
            int medCount = 1;
            DataTable reserveDetailTable = ReserveDetailTable();

            foreach (var pdata in prescriptionDetails)
            {
                DataRow newRow = reserveDetailTable.NewRow();
                newRow["ResDet_ReserveID"] = DBNull.Value;
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
                reserveDetailTable.Rows.Add(newRow); 
            } 
            return reserveDetailTable;
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
            detailTable.Columns.Add("ResDet_Usage", typeof(int));
            detailTable.Columns.Add("ResDet_Position", typeof(string));
            detailTable.Columns.Add("ResDet_Percentage", typeof(int));
            detailTable.Columns.Add("ResDet_TotalAmount", typeof(float));
            detailTable.Columns.Add("ResDet_Price", typeof(double));
            detailTable.Columns.Add("ResDet_Point", typeof(int));
            detailTable.Columns.Add("ResDet_SerialNumber", typeof(int));
            detailTable.Columns.Add("ResDet_MedicineDays", typeof(int));
            detailTable.Columns.Add("ResDet_PaySelf", typeof(bool));
            return detailTable;
        }
        #endregion

        public static DataTable GetReservePrescriptionsData()
        {
            var table = new DataTable();
            return table;
        }
    }
}
