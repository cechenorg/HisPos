using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace His_Pos.Class
{
    public class MedicalInfo
    {
        public MedicalInfo()
        {
            Hospital = new Hospital();
            SpecialCode = new SpecialCode();
            DiseaseCodes = new ObservableCollection<DiseaseCode>();
            TreatmentCase = new TreatmentCase.TreatmentCase();
        }

        public MedicalInfo(Hospital hospital, SpecialCode specialCode, ObservableCollection<DiseaseCode> diseaseCodes, TreatmentCase.TreatmentCase treatmentCase)
        {
            Hospital = hospital;
            SpecialCode = specialCode;
            DiseaseCodes = diseaseCodes;
            TreatmentCase = treatmentCase;
        }

        public Hospital Hospital { get; set; }//d21 原處方服務機構代號 d24 診治醫師代號 d13 就醫科別
        public SpecialCode SpecialCode { get; set ; }//d26 原處方服務機構之特定治療項目代號
        public ObservableCollection<DiseaseCode> DiseaseCodes { get; set; } //d8 d9 國際疾病分類碼
        public TreatmentCase.TreatmentCase TreatmentCase { get; set; }//d22 原處方服務機構之案件分類
    }
}
