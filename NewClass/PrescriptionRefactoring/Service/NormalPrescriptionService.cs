using System;

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
            //列印確認
            return true;
        }

        public override bool NormalAdjust()
        {
            if (current.PrescriptionStatus.IsCreateSign is null) return false;
            SavePatientData();
            current.SetAdjustStatus();//設定處方狀態
            current.InsertDb();
            return true;
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
            throw new NotImplementedException();
        }
    }
}
