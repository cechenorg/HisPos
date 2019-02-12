using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePrescription
{
    public class DeclarePrescription:ObservableObject
    {
        public DeclarePrescription()
        {

        }

        public DeclarePrescription(DataRow r)
        {
            ID = r.Field<int>("PreMas_ID");
            IsDeclare = r.Field<bool>("PreMas_IsDeclare");
            Patient.GetCustomerByCusId(r.Field<int>("PreMas_CustomerID"));
            Institution = ViewModelMainWindow.GetInstitution(r.Field<string>("PreMas_InstitutionID"));
            Division = ViewModelMainWindow.GetDivision(r.Field<string>("PreMas_DivisionID"));
            AdjustDate = r.Field<DateTime>("PreMas_AdjustDate");
            Pharmacist = ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels.SingleOrDefault(p => p.IdNumber.Equals(r.Field<string>("PreMas_PharmacistIDNumber")));
            MedicinePoint = r.Field<int>("PreMas_MedicinePoint");
            MedicalServicePoint = r.Field<int>("PreMas_MedicalServicePoint");
            TotalPoint = r.Field<int>("PreMas_TotalPoint");
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
    }
}
