using System;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.Refactoring;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class CooperativePrescriptionService : PrescriptionService
    {
        public CooperativePrescriptionService()
        {
            
        }

        public override bool CheckPrescription(bool noCard)
        {
            throw new NotImplementedException();
        }

        public override bool NormalAdjust()
        {
            throw new NotImplementedException();
        }

        public override void ErrorAdjust()
        {
            throw new NotImplementedException();
        }

        public override void DepositAdjust()
        {
            current.SetDepositAdjustStatus();
            current.InsertDb();
        }

        public override bool Register()
        {
            if (!current.AdjustCase.IsChronic())
            {
                MessageWindow.ShowMessage("一般箋處方不可登錄", MessageType.ERROR);
                return false;
            }

            return true;
        }
    }
}
