using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CooperativeInstitution;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class Treatment:ObservableObject
    {
        public Treatment()
        {
            Institution = new Institution.Institution();
            Division = new Division.Division();
            Pharmacist = new MedicalPersonnel();
            MainDisease = new DiseaseCode.DiseaseCode();
            SubDisease = new DiseaseCode.DiseaseCode();
            AdjustCase = new AdjustCase.AdjustCase();
            PrescriptionCase = new PrescriptionCase.PrescriptionCase();
            PaymentCategory = new PaymentCategory.PaymentCategory();
            SpecialTreat = new SpecialTreat.SpecialTreat();
            Copayment = new Copayment.Copayment();
        }
        public Treatment(CooperativePrescription c) {
            Institution = ViewModelMainWindow.Institutions.Count(ins => ins.Id == c.DeclareXmlDocument.Prescription.From) == 0 ? new Institution.Institution() : ViewModelMainWindow.Institutions.Single(ins => ins.Id == c.DeclareXmlDocument.Prescription.From);
            Division = ViewModelMainWindow.Divisions.Count(div => div.Id == c.DeclareXmlDocument.Prescription.Study.Subject) == 0 ? new Division.Division() : ViewModelMainWindow.Divisions.Single(div => div.Id == c.DeclareXmlDocument.Prescription.Study.Subject);
            int diseaseCount = c.DeclareXmlDocument.Prescription.Study.Diseases.Disease.Count;
            MainDisease = new DiseaseCode.DiseaseCode();
            SubDisease = new DiseaseCode.DiseaseCode();
            for (int i = 0; i < 1; i++){
                switch (i) {
                    case 0:
                            MainDisease.Id = c.DeclareXmlDocument.Prescription.Study.Diseases.Disease[i].Code;
                        break;
                    case 1:
                            SubDisease.Id = c.DeclareXmlDocument.Prescription.Study.Diseases.Disease[i].Code;
                        break;
                }
            } 
            PrescriptionCase = ViewModelMainWindow.PrescriptionCases.Count(precase => precase.Id == c.DeclareXmlDocument.Prescription.Insurance.PrescriptionCase) == 0 ? new PrescriptionCase.PrescriptionCase() : ViewModelMainWindow.PrescriptionCases.Single(precase => precase.Id == c.DeclareXmlDocument.Prescription.Insurance.PrescriptionCase);
            Copayment = ViewModelMainWindow.Copayments.Count(cop => cop.Id == c.DeclareXmlDocument.Prescription.Insurance.CopaymentCode) == 0 ? new Copayment.Copayment() : ViewModelMainWindow.Copayments.Single(cop => cop.Id == c.DeclareXmlDocument.Prescription.Insurance.CopaymentCode);
            MedicalNumber = c.DeclareXmlDocument.Prescription.Insurance.MedicalNumber;
            TreatDate = Convert.ToDateTime(c.InsertDate);
            AdjustDate = DateTime.Today;
            AdjustCase = new AdjustCase.AdjustCase();
            PaymentCategory = new PaymentCategory.PaymentCategory();
            SpecialTreat = new SpecialTreat.SpecialTreat();
            Pharmacist = new MedicalPersonnel();
        }

        public Treatment(DataRow r)
        {

        }

        private Institution.Institution institution;//釋出院所 D21
        public Institution.Institution Institution
        {
            get => institution;
            set
            {
                Set(() => Institution, ref institution, value);
            }
        }

        private Division.Division division;//就醫科別 D13
        public Division.Division Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
            }
        }

        private MedicalPersonnel pharmacist;//醫事人員代號 D25
        public MedicalPersonnel Pharmacist
        {
            get => pharmacist;
            set
            {
                Set(() => Pharmacist, ref pharmacist, value);
            }
        }

        private string medicalNumber;//就醫序號 D7
        public string MedicalNumber
        {
            get => medicalNumber;
            set
            {
                Set(() => MedicalNumber, ref medicalNumber, value);
            }
        }

        private DateTime treatDate;//就醫日期 D7
        public DateTime TreatDate
        {
            get => treatDate;
            set
            {
                Set(() => TreatDate, ref treatDate, value);
            }
        }

        private DateTime adjustDate;//調劑日期 D23
        public DateTime AdjustDate
        {
            get => adjustDate;
            set
            {
                Set(() => AdjustDate, ref adjustDate, value);
            }
        }

        private DiseaseCode.DiseaseCode mainDisease;//主診斷代碼(國際疾病分類碼1) D8
        public DiseaseCode.DiseaseCode MainDisease
        {
            get => mainDisease;
            set
            {
                Set(() => MainDisease, ref mainDisease, value);
            }
        }

        private DiseaseCode.DiseaseCode subDisease;//副診斷代碼(國際疾病分類碼2) D9
        public DiseaseCode.DiseaseCode SubDisease
        {
            get => subDisease;
            set
            {
                Set(() => SubDisease, ref subDisease, value);
            }
        }

        private int? chronicTotal;//連續處方可調劑次數 D36
        public int? ChronicTotal
        {
            get => chronicTotal;
            set
            {
                Set(() => ChronicTotal, ref chronicTotal, value);
            }
        }

        private int? chronicSeq;
        public int? ChronicSeq
        {
            get => chronicSeq;
            set
            {
                Set(() => ChronicSeq, ref chronicSeq, value);
            }
        }//連續處方箋調劑序號 D35

        private AdjustCase.AdjustCase adjustCase;//調劑案件 D1
        public AdjustCase.AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                Set(() => AdjustCase, ref adjustCase, value);
            }
        }

        private PrescriptionCase.PrescriptionCase prescriptionCase;//原處方服務機構之案件分類  D22
        public PrescriptionCase.PrescriptionCase PrescriptionCase
        {
            get => prescriptionCase;
            set
            {
                Set(() => PrescriptionCase, ref prescriptionCase, value);
            }
        }

        private Copayment.Copayment copayment;//部分負擔代碼  D15
        public Copayment.Copayment Copayment
        {
            get => copayment;
            set
            {
                Set(() => Copayment, ref copayment, value);
            }
        }

        private PaymentCategory.PaymentCategory paymentCategory;//給付類別 D5
        public PaymentCategory.PaymentCategory PaymentCategory
        {
            get => paymentCategory;
            set
            {
                Set(() => PaymentCategory, ref paymentCategory, value);
            }
        }

        private string originalMedicalNumber;//原處方就醫序號 D43
        public string OriginalMedicalNumber
        {
            get => originalMedicalNumber;
            set
            {
                Set(() => OriginalMedicalNumber, ref originalMedicalNumber, value);
            }
        }

        private SpecialTreat.SpecialTreat specialTreat;//特定治療代碼 D26
        public SpecialTreat.SpecialTreat SpecialTreat
        {
            get => specialTreat;
            set
            {
                Set(() => SpecialTreat, ref specialTreat, value);
            }
        }
        
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
