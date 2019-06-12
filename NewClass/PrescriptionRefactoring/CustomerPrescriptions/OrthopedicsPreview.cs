using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Product.Medicine.PreviewMedicine;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public class OrthopedicsPreview : CusPrePreviewBase
    {
        public OrthopedicsPreview(OrthopedicsPrescription c) :base(c)
        {
            Content = c;
            DoctorName = c.DeclareXmlDocument.Prescription.Study.Doctor_Name;
        }
        public OrthopedicsPrescription Content { get;}
        public string DoctorName { get; }
        public override void Print()
        {
            var printPre = CreatePrescription();
            printPre.PrintMedBagAndReceipt();
        }
        public override Prescription CreatePrescription()
        {
            var pre = new Prescription(Content);
            pre.UpdateCooperativePrescriptionIsRead();
            pre.CountPrescriptionPoint();
            return pre;
        }

        public override void GetMedicines()
        {
            Medicines.Clear();
            var prescription = Content.DeclareXmlDocument.Prescription;
            foreach (var med in prescription.MedicineOrder.Item)
            {
                Medicines.Add(new PreviewMedicine(med));
            }
        }
    }
}
