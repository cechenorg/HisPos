using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml;
using His_Pos.Class.Declare;
using His_Pos.H2_STOCK_MANAGE.ProductPurchase.TradeControl;
using JetBrains.Annotations;

namespace His_Pos.Class
{
    public class MedicalInfo : INotifyPropertyChanged
    {
        public MedicalInfo()
        {
            Hospital = new Hospital();
            SpecialCode = new SpecialCode();
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            TreatmentCase = new TreatmentCase.TreatmentCase();
        }

        public MedicalInfo(DataRow row)
        {
            Hospital = new Hospital(row,DataSource.GetHospitalData);
            SpecialCode = new SpecialCode();
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            TreatmentCase = new TreatmentCase.TreatmentCase(row);
            MainDiseaseCode =  new DiseaseCode.DiseaseCode { Id = row["HISINT_ID1"].ToString() };
            SecondDiseaseCode = new DiseaseCode.DiseaseCode { Id = row["HISINT_ID2"].ToString() };
        }

        public MedicalInfo(Hospital hospital, SpecialCode specialCode, List<DiseaseCode.DiseaseCode> diseaseCodes, TreatmentCase.TreatmentCase treatmentCase)
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
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            MainDiseaseCode.Id = xml.SelectSingleNode("d8") == null ? null : xml.SelectSingleNode("d8").InnerText;
            SecondDiseaseCode.Id = xml.SelectSingleNode("d9") == null ? null : xml.SelectSingleNode("d9").InnerText;
            TreatmentCase = new TreatmentCase.TreatmentCase(xml);
        }

        public MedicalInfo(DeclareFileDdata d)
        {
            Hospital = new Hospital(d);
            SpecialCode = new SpecialCode(d);
            MainDiseaseCode = new DiseaseCode.DiseaseCode();
            SecondDiseaseCode = new DiseaseCode.DiseaseCode();
            MainDiseaseCode.Id = !string.IsNullOrEmpty(d.Dbody.D8) ? d.Dbody.D8 : string.Empty;
            SecondDiseaseCode.Id = !string.IsNullOrEmpty(d.Dbody.D9) ? d.Dbody.D9 : string.Empty;
            TreatmentCase = new TreatmentCase.TreatmentCase(d);
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

        private SpecialCode _specialCode;

        public SpecialCode SpecialCode
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

        private TreatmentCase.TreatmentCase _treatmentCase;

        public TreatmentCase.TreatmentCase TreatmentCase
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