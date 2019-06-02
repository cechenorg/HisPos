using System;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public class CooperativePreview : CusPrePreviewBase
    {
        public CooperativePreview(CooperativePrescription.Prescription c, DateTime treatDate, string sourceId, bool isRead) : base(c, treatDate, isRead)
        {
            Content = c;
            SourceID = sourceId;
        }
        public CooperativePrescription.Prescription Content { get; }
        public string SourceID { get; }
        public override void Print()
        {
            var printPre = CreatePrescription();
            printPre.PrintMedBagAndReceipt();
        }

        public override Prescription CreatePrescription()
        {
            var pre = new Prescription(Content, TreatDate, SourceID, IsRead);
            pre.UpdateCooperativePrescriptionIsRead();
            pre.CountPrescriptionPoint(true);
            return pre;
        }

        public override void GetMedicines()
        {
            
        }
    }
}
