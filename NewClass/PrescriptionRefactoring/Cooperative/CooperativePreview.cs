using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow;
using Resources = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.PrescriptionRefactoring.Cooperative
{
    public class CooperativePreview : CooperativePreBase
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
    }
}
