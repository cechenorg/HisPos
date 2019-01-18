using His_Pos.Database;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace His_Pos.NewClass.Prescription
{
    public static class PrescriptionDb
    {
        public static Prescriptions GetCooperaPrescriptionsDataByCusIdNumber(string pharmcyMedicalNum, string cusIdnum)
        {
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
        public static Prescriptions GetCooperaPrescriptionsDataByDate(string pharmcyMedicalNum, DateTime sDate, DateTime eDate)
        {
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
        public static int InsertPrescription(Prescription prescription) { 
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionMaster", SetPrescriptionMaster(prescription));
            DataBaseFunction.AddSqlParameter(parameterList, "PrescriptionDetail", SetPrescriptionDetail(prescription)); 
            var table = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertPrescription]", parameterList);
            return Convert.ToInt32(table.Rows[0]["DecMasId"].ToString()); 
        }
        public static DataTable SetPrescriptionMaster(Prescription p) {
            DataTable prescriptionMasterTable = PrescriptionMasterTable();
            DataRow newRow = prescriptionMasterTable.NewRow(); 
            newRow["PreMas_ID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_CustomerID", p.Patient.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DeclareFileID", p.DeclareFileID); 
            newRow["PreMas_ImportFileID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_AdjustCaseID", p.Treatment.AdjustCase.Id); 
            newRow["PreMas_SerialNumber"] = DBNull.Value;
            newRow["PreMas_MakeUpMarkID"] = DBNull.Value;
            DataBaseFunction.AddColumnValue(newRow, "PreMas_PaymentCategoryID", p.Treatment.PaymentCategory.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MedicalNumber", p.Treatment.MedicalNumber);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_MainDiseaseID", p.Treatment.MainDisease.Id);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_SecondDiseaseID", p.Treatment.SubDisease.Id);
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
            DataBaseFunction.AddColumnValue(newRow, "PreMas_DeclareContent", p.DeclareContent);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsSendToServer", p.PrescriptionStatus.IsSendToSingde);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsGetCard", p.PrescriptionStatus.IsGetCard);
            DataBaseFunction.AddColumnValue(newRow, "PreMas_IsDeclare", p.PrescriptionStatus.IsDeclare); 
            prescriptionMasterTable.Rows.Add(newRow); 
            return prescriptionMasterTable;
        }
        public static DataTable SetPrescriptionDetail(Prescription p) { //一般藥費
            int medCount = 1;
            DataTable prescriptionMasterTable = PrescriptionMasterTable();
             
            foreach (var med in p.Medicines) {
                DataRow newRow = prescriptionMasterTable.NewRow();
                newRow["PreDet_PrescriptionID"] = DBNull.Value; 
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicalOrderID", 1); 
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Percentage", 100); //還沒算
                DataBaseFunction.AddColumnValue(newRow, "PreDet_SerialNumber", med.PaySelf ? 0 : medCount); 
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Point",med.PaySelf ? 0: med.NHIPrice * med.Amount); 
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineID",med.ID);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Dosage", med.Dosage);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Usage", med.Usage.Name);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Position", med.Position.Name); 
                DataBaseFunction.AddColumnValue(newRow, "PreDet_TotalAmount", med.Amount); 
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Price", med.PaySelf ? med.Price : med.NHIPrice); 
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineDays", med.Days);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_PaySelf", med.PaySelf);
                prescriptionMasterTable.Rows.Add(newRow);
                if (!med.PaySelf)
                    medCount++;
            }
            //這裡要補藥事服務費
            return prescriptionMasterTable;
        }
        public static DataTable SetPrescriptionDetailSimpleForm(Prescription p) { //日記藥費
            int medCount = 1;
            DataTable prescriptionMasterTable = PrescriptionMasterTable();

            foreach (var med in p.Medicines)
            {
                DataRow newRow = prescriptionMasterTable.NewRow();
                newRow["PreDet_PrescriptionID"] = DBNull.Value;
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicalOrderID", 1);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Percentage", 100); //還沒算
                DataBaseFunction.AddColumnValue(newRow, "PreDet_SerialNumber", med.PaySelf ? 0 : medCount);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Point", med.PaySelf ? 0 : med.NHIPrice * med.Amount);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineID", med.ID);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Dosage", med.Dosage);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Usage", med.Usage.Name);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Position", med.Position.Name);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_TotalAmount", med.Amount);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_Price", med.PaySelf ? med.Price : med.NHIPrice);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_MedicineDays", med.Days);
                DataBaseFunction.AddColumnValue(newRow, "PreDet_PaySelf", med.PaySelf);
                prescriptionMasterTable.Rows.Add(newRow);
                if (!med.PaySelf)
                    medCount++;
            }
            //這裡要補藥事服務費
            return prescriptionMasterTable;
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
            masterTable.Columns.Add("PreMas_DeclareContent", typeof(XmlDocument));
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

        public static void ProcessInventory(string productID,double amount)
        {

        }
        public static void ProcessCashFlow(int prescriptionID)
        {

        }
        public static void ProcessEntry(int prescriptionID,double consumption, double total)
        {

        }
    }
}
