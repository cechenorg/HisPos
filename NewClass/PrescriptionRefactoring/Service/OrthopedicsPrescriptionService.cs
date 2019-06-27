using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using His_Pos.Properties;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class OrthopedicsPrescriptionService : PrescriptionService
    {
        public OrthopedicsPrescriptionService()
        {

        }

        [SuppressMessage("ReSharper", "FlagArgument")]
        public override bool CheckPrescription(bool noCard)
        {
            CheckAnonymousPatient();
            if (!CheckRemarkEmpty()) return false;
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDate()) return false;
            if (!CheckNhiRules(noCard)) return false;
            if (!CheckPrescribeRules()) return false;
            if (!CheckMedicalNumber()) return false;
            return CheckSameDeclare() && PrintConfirm();
        }

        public override bool NormalAdjust()
        {
            if (current.PrescriptionStatus.IsCreateSign is null) return false;
            CheckCovertType();
            current.SetNormalAdjustStatus();//設定處方狀態
            current.InsertDb();
            CheckUpdateOrthopedicsStatus();
            return true;
        }

        public override void ErrorAdjust()
        {
            CheckCovertType();
            current.SetErrorAdjustStatus();
            current.InsertDb();
            CheckUpdateOrthopedicsStatus();
        }

        public override void DepositAdjust()
        {
            CheckCovertType();
            current.SetDepositAdjustStatus();
            current.InsertDb();
            CheckUpdateOrthopedicsStatus();
        }

        public override void PrescribeAdjust()
        {
            current.SetPrescribeAdjustStatus();
            current.InsertDb();
            CheckUpdateOrthopedicsStatus();
        }

        public override bool Register()
        {
            if (!CheckChronicRegister()) return false;
            CheckCovertType();
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

        private void UpdateOrthopedicsStatus()
        {
            if (!string.IsNullOrEmpty(current.SourceId))
                PrescriptionDb.UpdateOrthopedicsStatus(current.SourceId);
        }

        private bool CheckRemarkEmpty()
        {
            if (current.AdjustCase.ID != "2" && string.IsNullOrEmpty(current.Remark))
            {
                var e = new CooperativeRemarkInsertWindow();
                current.Remark = ((CooperativeRemarkInsertViesModel)e.DataContext).Remark;
                if (string.IsNullOrEmpty(current.Remark) || current.Remark.Length != 16)
                    return false;
                CheckIsCooperativeVIP();
                return true;
            }
            return true;
        }

        private void CheckIsCooperativeVIP()
        {
            var isVip = new ConfirmWindow(Resources.收部分負擔, Resources.免收確認);
            Debug.Assert(isVip.DialogResult != null, "isVip.DialogResult != null");
            current.PrescriptionStatus.IsVIP = (bool)!isVip.DialogResult;
            current.PrescriptionPoint.CopaymentPointPayable = current.PrescriptionStatus.IsVIP ? 0 : current.PrescriptionPoint.CopaymentPoint;
        }

        private void CheckCovertType()
        {
            if (current.AdjustCase.IsChronic())
                current.Type = PrescriptionType.Normal;
        }

        private void CheckUpdateOrthopedicsStatus()
        {
            if (!current.AdjustCase.IsChronic())
                UpdateOrthopedicsStatus();
        }
    }
}
