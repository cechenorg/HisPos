using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Person;

namespace His_Pos.PrescriptionInquire
{
    public class PrescriptionOverview
    {
        public PrescriptionOverview(DataRow row) {
            PatientName = row["CUS_NAME"].ToString();
            HospitalName = row["INS_NAME"].ToString();
            MedicalPersonnelName = row["EMP_NAME"].ToString();
            ChronicStatus = row["CHRONIC_STATUS"].ToString();
        }
        public string Decmas_Id { get; set; }
        public string AdjustCaseName { get; set; }
        public string PatientName { get; set; }
        public string HospitalName { get; set; }
        public string DivisionName { get; set; }
        public string MedicalPersonnelName { get; set; }
        public DateTime AdjustDate { get; set; }
        public string ChronicStatus { get; set; }
        public string Point { get; set; }

    }
}
