using System;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;

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
            if (!CheckChronicRegister()) return false;
            MedicinesSendSingdeViewModel vm = null;
            if (current.PrescriptionStatus.IsSendOrder)
            {
                var medicinesSendSingdeWindow = new MedicinesSendSingdeWindow(current);
                vm = (MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext;
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn)
                    return false;
            }
            current.PrescriptionStatus.SetRegisterStatus();
            current.InsertDb();
            SendOrder(vm);
            return true;
        }
    }
}
