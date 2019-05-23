using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow;
using Resource = His_Pos.Properties.Resources;

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
            var printPre = new Prescription(Content,TreatDate,SourceID,IsRead);
            MainWindow.ServerConnection.OpenConnection();
            printPre.GetCompletePrescriptionData(false);
            printPre.UpdateCooperativePrescriptionIsRead();
            MainWindow.ServerConnection.CloseConnection();
            printPre.CountPrescriptionPoint(true);
            var medBagPrint = new ConfirmWindow("是否列印藥袋", "列印確認", true);
            if ((bool)medBagPrint.DialogResult)
            {
                var printBySingleMode = new MedBagSelectionWindow();
                var singleMode = (bool)printBySingleMode.ShowDialog();
                var receiptPrint = false;
                if (printPre.PrescriptionPoint.AmountsPay > 0)
                {
                    var receiptResult = new ConfirmWindow(Resource.PrintReceipt, Resource.PrintConfirm, true);
                    if (receiptResult.DialogResult != null)
                        receiptPrint = (bool)receiptResult.DialogResult;
                }
                //printPre.PrintMedBag(singleMode);
                //if (receiptPrint)
                //    printPre.PrintReceipt();
            }
        }
    }
}
