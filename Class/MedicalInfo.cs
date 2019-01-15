using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.Service;
using JetBrains.Annotations;

namespace His_Pos.Class
{
    public class MedicalInfo : INotifyPropertyChanged
    {
        public MedicalInfo()
        {
            Hospital = new Hospital();
            SpecialCode = new SpecialTreat();
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            TreatmentCase = new PrescriptionCase();
        }

        public MedicalInfo(DataRow row)
        {
            Hospital = new Hospital(row,DataSource.GetHospitalData);
            SpecialCode = new SpecialTreat();
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            TreatmentCase = new PrescriptionCase(row);
            MainDiseaseCode =  new DiseaseCode.DiseaseCode { Id = row["HISINT_ID1"].ToString() };
            SecondDiseaseCode = new DiseaseCode.DiseaseCode { Id = row["HISINT_ID2"].ToString() };
            SpecialCode = new SpecialTreat(row);
        }

        public MedicalInfo(Hospital hospital, SpecialTreat specialCode, List<DiseaseCode.DiseaseCode> diseaseCodes, PrescriptionCase treatmentCase)
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
            SpecialCode = new SpecialTreat();
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            MainDiseaseCode.Id = xml.SelectSingleNode("d8") == null ? null : xml.SelectSingleNode("d8").InnerText;
            SecondDiseaseCode.Id = xml.SelectSingleNode("d9") == null ? null : xml.SelectSingleNode("d9").InnerText;
            TreatmentCase = new PrescriptionCase();
        }
        public MedicalInfo(XmlDocument xml)
        {
            Hospital = new Hospital(xml);
            SpecialCode = new SpecialTreat();
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            MainDiseaseCode.Id = xml.SelectNodes("DeclareXml/DeclareXmlDocument/case/study/diseases/item")[0].Attributes["code"].Value;
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            TreatmentCase = new PrescriptionCase();
        }

        public MedicalInfo(DeclareFileDdata d)
        {
            ///Hospital = MainWindow.Hospitals.SingleOrDefault(h => h.Id.Equals(d.Dhead.D21))?.DeepCloneViaJson();
            if (Hospital != null)
            {
                Hospital.Doctor = new MedicalPersonnel();
                Hospital.Doctor.IcNumber = !string.IsNullOrEmpty(d.Dhead.D24) ? d.Dhead.D24 : string.Empty;
                Hospital.Division = ViewModelMainWindow.Divisions.SingleOrDefault(div => div.Id.Equals(d.Dhead.D13))
                    ?.DeepCloneViaJson();
            }
            SpecialCode = new SpecialTreat();
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            MainDiseaseCode.Id = !string.IsNullOrEmpty(d.Dhead.D8) ? d.Dhead.D8 : string.Empty;
            SecondDiseaseCode.Id = !string.IsNullOrEmpty(d.Dhead.D9) ? d.Dhead.D9 : string.Empty;
            TreatmentCase = ViewModelMainWindow.PrescriptionCases.SingleOrDefault(t => t.Id.Equals(d.Dhead.D22))?.DeepCloneViaJson();
        }

        private Hospital _hospital;

        public Hospital Hospital
        {
            get => _hospital;
            set
            {
                _hospital = value;
                OnPropertyChanged(nameof(Hospital));
            }
        } //d21 原處方服務機構代號 d24 診治醫師代號 d13 就醫科別

        private SpecialTreat _specialCode;

        public SpecialTreat SpecialCode
        {
            get => _specialCode;
            set
            {
                _specialCode = value;
                OnPropertyChanged(nameof(SpecialCode));
            }
        } //d26 原處方服務機構之特定治療項目代號

        private DiseaseCode.DiseaseCode _mainDiseaseCode;

        public DiseaseCode.DiseaseCode MainDiseaseCode
        {
            get => _mainDiseaseCode;
            set
            {
                _mainDiseaseCode = value;
                OnPropertyChanged(nameof(MainDiseaseCode));
            }
        } //d8 國際疾病分類碼

        private DiseaseCode.DiseaseCode _secondDiseaseCode;

        public DiseaseCode.DiseaseCode SecondDiseaseCode
        {
            get => _secondDiseaseCode;
            set
            {
                _secondDiseaseCode = value;
                OnPropertyChanged(nameof(SecondDiseaseCode));
            }
        } //d9 國際疾病分類碼

        private PrescriptionCase _treatmentCase;

        public PrescriptionCase TreatmentCase
        {
            get => _treatmentCase;
            set
            {
                _treatmentCase = value;
                OnPropertyChanged(nameof(TreatmentCase));
            }
        } //d22 原處方服務機構之案件分類

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}