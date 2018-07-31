using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;

namespace His_Pos.Class
{
    public class MedicalInfo
    {
        public MedicalInfo()
        {
            Hospital = new Hospital();
            SpecialCode = new SpecialCode();
            MainDiseaseCode = new DiseaseCode();
            SecondDiseaseCode = new DiseaseCode();
            TreatmentCase = new TreatmentCase.TreatmentCase();
        }

        public MedicalInfo(DataRow row)
        {
            Hospital = new Hospital(row,DataSource.GetHospitalData);
            SpecialCode = new SpecialCode();
            MainDiseaseCode = new DiseaseCode();
            SecondDiseaseCode = new DiseaseCode();
            TreatmentCase = new TreatmentCase.TreatmentCase(row);
            MainDiseaseCode =  new DiseaseCode { Id = row["HISINT_ID1"].ToString() };
            SecondDiseaseCode = new DiseaseCode { Id = row["HISINT_ID2"].ToString() };
        }

        public MedicalInfo(Hospital hospital, SpecialCode specialCode, List<DiseaseCode> diseaseCodes, TreatmentCase.TreatmentCase treatmentCase)
        {
            Hospital = hospital;
            SpecialCode = specialCode;
            TreatmentCase = treatmentCase;
            switch (diseaseCodes.Count)
            {
                case 1:
                    MainDiseaseCode = diseaseCodes[0];
                    break;
                case 2:
                    MainDiseaseCode = diseaseCodes[0];
                    SecondDiseaseCode = diseaseCodes[1];
                    break;
            }
        }
        public MedicalInfo(XmlNode xml) {
            Hospital = new Hospital(xml);
            SpecialCode = new SpecialCode(xml);
            MainDiseaseCode = new DiseaseCode();
            SecondDiseaseCode = new DiseaseCode();
            MainDiseaseCode.Id = xml.SelectSingleNode("d8") == null ? null : xml.SelectSingleNode("d8").InnerText;
            SecondDiseaseCode.Id = xml.SelectSingleNode("d9") == null ? null : xml.SelectSingleNode("d9").InnerText;
            TreatmentCase = new TreatmentCase.TreatmentCase(xml);
        }
        public Hospital Hospital { get; set; }//d21 原處方服務機構代號 d24 診治醫師代號 d13 就醫科別
        public SpecialCode SpecialCode { get; set; }//d26 原處方服務機構之特定治療項目代號
        public DiseaseCode MainDiseaseCode { get; set; }//d8 國際疾病分類碼
        public DiseaseCode SecondDiseaseCode { get; set; }//d9 國際疾病分類碼
        public TreatmentCase.TreatmentCase TreatmentCase { get; set; }//d22 原處方服務機構之案件分類
    }
}