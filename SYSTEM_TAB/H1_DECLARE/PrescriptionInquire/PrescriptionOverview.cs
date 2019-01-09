using System;
using System.Data;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionInquire
{
    public class PrescriptionOverview
    {
        public PrescriptionOverview(DataRow row) {
            Decmas_Id = row["HISDECMAS_ID"].ToString();
            Division = new Division(row);
            PatientName = row["CUS_NAME"].ToString();
            HospitalName = row["INS_NAME"].ToString();
            MedicalPersonnelName = row["EMP_NAME"].ToString();
            ChronicStatus = row["CHRONIC_STATUS"].ToString();
            AdjustDate = Convert.ToDateTime(row["HISDECMAS_TREATDATE"]).AddYears(-1911).ToString("yyy/MM/dd");
            Point = row["HISDECMAS_POINT"].ToString();
            MedDeclarePoint = Double.Parse(row["HISDECMAS_DRUGPOINT"].ToString());
            MedServicePrice = Double.Parse(row["HISDECMAS_SERVICEPOINT"].ToString());
            MedUseePrice = Double.Parse(string.IsNullOrEmpty(row["MEDUSE_VALUE"].ToString()) ? "0" : row["MEDUSE_VALUE"].ToString());
            CopaymentPrice = Double.Parse(row["HISDECMAS_COPAYMENTPOINT"].ToString());
            Profit = Double.Parse(Point) + MedUseePrice;
        }
        public PrescriptionOverview(DeclareData declareData) {
            Decmas_Id = declareData.DecMasId;
            AdjustCaseName = declareData.Prescription.Treatment.AdjustCase.FullName;
            PatientName = declareData.Prescription.Customer.Name;
            HospitalName = declareData.Prescription.Treatment.MedicalInfo.Hospital.Name;
            Division = declareData.Prescription.Treatment.MedicalInfo.Hospital.Division;
            MedicalPersonnelName = declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.Name;
            AdjustDate = declareData.Prescription.Treatment.AdjustDate.ToShortTimeString();
            Point = declareData.D18TotalPoint.ToString();
            ChronicStatus =  declareData.Prescription.ChronicTotal + "/" + declareData.Prescription.ChronicSequence ;
        }
        public string Decmas_Id { get; set; }
        public string AdjustCaseName { get; set; }

        public string PatientName { get; set; }
        public string HospitalName { get; set; }
        public Division Division { get; set; }
        public string MedicalPersonnelName { get; set; }
        public string AdjustDate { get; set; }
        public string ChronicStatus { get; set; }
        public string Point { get; set; }
        public double MedDeclarePoint { get; set; }
        public double MedServicePrice { get; set; }
        public double MedUseePrice { get; set; }
        public double CopaymentPrice { get; set; }
        public double Profit { get; set; }
        public bool IsPredictChronic { get; set; }
        
    }
}
