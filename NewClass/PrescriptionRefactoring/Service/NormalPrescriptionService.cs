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
            if (Current.IsPrescribe)
            {
                if (!CheckPrescribeRules()) return false;
            }
            else
            {
                if (!CheckNhiRules(noCard)) return false;
                if(!noCard)
                    if (!CheckMedicalNumber()) return false;
            }
            if (!CheckMedicines()) return false;
            return CheckSameDeclare() && PrintConfirm();
        }

        public override bool CheckEditPrescription(bool noCard)
        {
            CheckAnonymousPatient();
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDate()) return false;
            if (Current.IsPrescribe)
            {
                if (!CheckPrescribeRules()) return false;
            }
            else
            {
                if (!CheckNhiRules(noCard)) return false;
                if (!noCard)
                    if (!CheckMedicalNumber()) return false;
            }
            return CheckMedicines();
        }

        public override bool NormalAdjust()
        {
            if (Current.PrescriptionStatus.IsCreateSign is null) return false;
            Current.SetNormalAdjustStatus();//設定處方狀態
            Current.InsertDb();
            return true;
        }

        public override void ErrorAdjust()
        {
            Current.SetErrorAdjustStatus();
            Current.InsertDb();
        }

        public override void DepositAdjust()
        {
            Current.SetDepositAdjustStatus();
            Current.InsertDb();
        }

        public override void PrescribeAdjust()
        {
            Current.SetPrescribeAdjustStatus();
            Current.InsertDb();
        }

        public override bool Register()
        {
            if (!CheckChronicRegister()) return false;
            MedicinesSendSingdeViewModel vm = null;
            if (Current.PrescriptionStatus.IsSendOrder)
            {
                var medicinesSendSingdeWindow = new MedicinesSendSingdeWindow(Current);
                vm = (MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext;
                if (((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn)
                    return false;
            }
            Current.PrescriptionStatus.SetRegisterStatus();
            Current.InsertDb();
            SendOrder(vm);
            return true;
        }
    }
}
