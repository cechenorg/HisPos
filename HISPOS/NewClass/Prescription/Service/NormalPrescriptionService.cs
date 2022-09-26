using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;

namespace His_Pos.NewClass.Prescription.Service
{
    public class NormalPrescriptionService : PrescriptionService
    {
        public NormalPrescriptionService()
        {
        }

        public override bool CheckPrescription(bool noCard, bool errorAdjust)
        {
            if (!CheckAnonymousPatient()) return false;
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
            if (!CheckMedicines()) return false;
            if (!CheckSameDeclare()) return false;
            if (!CheckSendOrder()) return false;
            return PrintConfirm();
        }

        public override bool CheckPrescriptionBeforeOrder(bool noCard, bool errorAdjust)
        {
            if (!CheckAnonymousPatient()) return false;
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
            if (!CheckMedicines()) return false;
            if (!CheckSameDeclare()) return false;
            if (!CheckSendOrder()) return false;
            return true;
        }
        public void CheckPrescriptionFromAutoRegister()
        {
            CheckSendOrderFromAutoRegister();
            PrintConfirm();
        }

        private bool CheckSendOrder()
        {
            if (string.IsNullOrEmpty(Current.PrescriptionStatus.OrderStatus))
                Current.PrescriptionStatus.OrderStatus = "訂單狀態:無訂單";
            if (Current.PrescriptionStatus.IsSendOrder && !Current.PrescriptionStatus.OrderStatus.Equals("訂單狀態:已收貨"))
            {
                var medicinesSendSingdeWindow = new MedicinesSendSingdeWindow(Current);
                vm = (MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext;
                return !((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn;
            }
            return true;
        }

        private bool CheckSendOrderFromAutoRegister()
        {
            if (string.IsNullOrEmpty(Current.PrescriptionStatus.OrderStatus))
                Current.PrescriptionStatus.OrderStatus = "訂單狀態:無訂單";
            if (Current.PrescriptionStatus.IsSendOrder && !Current.PrescriptionStatus.OrderStatus.Equals("訂單狀態:已收貨"))
            {
                var medicinesSendSingdeWindow = new MedicinesSendSingdeWindow(Current, true);
                vm = (MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext;
                return !((MedicinesSendSingdeViewModel)medicinesSendSingdeWindow.DataContext).IsReturn;
            }
            return true;
        }

        public override bool CheckEditPrescription(bool hasCard)
        {
            if (!CheckAnonymousPatient()) return false;
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDateFromEdit()) return false;
            if (Current.IsPrescribe)
            {
                if (!CheckPrescribeRules()) return false;
            }
            else
            {
                if (!CheckNhiRules(!hasCard)) return false;
                if (hasCard)
                    if (!CheckMedicalNumber()) return false;
            }
            return CheckMedicines();
        }

        public override bool NormalAdjust()
        {
            if (Current.PrescriptionStatus.IsCreateSign is null) return false;
            Current.SetNormalAdjustStatus();//設定處方狀態
            return Current.InsertDb();
        }

        public override bool ErrorAdjust()
        {
            Current.SetErrorAdjustStatus();
            return Current.InsertDb();
        }

        public override bool DepositAdjust()
        {
            Current.SetDepositAdjustStatus();
            return Current.InsertDb();
        }

        public override bool PrescribeAdjust()
        {
            Current.SetPrescribeAdjustStatus();
            return Current.InsertDb();
        }

        public override bool Register()
        {
            if (!CheckChronicRegister()) return false;
            Current.PrescriptionStatus.SetRegisterStatus();
            if (Current.InsertDb())
            {
                if (Current.PrescriptionStatus.IsSendOrder && !Current.PrescriptionStatus.OrderStatus.Equals("訂單狀態:已收貨"))
                    SendOrder(vm);
                return true;
            }
            return false;
        }

        public override bool CheckCustomerSelected()
        {
            if (!CheckAnonymousPatient()) return false;
            if (!CheckValidCustomer()) return false;
            return true;
        }
    }
}