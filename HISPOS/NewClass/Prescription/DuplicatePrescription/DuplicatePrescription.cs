using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System;
using System.Data;
using His_Pos.InfraStructure;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

// ReSharper disable All

namespace His_Pos.NewClass.Prescription.DuplicatePrescription
{
    public class DuplicatePrescription : ObservableObject
    {
        private int id;

        public int ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
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

        private string patientData;

        public string PatientData
        {
            get => patientData;
            set
            {
                Set(() => PatientData, ref patientData, value);
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

        private DateTime treatDate;

        public DateTime TreatDate
        {
            get => treatDate;
            set
            {
                Set(() => TreatDate, ref treatDate, value);
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

        private int? chronicTotal;

        private int? chronicSequence;

        public string ChronicContent
        {
            get => chronicTotal is null ? string.Empty : chronicTotal + " - " + chronicSequence;
        }

        public DuplicatePrescription(DataRow r)
        {
            ID = r.Field<int>("PreMas_ID");
            Patient = CustomerService.GetCustomerByCusId(r.Field<int>("CustomerID"));
            PatientData = "姓名:" + r.Field<string>("CustomerName") + " 身分證:" + r.Field<string>("CustomerIdNumber");
            Institution = VM.GetInstitution(r.Field<string>("InstitutionID"));
            Division = VM.GetDivision(r.Field<string>("DivisionID"));
            TreatDate = r.Field<DateTime>("TreatmentDate");
            AdjustDate = r.Field<DateTime>("AdjustDate");
            chronicTotal = r.Field<byte?>("ChronicTotal");
            chronicSequence = r.Field<byte?>("ChronicSequence");
        }
    }
}