using System;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.Refactoring;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class OrthopedicsPrescriptionService : PrescriptionService
    {
        public OrthopedicsPrescriptionService()
        {

        }

        public override bool CheckPrescription()
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

        public override bool DepositAdjust()
        {
            throw new NotImplementedException();
        }

        public override bool Register()
        {
            MessageWindow.ShowMessage("合作骨科診所處方不可登錄",MessageType.ERROR);
            return false;
        }
    }
}
