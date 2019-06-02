using His_Pos.NewClass.CooperativeInstitution;

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
            pre.CountPrescriptionPoint(true);
            return pre;
        }

        public override void GetMedicines()
        {
            throw new System.NotImplementedException();
        }
    }
}
