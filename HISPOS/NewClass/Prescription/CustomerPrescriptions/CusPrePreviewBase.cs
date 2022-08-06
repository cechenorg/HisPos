using GalaSoft.MvvmLight;
using His_Pos.NewClass.Cooperative.CooperativeInstitution;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Medicine.PreviewMedicine;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System;
using System.Data;
using Customer = His_Pos.NewClass.Person.Customer.Customer;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription.CustomerPrescriptions
{
    public abstract class CusPrePreviewBase : ObservableObject
    {
        public CusPrePreviewBase()
        {
        }

        protected CusPrePreviewBase(OrthopedicsPrescription c)
        {
            #region CooPreVariable

            var prescription = c.DeclareXmlDocument.Prescription;
            var study = prescription.Study;
            var customer = prescription.CustomerProfile.Customer;
            var birthYear = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1911 : int.Parse(customer.Birth.Substring(0, 3)) + 1911;
            var birthMonth = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(3, 2));
            var birthDay = string.IsNullOrEmpty(customer.Birth.Trim()) ? 1 : int.Parse(customer.Birth.Substring(5, 2));

            #endregion CooPreVariable

            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            TreatDate = Convert.ToDateTime(c.InsertDate);
            IsRead = c.IsRead?.Equals("D") ?? false;
            Medicines = new PreviewMedicines();
            Medicines.AddItemsFromOrthopedics(prescription.MedicineOrder.Item);
        }

        protected CusPrePreviewBase(CooperativePrescription.Prescription c, DateTime treatDate, bool isRead)
        {
            #region CooPreVariable

            var prescription = c;
            var customer = prescription.CustomerProfile.Customer;
            var study = prescription.Study;
            var cusBirth = customer.Birth.Trim();
            int birthYear = 1911, birthMonth = 1, birthDay = 1;
            if (cusBirth.Length >= 7)
            {
                birthYear = string.IsNullOrEmpty(cusBirth) ? 1911 : int.Parse(cusBirth.Substring(0, 3)) + 1911;
                birthMonth = string.IsNullOrEmpty(cusBirth) ? 1 : int.Parse(cusBirth.Substring(3, 2));
                birthDay = string.IsNullOrEmpty(cusBirth) ? 1 : int.Parse(cusBirth.Substring(5, 2));
            }

            #endregion CooPreVariable

            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            TreatDate = treatDate.Date;
            IsRead = isRead;
            Medicines = new PreviewMedicines();
            Medicines.AddItemsFromCooperative(prescription.MedicineOrder.Item);
           
        }

        protected CusPrePreviewBase(CooperativePrescription.Prescription c, DateTime treatDate, bool isRead, bool isPrint)
        {
            #region CooPreVariable

            var prescription = c;
            var customer = prescription.CustomerProfile.Customer;
            var study = prescription.Study;
            var cusBirth = customer.Birth.Trim();
            int birthYear = 1911, birthMonth = 1, birthDay = 1;
            if (cusBirth.Length >= 7)
            {
                birthYear = string.IsNullOrEmpty(cusBirth) ? 1911 : int.Parse(cusBirth.Substring(0, 3)) + 1911;
                birthMonth = string.IsNullOrEmpty(cusBirth) ? 1 : int.Parse(cusBirth.Substring(3, 2));
                birthDay = string.IsNullOrEmpty(cusBirth) ? 1 : int.Parse(cusBirth.Substring(5, 2));
            }

            #endregion CooPreVariable

            Patient = new Customer(customer, birthYear, birthMonth, birthDay);
            Institution = VM.GetInstitution(prescription.From);
            Division = VM.GetDivision(study.Subject);
            TreatDate = treatDate.Date;
            IsRead = isRead;
            Medicines = new PreviewMedicines();
            Medicines.AddItemsFromCooperative(prescription.MedicineOrder.Item);
            IsPrint = isPrint;
            if (IsPrint == true) {
                IsPrintString = "已自動列印";
            }
            else {
                IsPrintString = "";
            }

        }

        protected CusPrePreviewBase(DataRow r)
        {
            Institution = VM.GetInstitution(r.Field<string>("Ins_ID"));
            Division = VM.GetDivision(r.Field<string>("Div_ID"));
            TreatDate = r.Field<DateTime>("Tre_Date");
            Medicines = new PreviewMedicines();
        }

        public Institution Institution { get; }
        public Division Division { get; }
        public Customer Patient { get; }
        public PreviewMedicines Medicines { get; set; }
        public DateTime TreatDate { get; }
        public DateTime AdjustDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsPrint { get; set; }
        public string IsPrintString { get; set; }
        public abstract void Print();
        public abstract void PrintDir();
        public abstract Prescription CreatePrescription();

        public abstract void GetMedicines();
        public bool IsVIP { get; set; }
    }
}