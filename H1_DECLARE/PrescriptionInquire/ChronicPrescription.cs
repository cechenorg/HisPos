using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.PrescriptionDec
{
    public class ChronicPrescription
    {
        public ChronicPrescription(string chronicId,string hisDeclareMasterId,string patientIdNum, string hisDivsionId, string prescriptionDate,
            int count, string institutionName, string hisDivisionName)
        {
            ChronicId = chronicId;
            HisDeclareMasterId = hisDeclareMasterId;
            PatientIdNum = patientIdNum;
            HisDivsionId = hisDivsionId;
            PrescriptionDate = prescriptionDate;
            Count = count;
            InstitutionName = institutionName;
            HisDivisionName = hisDivisionName;
        }

        public string ChronicId { get; set; }
        public string HisDeclareMasterId { get; set; }
        public string PatientIdNum { get; set; }
        public string HisDivsionId { get; set; }
        public string PrescriptionDate { get; set; }
        public int Count { get; set; }
        public string InstitutionName { get; set; }
        public string HisDivisionName { get; set; }
    }
}
