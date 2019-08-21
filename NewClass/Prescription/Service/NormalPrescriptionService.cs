using System.Collections.Generic;
using System.Linq;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;

namespace His_Pos.NewClass.Prescription.Service
{
    public class NormalPrescriptionService : PrescriptionService
    {
        public NormalPrescriptionService()
        {
            
        }

        public override bool CheckPrescription(bool noCard,bool errorAdjust)
        {
            CheckAnonymousPatient();
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDate(errorAdjust)) return false;
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
            if (!CheckSameDeclare()) return false;
            if (!CheckSendOrder()) return false;
            return PrintConfirm();
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

        public override bool CheckEditPrescription(bool noCard)
        {
            CheckAnonymousPatient();
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDate(true)) return false;
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
                if(Current.PrescriptionStatus.IsSendOrder && !Current.PrescriptionStatus.OrderStatus.Equals("訂單狀態:已收貨"))
                    SendOrder(vm);
                return true;
            }
            return false;
        }
    }
}
