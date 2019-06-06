using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.Service;
using StringRes = His_Pos.Properties.Resources;
using Ins = His_Pos.NewClass.Prescription.Treatment.Institution.Institution;
using Div = His_Pos.NewClass.Prescription.Treatment.Division.Division;
using DisCode = His_Pos.NewClass.Prescription.Treatment.DiseaseCode.DiseaseCode;
using AdjCase = His_Pos.NewClass.Prescription.Treatment.AdjustCase.AdjustCase;
using PayCat = His_Pos.NewClass.Prescription.Treatment.PaymentCategory.PaymentCategory;
using SpeTre = His_Pos.NewClass.Prescription.Treatment.SpecialTreat.SpecialTreat;
using Cop = His_Pos.NewClass.Prescription.Treatment.Copayment.Copayment;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Person.Employee;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class Treatment:ObservableObject,ICloneable
    {
        public Treatment()
        {
            Institution = new Ins();
            Division = new Div();
            Pharmacist = new Employee();
            MainDisease = new DisCode();
            SubDisease = new DisCode();
            AdjustCase = new AdjCase();
            PrescriptionCase = new PrescriptionCase.PrescriptionCase();
            PaymentCategory = new PayCat();
            SpecialTreat = new SpeTre();
            Copayment = new Cop();
        }
        public Treatment(OrthopedicsPrescription c)
        {
            var prescription = c.DeclareXmlDocument.Prescription;
            var study = prescription.Study;
            var diseases = study.Diseases.Disease;
            var insurance = prescription.Insurance;
            var chronic = prescription.Continous_prescription;
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            var diseaseCount = diseases.Count;
            if (diseaseCount > 2)
                diseaseCount = 2;
            MainDisease = new DisCode();
            SubDisease = new DisCode();
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
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            Copayment = new Cop();
            if (!string.IsNullOrEmpty(insurance.CopaymentCode))
            {
                switch (insurance.CopaymentCode)
                {
                    case "003":
                    case "004":
                    case "007":
                    case "009":
                    case "I22":
                    case "001":
                    case "002":
                    case "005":
                    case "006":
                    case "008":
                    case "902":
                    case "903":
                    case "906":
                    case "907":
                        Copayment = VM.GetCopayment(insurance.CopaymentCode);
                        break;
                }       
            }             
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null && ChronicTotal != null) {
                OriginalMedicalNumber = insurance.MedicalNumber;
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = VM.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else {
                MedicalNumber = insurance.MedicalNumber;
                AdjustCase = VM.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            if (string.IsNullOrEmpty(TempMedicalNumber) && !string.IsNullOrEmpty(c.DeclareXmlDocument.Prescription.Insurance.IcErrorCode)) //例外就醫
                TempMedicalNumber = c.DeclareXmlDocument.Prescription.Insurance.IcErrorCode;

            TreatDate =  Convert.ToDateTime(c.InsertDate);
            AdjustDate = DateTime.Today;
            PaymentCategory = VM.GetPaymentCategory("4");
            SpecialTreat = new SpeTre();
        }
        public Treatment(CooperativePrescription.Prescription c,DateTime treatDate) {
            var prescription = c;
            var study = prescription.Study;
            var diseases = study.Diseases.Disease;
            var insurance = prescription.Insurance;
            var chronic = prescription.Continous_prescription;
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            var diseaseCount = diseases.Count;
            if (diseaseCount > 2)
                diseaseCount = 2;
            MainDisease = new DisCode();
            SubDisease = new DisCode();
            for (int i = 0; i < diseaseCount; i++)
            {
                switch (i)
                {
                    case 0:
                        MainDisease.ID = diseases[i].Code;
                        break;
                    case 1:
                        SubDisease.ID = diseases[i].Code;
                        break;
                }
            }
            PrescriptionCase = VM.GetPrescriptionCases(insurance.PrescriptionCase);
            Copayment = new Cop();
            if (!string.IsNullOrEmpty(insurance.CopaymentCode))
            {
                switch (insurance.CopaymentCode)
                {
                    case "003":
                    case "004":
                    case "007":
                    case "009":
                    case "I22":
                    case "001":
                    case "002":
                    case "005":
                    case "006":
                    case "008":
                    case "902":
                    case "903":
                    case "906":
                    case "907":
                        Copayment = VM.GetCopayment(insurance.CopaymentCode);
                        break;
                }
            }
            int.TryParse(chronic.Count, out var seq);
            if (seq != 0)
                ChronicSeq = seq;
            int.TryParse(chronic.Total, out var total);
            if (total != 0)
                ChronicTotal = total;
            if (ChronicSeq != null && ChronicTotal != null)
            {
                OriginalMedicalNumber = insurance.MedicalNumber;
                MedicalNumber = "IC0" + ChronicSeq;
                AdjustCase = VM.GetAdjustCase("2");
                TempMedicalNumber = OriginalMedicalNumber;
            }
            else
            {
                MedicalNumber = insurance.MedicalNumber;
                AdjustCase = VM.GetAdjustCase("1");
                TempMedicalNumber = MedicalNumber;
            }
            if (string.IsNullOrEmpty(TempMedicalNumber) && !string.IsNullOrEmpty(c.Insurance.IcErrorCode)) //例外就醫
                TempMedicalNumber = c.Insurance.IcErrorCode;
             
            TreatDate = treatDate.Date;
            AdjustDate = DateTime.Today;
            PaymentCategory = VM.GetPaymentCategory("4");
            SpecialTreat = new SpeTre();
        } 
        public Treatment(DataRow r)
        {
            Division = VM.GetDivision(r.Field<string>("DivisionID"));
            AdjustCase = VM.GetAdjustCase(r.Field<string>("AdjustCaseID"));
            Copayment = VM.GetCopayment(r.Field<string>("CopaymentID"));
            PrescriptionCase = VM.GetPrescriptionCases(r.Field<string>("PrescriptionCaseID"));
            Institution = VM.GetInstitution(r.Field<string>("InstitutionID"));
            PaymentCategory = VM.GetPaymentCategory(r.Field<string>("PaymentCategoryID"));
            AdjustDate = r.Field<DateTime>("AdjustDate");
            TreatDate = r.Field<DateTime?>("TreatmentDate"); 
            if (!string.IsNullOrEmpty(r.Field<byte?>("ChronicSequence").ToString()))
                ChronicSeq = int.Parse(r.Field<byte>("ChronicSequence").ToString());
            if (!string.IsNullOrEmpty(r.Field<byte?>("ChronicTotal").ToString()))
                ChronicTotal = int.Parse(r.Field<byte>("ChronicTotal").ToString()); 
            MainDisease = new DisCode();
            if (!string.IsNullOrEmpty(r.Field<string>("MainDiseaseID")))
            {
                MainDisease = DisCode.GetDiseaseCodeByID(r.Field<string>("MainDiseaseID"));
            }
            SubDisease = new DisCode();
            if (!string.IsNullOrEmpty(r.Field<string>("SecondDiseaseID")))
            {
                SubDisease = DisCode.GetDiseaseCodeByID(r.Field<string>("SecondDiseaseID"));
            }
            Pharmacist = VM.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(p => p.IDNumber.Equals(r.Field<string>("Emp_IDNumber")));
            SpecialTreat = new SpeTre();
            if (!string.IsNullOrEmpty(r.Field<string>("SpecialTreatID")))
            {
                SpecialTreat = VM.GetSpecialTreat(r.Field<string>("SpecialTreatID"));
            }
            MedicalNumber = r.Field<string>("MedicalNumber");
            OriginalMedicalNumber = r.Field<string>("OldMedicalNumber");

            if (AdjustCase.ID.Equals("2"))
            {
                TempMedicalNumber = ChronicSeq == 1 ? MedicalNumber : OriginalMedicalNumber;
            }
            else
                TempMedicalNumber = MedicalNumber;

        }

        #region Variables
        private Ins institution;//釋出院所 D21
        public Ins Institution
        {
            get => institution;
            set
            {
                Set(() => Institution, ref institution, value);
            }
        }

        private Div division;//就醫科別 D13
        public Div Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
            }
        }

        private Employee pharmacist;//醫事人員代號 D25
        public Employee Pharmacist
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

        private DisCode mainDisease;//主診斷代碼(國際疾病分類碼1) D8
        public DisCode MainDisease
        {
            get => mainDisease;
            set
            {
                Set(() => MainDisease, ref mainDisease, value);
            }
        }

        private DisCode subDisease;//副診斷代碼(國際疾病分類碼2) D9
        public DisCode SubDisease
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

        private AdjCase adjustCase;//調劑案件 D1
        public AdjCase AdjustCase
        {
            get => adjustCase;
            set
            {
                if (value != adjustCase)
                    Set(() => AdjustCase, ref adjustCase, value);
            }
        }

        private PrescriptionCase.PrescriptionCase prescriptionCase;//原處方服務機構之案件分類  D22
        public PrescriptionCase.PrescriptionCase PrescriptionCase
        {
            get => prescriptionCase;
            set
            {
                if (value != prescriptionCase)
                    Set(() => PrescriptionCase, ref prescriptionCase, value);
            }
        }

        private Cop copayment;//部分負擔代碼  D15
        public Cop Copayment
        {
            get => copayment;
            set
            {
                if (value != copayment)
                    Set(() => Copayment, ref copayment, value);
            }
        }

        private PayCat paymentCategory;//給付類別 D5
        public PayCat PaymentCategory
        {
            get => paymentCategory;
            set
            {
                if (value != paymentCategory)
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

        private SpeTre specialTreat;//特定治療代碼 D26
        public SpeTre SpecialTreat
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
        public PrescriptionType Type { get; private set; }
        #endregion
        #region NHIRulesCheckFunctions
        private string CheckInstitution()
        {
            if (CheckIsHomeCare() || CheckIsQuitSmoking())
            {
                Institution = new Ins { ID = "N", Name = string.Empty };
                return string.Empty;
            }
            if (string.IsNullOrEmpty(Institution.ID))
                return StringRes.InstitutionError;
            return VM.GetInstitution(Institution.ID) is null ? StringRes.InstitutionError : string.Empty;
        }
        private void CheckPrescribeInstitution()
        {
            if (string.IsNullOrEmpty(Institution.FullName))
            {
                Institution =
                    new Ins
                    {
                        ID = VM.CurrentPharmacy.ID,
                        Name = VM.CurrentPharmacy.Name,
                        FullName = VM.CurrentPharmacy.ID + VM.CurrentPharmacy.Name
                    };
            }
        }
        private string CheckAdjustCase()
        {
            if (string.IsNullOrEmpty(AdjustCase.ID))
                return StringRes.AdjustCaseError;
            return string.Empty;
        }
        private string CheckPrescriptionCase()
        {
            if (!CheckIsHomeCare() && !CheckIsQuitSmoking() && string.IsNullOrEmpty(PrescriptionCase.ID))
                return StringRes.PrescriptionCaseError;
            if (Division.ID.Equals("40") && (AdjustCase.ID.Equals("1") || AdjustCase.ID.Equals("3")))
                PrescriptionCase = VM.GetPrescriptionCases("19");
            return string.Empty;
        }
        public bool CheckAdjustDate()
        {
            if (AdjustDate is null)
            {
                MessageWindow.ShowMessage(StringRes.AdjustDateError, MessageType.WARNING);
                return false;
            }
            if (TreatDate == null || !(ChronicSeq is null)) return true;
            var startDate = (DateTime)TreatDate;
            var tmpStartDate = startDate.DeepCloneViaJson();
            var endDate = (DateTime)AdjustDate;
            var holiday = 0;
            while (tmpStartDate < endDate)
            {
                if ((int)tmpStartDate.DayOfWeek == 0 || (int)tmpStartDate.DayOfWeek == 6)
                {
                    holiday += 1;
                }
                tmpStartDate = tmpStartDate.AddDays(1);
            }
            if (new TimeSpan(endDate.Ticks - startDate.Ticks).Days - holiday > 3)
            {
                var adjustDateOutOfRange = new ConfirmWindow(StringRes.PrescriptoinOutOfDate,"");
                Debug.Assert(adjustDateOutOfRange.DialogResult != null, "adjustDateOutOfRange.DialogResult != null");
                return (bool)adjustDateOutOfRange.DialogResult;
            }
            return true;
        }
        public string CheckMedicalNumber(bool noCard)
        {
            if (noCard)
            {
                if (!string.IsNullOrEmpty(TempMedicalNumber))
                {
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
                            OriginalMedicalNumber = null;
                        }
                    }
                }
                return string.Empty;
            }
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
                    OriginalMedicalNumber = null;
                }
            }
            return string.Empty;
        }
        private string CheckCopayment()
        {
            if (Copayment is null) return StringRes.CopaymentError;
            if (CheckIsHomeCare())
            {
                Copayment = VM.GetCopayment("009");
                return string.Empty;
            }
            return string.IsNullOrEmpty(Copayment.Id) ? StringRes.CopaymentError : string.Empty;
        }
        private string CheckPharmacist()
        {
            return string.IsNullOrEmpty(Pharmacist.IDNumber) ? StringRes.PharmacistIDError : string.Empty;
        }
        private string CheckDivision()
        {
            if (Division is null || string.IsNullOrEmpty(Division.ID))
            {
                if (CheckIsHomeCare() || CheckIsQuitSmoking())
                    return string.Empty;
                return StringRes.DivisionError;
            }
            return string.Empty;
        }
        private string CheckTreatDate()
        {
            if (TreatDate is null) return CheckIsHomeCare() ? string.Empty : StringRes.TreatDateError;
            if (TreatDate != null && DateTime.Compare((DateTime)TreatDate, (DateTime)AdjustDate) > 0) return "就醫日不可大於調劑日";
            return string.Empty;
        }
        private string CheckPaymentCategory()
        {
            if (PaymentCategory is null)
            {
                if (CheckIsHomeCare() || ChronicSeq != null || AdjustCase.ID.Equals("2"))
                    return string.Empty;
                return StringRes.PaymentCategoryError;
            }
            return string.Empty;
        }
        public bool CheckIsQuitSmoking()
        {
            if (string.IsNullOrEmpty(AdjustCase.ID)) return false;
            return AdjustCase.ID.Equals("5");
        }
        public bool CheckIsHomeCare()
        {
            if (string.IsNullOrEmpty(AdjustCase.ID)) return false;
            return AdjustCase.ID.Equals("D");
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
            if (string.IsNullOrEmpty(AdjustCase.ID)) return string.Empty;
            if (!AdjustCase.ID.Equals("2")) return string.Empty;
            if (ChronicSeq is null && ChronicTotal is null)
                return StringRes.ChronicTimesError; 
            if (ChronicSeq is null)
                return StringRes.ChronicSeqError;
            if (ChronicTotal is null)
                return StringRes.ChronicTotalError;
            return string.Empty;
        }
        public string Check(bool noCard)
        {
            return
             CheckInstitution() +
             CheckAdjustCase() +
             CheckPrescriptionCase() +
             CheckPharmacist() +
             CheckMedicalNumber(noCard) +
             CheckCopayment() +
             CheckDivision() +
             CheckTreatDate() +
             CheckPaymentCategory() +
             CheckDiseaseCode() +
             CheckChronicTimes();
        }
        public string CheckPrescribe()
        {
            CheckPrescribeInstitution();
            if (AdjustCase is null || !AdjustCase.ID.Equals("0"))
                AdjustCase = VM.GetAdjustCase("0").DeepCloneViaJson();
            return CheckPharmacist();
        }
        #endregion

        public void Initial()
        {
            Division = null;
            SpecialTreat = null;
            TreatDate = DateTime.Today;
            AdjustDate = DateTime.Today;
            AdjustCase = VM.GetAdjustCase("1");
            PrescriptionCase = VM.GetPrescriptionCases("09");
            PaymentCategory = VM.GetPaymentCategory("4");
            Copayment = VM.GetCopayment("I20");
        }
        public void GetLastMedicalNumber()
        {
            if (HisApiFunction.OpenCom())
            {
                int iBufferLen = 7;
                byte[] pBuffer = new byte[7];
                var res = HisApiBase.hisGetLastSeqNum(pBuffer, ref iBufferLen);
                if (res == 0)
                    TempMedicalNumber = Function.ByteArrayToString(4, pBuffer, 3);
                HisApiFunction.CloseCom();
            }
        }

        public void Clear()
        {
            if (string.IsNullOrEmpty(Institution.FullName))
            {
                Institution =
                    new Ins
                    {
                        ID = VM.CurrentPharmacy.ID,
                        Name = VM.CurrentPharmacy.Name,
                        FullName = VM.CurrentPharmacy.ID + VM.CurrentPharmacy.Name
                    };
            }
            PrescriptionCase = null;
            TempMedicalNumber = string.Empty;
            Copayment = null;
            TreatDate = null;
            ChronicSeq = null;
            ChronicTotal = null;
            Division = null;
            MainDisease = null;
            SubDisease = null;
            SpecialTreat = null;
            PaymentCategory = null;
        }

        public object Clone()
        {
            Treatment t = new Treatment();
            t.AdjustCase = VM.GetAdjustCase(AdjustCase.ID);
            t.AdjustDate = AdjustDate;
            t.ChronicSeq = ChronicSeq;
            t.ChronicTotal = chronicTotal;
            t.Copayment = VM.GetCopayment(Copayment?.Id);
            t.Division = VM.GetDivision(Division?.ID);
            t.Institution = Institution.DeepCloneViaJson();
            t.MainDisease = MainDisease?.DeepCloneViaJson();
            t.SubDisease = SubDisease?.DeepCloneViaJson();
            t.MedicalNumber = MedicalNumber;
            t.OriginalMedicalNumber = string.IsNullOrEmpty(OriginalMedicalNumber)?string.Empty:OriginalMedicalNumber;
            t.PaymentCategory = VM.GetPaymentCategory(PaymentCategory?.ID);
            t.SpecialTreat = VM.GetSpecialTreat(SpecialTreat?.ID);
            t.Pharmacist = VM.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(p=>p.IDNumber.Equals(Pharmacist.IDNumber));
            t.PrescriptionCase = VM.GetPrescriptionCases(PrescriptionCase?.ID);
            t.TreatDate = TreatDate;
            t.TempMedicalNumber = TempMedicalNumber;
            return t;
        }
    }
}
