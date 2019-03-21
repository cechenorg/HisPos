using System;
using System.Data;
using System.Globalization;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.NewClass.Prescription.Search
{
    public class PrescriptionSearchPreview:ObservableObject
    {
        public PrescriptionSearchPreview() { }

        public PrescriptionSearchPreview(DataRow r,PrescriptionSource s)
        {
            Source = s;
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
            if (s == PrescriptionSource.Normal)
            {
                IsAdjust = r.Field<bool>("IsAdjust");
                TaiwanCalendar tc = new TaiwanCalendar();
                if (r.Field<DateTime?>("InsertTime") != null) {
                    DateTime istime = r.Field<DateTime>("InsertTime");
                    InsertDate = string.Format("{0}-{1}", tc.GetYear(istime),istime.ToString("MM-dd HH點mm分"));
                         
                } 
                
                switch (r.Field<string>("StoOrd_Status")) {
                    case "W":
                        StoStatus = "等待確認";
                        break;
                    case "P":
                        StoStatus = "等待收貨";
                        break;
                    case "D":
                        StoStatus = "已收貨";
                        break;
                    default:
                        StoStatus = "無訂單";
                        break;
                }
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
        private string stoStatus;
        public string StoStatus
        {
            get => stoStatus;
            set
            {
                Set(() => StoStatus, ref stoStatus, value);
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
        public PrescriptionSource Source { get; set; }
        public Prescription GetPrescriptionByID()
        {
            return new Prescription(PrescriptionDb.GetPrescriptionByID(ID).Rows[0],PrescriptionSource.Normal);
        }

        public Prescription GetReservePrescriptionByID()
        {
            return new Prescription(PrescriptionDb.GetReservePrescriptionByID(ID).Rows[0], PrescriptionSource.ChronicReserve);
        }
    }
}
