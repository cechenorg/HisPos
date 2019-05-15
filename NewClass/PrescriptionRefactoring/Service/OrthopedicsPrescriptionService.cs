using System;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class OrthopedicsPrescriptionService : PrescriptionService
    {
        public OrthopedicsPrescriptionService()
        {

        }

        public override bool NormalAdjust()
        {
            throw new NotImplementedException();
        }

        public override bool ErrorAdjust()
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
