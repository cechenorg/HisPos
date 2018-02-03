using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class MedicalInfo
    {
        public MedicalInfo()
        {
        }

        public MedicalInfo(Hospital hospital, SpecialCode specialCode, TreatmentCase.TreatmentCase treatmentCase)
        {
            Hospital = hospital;
            Hospital.Division = hospital.Division;
            Hospital.Doctor = hospital.Doctor;
            SpecialCode = specialCode;
            TreatmentCase = treatmentCase;
        }

        public Hospital Hospital { get; set; }//d21 原處方服務機構代號 d24 診治醫師代號 d13 就醫科別
        public SpecialCode SpecialCode { get; set ; }//d26 原處方服務機構之特定治療項目代號
        public List<DiseaseCode> DiseaseCodes { get; set; } = new List<DiseaseCode>();//d8 d9 國際疾病分類碼
        public TreatmentCase.TreatmentCase TreatmentCase { get; set; }//d22 原處方服務機構之案件分類
    }
}
