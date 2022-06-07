using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Service;
using System;
using System.Data;
using System.Globalization;

namespace His_Pos.NewClass.Prescription.Search
{
    public class PrescriptionSearchPreview : ObservableObject
    {
        public PrescriptionSearchPreview()
        {
        }

        public PrescriptionSearchPreview(DataRow r, PrescriptionType s)
        {
            Type = s;
            ID = r.Field<int>("ID");
            Patient = new Customer();
            Patient.Name = r.Field<string>("Cus_Name");
            Patient.IDNumber = r.Field<string>("Cus_IDNumber");
            Patient.Birthday = r.Field<DateTime?>("Cus_Birthday");
            Institution = ViewModelMainWindow.GetInstitution(r.Field<string>("InstitutionID"));
            Division = ViewModelMainWindow.GetDivision(r.Field<string>("DivisionID"));
            AdjustCase = ViewModelMainWindow.GetAdjustCase(r.Field<string>("AdjustCaseID"));
            AdjustDate = r.Field<DateTime>("AdjustDate");
            TreatDate = r.Field<DateTime?>("TreatmentDate");
            MedicalNumber = r.Field<string>("MedicalNumber");
            TotalPoint = r.Field<int>("TotalPoint");
            MedicalServicePoint = r.Field<int>("MedicalServicePoint");
            MedicinePoint = r.Field<int>("MedicinePoint");
            CopaymentPoint = r.Field<int>("CopaymentPoint");
            SpecialMaterialPoint = r.Field<int>("SpecialMaterialPoint");
            if (NewFunction.CheckDataRowContainsColumn(r, "NoBuckleStatus"))
            {
                NoBuckleStatus = r.Field<int?>("NoBuckleStatus");
            }
            if (NewFunction.CheckDataRowContainsColumn(r, "RegisterTime"))
            {
                TaiwanCalendar tc = new TaiwanCalendar();
                if (r.Field<DateTime?>("RegisterTime") != null)
                {
                    var istime = r.Field<DateTime>("RegisterTime");
                    RegisterDate = $"{tc.GetYear(istime)}/{istime:MM/dd HH:mm}";
                }
            }
            if (s == PrescriptionType.Normal)
            {
                IsAdjust = r.Field<bool>("IsAdjust");
                IsDeposit = r.Field<bool>("IsDeposit");
                var tc = new TaiwanCalendar();
                if (r.Field<DateTime?>("InsertTime") != null)
                {
                    var istime = r.Field<DateTime>("InsertTime");
                    InsertDate = $"{tc.GetYear(istime)}/{istime:MM/dd HH:mm}";
                }

                switch (r.Field<string>("StoOrd_Status"))
                {
                    case "W":
                        StoStatus = "等待確認";
                        break;

                    case "P":
                        StoStatus = "等待收貨";
                        break;

                    case "D":
                        StoStatus = "已收貨";
                        break;

                    case "S":
                        StoStatus = "訂單做廢";
                        break;

                    default:
                        StoStatus = "無訂單";
                        break;
                }
            }

            if (string.IsNullOrEmpty(InsertDate) && !string.IsNullOrEmpty(RegisterDate))
            {
                Type = PrescriptionType.ChronicRegister;
            }
        }

        private Customer patient;

        public Customer Patient
        {
            get => patient;
            set
            {
                Set(() => Patient, ref patient, value);
            }
        }

        private Institution institution;

        public Institution Institution
        {
            get => institution;
            set
            {
                Set(() => Institution, ref institution, value);
            }
        }

        private Division division;

        public Division Division
        {
            get => division;
            set
            {
                Set(() => Division, ref division, value);
            }
        }

        private AdjustCase adjustCase;

        public AdjustCase AdjustCase
        {
            get => adjustCase;
            set
            {
                Set(() => AdjustCase, ref adjustCase, value);
            }
        }

        private DateTime adjustDate;

        public DateTime AdjustDate
        {
            get => adjustDate;
            set
            {
                Set(() => AdjustDate, ref adjustDate, value);
            }
        }

        private DateTime? treatDate;

        public DateTime? TreatDate
        {
            get => treatDate;
            set
            {
                Set(() => TreatDate, ref treatDate, value);
            }
        }

        private int id;

        public int ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }

        private bool isAdjust;

        public bool IsAdjust
        {
            get => isAdjust;
            set
            {
                Set(() => IsAdjust, ref isAdjust, value);
            }
        }

        private bool isDeposit;

        public bool IsDeposit
        {
            get => isDeposit;
            set
            {
                Set(() => IsDeposit, ref isDeposit, value);
            }
        }

        private int? noBuckleStatus;

        public int? NoBuckleStatus
        {
            get => noBuckleStatus;
            set
            {
                Set(() => NoBuckleStatus, ref noBuckleStatus, value);
            }
        }

        private string stoStatus;

        public string StoStatus
        {
            get => stoStatus;
            set
            {
                Set(() => StoStatus, ref stoStatus, value);
            }
        }

        private string medicalNumber;

        public string MedicalNumber
        {
            get => medicalNumber;
            set
            {
                Set(() => MedicalNumber, ref medicalNumber, value);
            }
        }

        private string insertDate;

        public string InsertDate
        {
            get => insertDate;
            set
            {
                Set(() => InsertDate, ref insertDate, value);
            }
        }

        private string registerDate;

        public string RegisterDate
        {
            get => registerDate;
            set
            {
                Set(() => RegisterDate, ref registerDate, value);
            }
        }

        private int totalPoint;

        public int TotalPoint
        {
            get => totalPoint;
            set
            {
                Set(() => TotalPoint, ref totalPoint, value);
            }
        }

        private int medicalServicePoint;

        public int MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
            }
        }

        private int medicinePoint;

        public int MedicinePoint
        {
            get => medicinePoint;
            set
            {
                Set(() => MedicinePoint, ref medicinePoint, value);
            }
        }

        private int copaymentPoint;

        public int CopaymentPoint
        {
            get => copaymentPoint;
            set
            {
                Set(() => CopaymentPoint, ref copaymentPoint, value);
            }
        }

        private int specialMaterialPoint;

        public int SpecialMaterialPoint
        {
            get => specialMaterialPoint;
            set
            {
                Set(() => SpecialMaterialPoint, ref specialMaterialPoint, value);
            }
        }

        public PrescriptionType Type { get; set; }
    }
}