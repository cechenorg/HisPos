using System;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
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
