using System;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.Refactoring;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class NormalPrescriptionService : PrescriptionService
    {
        public NormalPrescriptionService()
        {
            
        }

        public override bool CheckPrescription()
        {
            CheckAnonymousPatient();
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDate()) return false;
            if (!CheckNhiRules()) return false;
            if (!CheckMedicines()) return false;
            if (!CheckMedicalNumber()) return false;
            if (!PrintConfirm()) return false;
            return true;
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
            current.SetErrorAdjustStatus();//設定處方狀態
            current.InsertDb();
        }

        public override bool DepositAdjust()
        {
            throw new NotImplementedException();
        }

        public override bool Register()
        {
            throw new NotImplementedException();
        }
    }
}
