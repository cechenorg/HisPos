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
            if (!CheckMedicines()) return false;
            if (!CheckMedicalNumber()) return false;
            //自費不檢查健保規則
            return true;
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
            throw new NotImplementedException();
        }
    }
}
