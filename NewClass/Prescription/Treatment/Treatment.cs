using System;
using System.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Person.MedicalPerson;
using StringRes = His_Pos.Properties.Resources;

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
        public Treatment(CooperativePrescription c)
        {
            var prescription = c.DeclareXmlDocument.Prescription;
            var study = prescription.Study;
            var diseases = study.Diseases.Disease;
            var insurance = prescription.Insurance;
            var chronic = prescription.Continous_prescription;
            Institution = ViewModelMainWindow.GetInstitution(prescription.From);
            Division = ViewModelMainWindow.GetDivision(study.Subject);
            var diseaseCount = diseases.Count;
            if (diseaseCount > 2)
                diseaseCount = 2;
            MainDisease = new DiseaseCode.DiseaseCode();
            SubDisease = new DiseaseCode.DiseaseCode();
            for (int i = 0; i < diseaseCount; i++){
                switch (i) {
                    case 0:
                            MainDisease.ID = diseases[i].Code;
                        break;
                    case 1:
                            SubDisease.ID = diseases[i].Code;
                        break;
                }
            }
            PrescriptionCase = ViewModelMainWindow.GetPrescriptionCases(insurance.PrescriptionCase);
            Copayment = ViewModelMainWindow.GetCopayment(insurance.CopaymentCode);
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null && ChronicTotal != null) {
                OriginalMedicalNumber = insurance.MedicalNumber;
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = ViewModelMainWindow.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else {
                MedicalNumber = insurance.MedicalNumber;
                AdjustCase = ViewModelMainWindow.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            TreatDate = Convert.ToDateTime(c.InsertDate);
            AdjustDate = DateTime.Today;
            PaymentCategory = ViewModelMainWindow.GetPaymentCategory("4");
            SpecialTreat = new SpecialTreat.SpecialTreat();
            Pharmacist = ViewModelMainWindow.CurrentPharmacy.GetPharmacist();
        }

        public Treatment(DataRow r)
        {
            Division = ViewModelMainWindow.GetDivision(r.Field<string>("DivisionID").ToString());
            AdjustCase = ViewModelMainWindow.GetAdjustCase(r.Field<string>("AdjustCaseID").ToString());
            Copayment = ViewModelMainWindow.GetCopayment(r.Field<string>("CopaymentID").ToString());
            PrescriptionCase = ViewModelMainWindow.GetPrescriptionCases(r.Field<string>("PrescriptionCaseID").ToString());
            Institution = ViewModelMainWindow.GetInstitution(r.Field<string>("InstitutionID").ToString());
            PaymentCategory = ViewModelMainWindow.GetPaymentCategory(r.Field<string>("PaymentCategoryID").ToString());
            AdjustDate = r.Field<DateTime>("AdjustDate");
            TreatDate = r.Field<DateTime>("TreatmentDate");
            if(!string.IsNullOrEmpty(r.Field<string>("ChronicSequence")))
                ChronicSeq = int.Parse(r.Field<string>("ChronicSequence"));
            if (!string.IsNullOrEmpty(r.Field<string>("ChronicTotal")))
                ChronicTotal = int.Parse(r.Field<string>("ChronicTotal")); 
            MainDisease = new DiseaseCode.DiseaseCode();
            MainDisease.ID = r.Field<string>("MainDiseaseID");
            SubDisease = new DiseaseCode.DiseaseCode();
            SubDisease.ID = r.Field<string>("SecondDiseaseID");
            Pharmacist = new MedicalPersonnel(r);
            SpecialTreat = new SpecialTreat.SpecialTreat();
            SpecialTreat.Id = r.Field<string>("SpecialTreatID");

            MedicalNumber = r.Field<string>("MedicalNumber");
            OriginalMedicalNumber = r.Field<string>("OldMedicalNumber");
             
        }
        #region Variables
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

        private DateTime? treatDate;//就醫日期 D7
        public DateTime? TreatDate
        {
            get => treatDate;
            set
            {
                Set(() => TreatDate, ref treatDate, value);
            }
        }

        private DateTime? adjustDate;//調劑日期 D23
        public DateTime? AdjustDate
        {
            get => adjustDate;
            set
            {
                Set(() => AdjustDate, ref adjustDate, value);
                Messenger.Default.Send(new NotificationMessage("AdjustDateChanged"));
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
        private string tempMedicalNumber;
        public string TempMedicalNumber
        {
            get => tempMedicalNumber;
            set
            {
                if (tempMedicalNumber != value)
                {
                    Set(() => TempMedicalNumber, ref tempMedicalNumber, value);
                }
            }
        }
        #endregion
        #region NHIRulesCheckFunctions
        private string CheckInstitution()
        {
            if (CheckIsHomeCare() || CheckIsQuitSmoking())
            {
                Institution = new Institution.Institution { Id = "N", Name = string.Empty };
                return string.Empty;
            }
            return Institution is null ? StringRes.InstitutionError : string.Empty;
        }
        private string CheckAdjustCase()
        {
            if (string.IsNullOrEmpty(AdjustCase.Id))
                return StringRes.AdjustCaseError;
            return string.Empty;
        }
        private string CheckPrescriptionCase()
        {
            if (!CheckIsHomeCare() && !CheckIsQuitSmoking() && string.IsNullOrEmpty(PrescriptionCase.Id))
                return StringRes.PrescriptionCaseError;
            return string.Empty;
        }
        private string CheckAdjustDate()
        {
            if (AdjustDate is null) return StringRes.AdjustDateError;
            if (TreatDate == null || !(ChronicSeq is null)) return string.Empty;
            var startDate = (DateTime)TreatDate;
            var endDate = (DateTime)AdjustDate;
            var holiday = 0;
            while (startDate < endDate)
            {
                if ((int)startDate.DayOfWeek == 0 || (int)startDate.DayOfWeek == 6)
                {
                    holiday += 1;
                }
                startDate = startDate.AddDays(1);
            }
            if (new TimeSpan(endDate.Ticks - startDate.Ticks).Days - holiday > 3)
            {
                return StringRes.PrescriptoinOutOfDate;
            }
            return string.Empty;
        }
        private string CheckMedicalNumber()
        {
            if (string.IsNullOrEmpty(TempMedicalNumber))
            {
                if (!CheckIsHomeCare()) return StringRes.MedicalNumberError;
                TempMedicalNumber = "N";
                return string.Empty;
            }
            if (ChronicSeq is null)
                MedicalNumber = TempMedicalNumber;
            else
            {
                if (ChronicSeq > 1)
                {
                    MedicalNumber = "IC0" + ChronicSeq;
                    OriginalMedicalNumber = TempMedicalNumber;
                }
                else
                {
                    MedicalNumber = TempMedicalNumber;
                }
            }
            return string.Empty;
        }
        private string CheckCopayment()
        {
            if (CheckIsHomeCare())
            {
                Copayment = ViewModelMainWindow.GetCopayment("009");
                return string.Empty;
            }
            return string.IsNullOrEmpty(Copayment.Id) ? StringRes.CopaymentError : string.Empty;
        }
        private string CheckPharmacist()
        {
            return string.IsNullOrEmpty(Pharmacist.IdNumber) ? StringRes.PharmacistIDError : string.Empty;
        }
        private string CheckDivision()
        {
            if (Division is null || string.IsNullOrEmpty(Division.Id))
            {
                if (CheckIsHomeCare() || CheckIsQuitSmoking())
                    return string.Empty;
                return StringRes.DivisionError;
            }
            return string.Empty;
        }
        private string CheckTreatDate()
        {
            if (TreatDate is null)
            {
                if (CheckIsHomeCare())
                    return string.Empty;
                return StringRes.TreatDateError;
            }
            return string.Empty;
        }
        private string CheckPaymentCategory()
        {
            if (PaymentCategory is null)
            {
                if (CheckIsHomeCare() || ChronicSeq != null || AdjustCase.Id.Equals("2"))
                    return string.Empty;
                return StringRes.PaymentCategoryError;
            }
            return string.Empty;
        }
        public bool CheckIsQuitSmoking()
        {
            if (string.IsNullOrEmpty(AdjustCase.Id)) return false;
            return AdjustCase.Id.Equals("5");
        }
        public bool CheckIsHomeCare()
        {
            if (string.IsNullOrEmpty(AdjustCase.Id)) return false;
            return AdjustCase.Id.Equals("D");
        }
        private string CheckDiseaseCode()
        {
            if (string.IsNullOrEmpty(MainDisease.ID))
            {
                if (CheckIsHomeCare())
                    return string.Empty;
                return StringRes.DiseaseCodeError;
            }
            return string.Empty;
        }
        private string CheckChronicTimes()
        {
            if (string.IsNullOrEmpty(AdjustCase.Id)) return string.Empty;
            if (!AdjustCase.Id.Equals("2")) return string.Empty;
            if (ChronicSeq is null && ChronicTotal is null)
                return StringRes.ChronicTimesError; 
            if (ChronicSeq is null)
                return StringRes.ChronicSeqError;
            if (ChronicTotal is null)
                return StringRes.ChronicTotalError;
            return string.Empty;
        }
        public string Check()
        {
            return
             CheckInstitution() +
             CheckAdjustCase() +
             CheckPrescriptionCase() +
             CheckAdjustDate() +
             CheckPharmacist() +
             CheckMedicalNumber() +
             CheckCopayment() +
             CheckDivision() +
             CheckTreatDate() +
             CheckPaymentCategory() +
             CheckDiseaseCode() +
             CheckChronicTimes();
        }
        #endregion

        public void Initial()
        {
            Division = null;
            SpecialTreat = null;

            Pharmacist = ViewModelMainWindow.CurrentPharmacy.GetPharmacist();
            TreatDate = DateTime.Today;
            AdjustDate = DateTime.Today;
            AdjustCase = ViewModelMainWindow.GetAdjustCase("1");
            PrescriptionCase = ViewModelMainWindow.GetPrescriptionCases("09");
            PaymentCategory = ViewModelMainWindow.GetPaymentCategory("4");
            Copayment = ViewModelMainWindow.GetCopayment("I20");
        }
    }
}
