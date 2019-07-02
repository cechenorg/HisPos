using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class NormalPrescriptionService : PrescriptionService
    {
        public NormalPrescriptionService()
        {
            
        }

        public override bool CheckPrescription(bool noCard)
        {
            CheckAnonymousPatient();
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDate()) return false;
            if (current.IsPrescribe)
            {
                if (!CheckPrescribeRules()) return false;
            }
            else
            {
                if (!CheckNhiRules(noCard)) return false;
                if (!CheckMedicalNumber(noCard)) return false;
            }
            if (!CheckMedicines()) return false;
            return CheckSameDeclare() && PrintConfirm();
        }

        public override bool NormalAdjust()
        {
            if (current.PrescriptionStatus.IsCreateSign is null) return false;
            current.SetNormalAdjustStatus();//設定處方狀態
            current.InsertDb();
            return true;
        }

        public override void ErrorAdjust()
        {
            current.SetErrorAdjustStatus();
            current.InsertDb();
        }

        public override void DepositAdjust()
        {
            current.SetDepositAdjustStatus();
            current.InsertDb();
        }

        public override void PrescribeAdjust()
        {
            current.SetPrescribeAdjustStatus();
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
