using System;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.NewClass.Product.Medicine.PreviewMedicine;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.PrescriptionRefactoring.Cooperative
{
    public abstract class CooperativePreBase
    {
        public CooperativePreBase()
        {

        }

        protected CooperativePreBase(OrthopedicsPrescription c)
        {
            #region CooPreVariable
            var prescription = c.DeclareXmlDocument.Prescription;
            var study = prescription.Study;
            var customer = prescription.CustomerProfile.Customer;
            var birthYear = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1911 : int.Parse(customer.Birth.Substring(0, 3)) + 1911;
            var birthMonth = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(3, 2));
            var birthDay = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(5, 2));
            #endregion 
            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            TreatDate = Convert.ToDateTime(c.InsertDate);
            IsRead = c.IsRead?.Equals("D") ?? false;
            Medicines = new PreviewMedicines();
            Medicines.AddItemsFromOrthopedics(prescription.MedicineOrder.Item);
        }

        protected CooperativePreBase(CooperativePrescription.Prescription c, DateTime treatDate, bool isRead)
        {
            #region CooPreVariable
            var prescription = c;
            var customer = prescription.CustomerProfile.Customer;
            var study = prescription.Study;
            var birthYear = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1911 : int.Parse(customer.Birth.Substring(0, 3)) + 1911;
            var birthMonth = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(3, 2));
            var birthDay = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(5, 2));
            #endregion
            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            TreatDate = treatDate.Date;
            IsRead = isRead;
            Medicines = new PreviewMedicines();
            Medicines.AddItemsFromCooperative(prescription.MedicineOrder.Item);
        }
        public Institution Institution { get; }
        public Division Division { get; }
        public Customer Patient { get; }
        public PreviewMedicines Medicines { get; set; }
        public DateTime TreatDate { get; }
        public bool IsRead { get; set; }
    }
}
