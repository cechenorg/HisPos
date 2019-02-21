using System;
using System.Data;
using System.Linq;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePrescription
{
    public class DeclarePrescription:ObservableObject
    {
        public DeclarePrescription()
        {

        }

        public DeclarePrescription(DataRow r)
        {
            Patient = new Customer();
            ID = r.Field<int>("PreMas_ID");
            IsDeclare = r.Field<bool>("PreMas_IsDeclare");
            Patient.ID = r.Field<int>("PreMas_CustomerID");
            Patient.Name = r.Field<string>("Cus_Name");
            Institution = new Institution();
            Institution.ID = r.Field<string>("PreMas_InstitutionID");
            Institution.Name = r.Field<string>("Ins_Name");
            Institution.FullName = Institution.ID + " " + Institution.Name;
            Division = new Division();
            Division.ID = r.Field<string>("PreMas_DivisionID");
            Division.Name = r.Field<string>("Div_Name");
            Division.FullName = Division.ID + " " + Division.Name;
            AdjustDate = r.Field<DateTime>("PreMas_AdjustDate");
            Pharmacist = new MedicalPersonnel();
            Pharmacist.ID = r.Field<int>("Emp_ID");
            Pharmacist.IdNumber = r.Field<string>("PreMas_PharmacistIDNumber");
            Pharmacist.Name = r.Field<string>("Emp_Name");
            MedicinePoint = r.Field<int>("PreMas_MedicinePoint");
            MedicalServicePoint = r.Field<int>("PreMas_MedicalServicePoint");
            TotalPoint = r.Field<int>("PreMas_TotalPoint");
            PharmacyID = r.Field<string>("PreMas_PharmacyID");
            AdjustCase = new AdjustCase();
            AdjustCase.ID = r.Field<string>("PreMas_AdjustCaseID");
            AdjustCase.Name = r.Field<string>("Adj_Name");
            AdjustCase.FullName = AdjustCase.ID + " " + AdjustCase.Name;
            IsGetCard = r.Field<bool>("PreMas_IsGetCard");
            FileContentStr = r.Field<string>("PreMas_DeclareContent");
        }
        public int ID { get; }
        private bool isDeclare;

        public bool IsDeclare
        {
            get => isDeclare;
            set
            {
                Set(() => IsDeclare, ref isDeclare, value);
            }
        }
        public bool IsGetCard { get; set; }
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
        private DateTime adjustDate;

        public DateTime AdjustDate
        {
            get => adjustDate;
            set
            {
                Set(() => AdjustDate, ref adjustDate, value);
            }
        }
        private MedicalPersonnel pharmacist;

        public MedicalPersonnel Pharmacist
        {
            get => pharmacist;
            set
            {
                Set(() => Pharmacist, ref pharmacist, value);
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
        private int medicalServicePoint;

        public int MedicalServicePoint
        {
            get => medicalServicePoint;
            set
            {
                Set(() => MedicalServicePoint, ref medicalServicePoint, value);
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
        private string pharmacyID;
        public string PharmacyID
        {
            get => pharmacyID;
            set
            {
                Set(() => PharmacyID, ref pharmacyID, value);
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
        public Ddata FileContent { get; set; }
        public string FileContentStr { get; set; }
    }
}
