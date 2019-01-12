using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class Treatment : INotifyPropertyChanged
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
        public Treatment(DataRow r)
        {

        }

        private Institution.Institution institution;//釋出院所 D21
        public Institution.Institution Institution
        {
            get => institution;
            set
            {
                institution = value;
                OnPropertyChanged(nameof(Institution));
            }
        }

        private Division.Division division;//就醫科別 D13
        public Division.Division Division
        {
            get => division;
            set
            {
                division = value;
                OnPropertyChanged(nameof(Division));
            }
        }

        private MedicalPersonnel pharmacist;//醫事人員代號 D25
        public MedicalPersonnel Pharmacist
        {
            get => pharmacist;
            set
            {
                pharmacist = value;
                OnPropertyChanged(nameof(Pharmacist));
            }
        }

        private string medicalNumber;//就醫序號 D7
        public string MedicalNumber
        {
            get => medicalNumber;
            set
            {
                medicalNumber = value;
                OnPropertyChanged(nameof(MedicalNumber));
            }
        }

        private DateTime treatDate;//就醫日期 D7
        public DateTime TreatDate
        {
            get => treatDate;
            set
            {
                treatDate = value;
                OnPropertyChanged(nameof(TreatDate));
            }
        }

        private DateTime adjustDate;//調劑日期 D23
        public DateTime AdjustDate
        {
            get => adjustDate;
            set
            {
                adjustDate = value;
                OnPropertyChanged(nameof(AdjustDate));
            }
        }

        private DiseaseCode.DiseaseCode mainDisease;//主診斷代碼(國際疾病分類碼1) D8
        public DiseaseCode.DiseaseCode MainDisease
        {
            get => mainDisease;
            set
            {
                mainDisease = value;
                OnPropertyChanged(nameof(MainDisease));
            }
        }

        private DiseaseCode.DiseaseCode subDisease;//副診斷代碼(國際疾病分類碼2) D9
        public DiseaseCode.DiseaseCode SubDisease
        {
            get => subDisease;
            set
            {
                subDisease = value;
                OnPropertyChanged(nameof(SubDisease));
            }
        }

        private int chronicTotal;//連續處方可調劑次數 D36
        public int ChronicTotal
        {
            get => chronicTotal;
            set
            {
                chronicTotal = value;
                OnPropertyChanged(nameof(ChronicTotal));
            }
        }

        private int chronicSeq;
        public int ChronicSeq
        {
            get => chronicSeq;
            set
            {
                chronicSeq = value;
                OnPropertyChanged(nameof(ChronicSeq));
            }
        }//連續處方箋調劑序號 D35

        private AdjustCase.AdjustCase adjustCase;//調劑案件 D1
        public AdjustCase.AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                adjustCase = value;
                OnPropertyChanged(nameof(AdjustCase));
            }
        }

        private PrescriptionCase.PrescriptionCase prescriptionCase;//原處方服務機構之案件分類  D22
        public PrescriptionCase.PrescriptionCase PrescriptionCase
        {
            get => prescriptionCase;
            set
            {
                prescriptionCase = value;
                OnPropertyChanged(nameof(PrescriptionCase));
            }
        }

        private Copayment.Copayment copayment;//部分負擔代碼  D15
        public Copayment.Copayment Copayment
        {
            get => copayment;
            set
            {
                copayment = value;
                OnPropertyChanged(nameof(Copayment));
            }
        }

        private PaymentCategory.PaymentCategory paymentCategory;//給付類別 D5
        public PaymentCategory.PaymentCategory PaymentCategory
        {
            get => paymentCategory;
            set
            {
                paymentCategory = value;
                OnPropertyChanged(nameof(PaymentCategory));
            }
        }

        private string originalMedicalNumber;//原處方就醫序號 D43
        public string OriginalMedicalNumber
        {
            get => originalMedicalNumber;
            set
            {
                originalMedicalNumber = value;
                OnPropertyChanged(nameof(OriginalMedicalNumber));
            }
        }

        private SpecialTreat.SpecialTreat specialTreat;//特定治療代碼 D26
        public SpecialTreat.SpecialTreat SpecialTreat
        {
            get => specialTreat;
            set
            {
                specialTreat = value;
                OnPropertyChanged(nameof(SpecialTreat));
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
