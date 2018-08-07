using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;

namespace His_Pos.PrescriptionInquire
{
    public class PrescriptionOverview
    {
        public PrescriptionOverview(DataRow row) {
            Decmas_Id = row["HISDECMAS_ID"].ToString();
            DivisionName = row["HISDIV_FULLNAME"].ToString();
            PatientName = row["CUS_NAME"].ToString();
            HospitalName = row["INS_NAME"].ToString();
            MedicalPersonnelName = row["EMP_NAME"].ToString();
            ChronicStatus = row["CHRONIC_STATUS"].ToString();
            AdjustDate = Convert.ToDateTime(row["HISDECMAS_TREATDATE"]).ToString("yyyy/MM/dd");
        }
        public PrescriptionOverview(DeclareData declareData) {
            Decmas_Id = declareData.DecMasId;
            AdjustCaseName = declareData.Prescription.Treatment.AdjustCase.FullName;
            PatientName = declareData.Prescription.Customer.Name;
            HospitalName = declareData.Prescription.Treatment.MedicalInfo.Hospital.Name;
            DivisionName = declareData.Prescription.Treatment.MedicalInfo.Hospital.Division.FullName;
            MedicalPersonnelName = declareData.Prescription.Treatment.MedicalInfo.Hospital.Doctor.Name;
            AdjustDate = declareData.Prescription.Treatment.AdjustDate.ToShortTimeString();

        }
        public string Decmas_Id { get; set; }
        public string AdjustCaseName { get; set; }

        public string PatientName { get; set; }
        public string HospitalName { get; set; }
        public string DivisionName { get; set; }
        public string MedicalPersonnelName { get; set; }
        public string AdjustDate { get; set; }
        public string ChronicStatus { get; set; }
        public string Point { get; set; }

    }
}
