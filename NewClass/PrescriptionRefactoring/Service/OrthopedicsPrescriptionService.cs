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
            if (!CheckPrescribeRules()) return false;
            return CheckSameDeclare() && PrintConfirm();
        }

        public override bool NormalAdjust()
        {
            if (Current.PrescriptionStatus.IsCreateSign is null) return false;
            CheckCovertType();
            Current.SetNormalAdjustStatus();//設定處方狀態
            Current.InsertDb();
            CheckUpdateOrthopedicsStatus();
            return true;
        }

        public override void ErrorAdjust()
        {
            CheckCovertType();
            Current.SetErrorAdjustStatus();
            Current.InsertDb();
            CheckUpdateOrthopedicsStatus();
        }

        public override void DepositAdjust()
        {
            CheckCovertType();
            Current.SetDepositAdjustStatus();
            Current.InsertDb();
            CheckUpdateOrthopedicsStatus();
        }

        public override void PrescribeAdjust()
        {
            Current.SetPrescribeAdjustStatus();
            Current.InsertDb();
            CheckUpdateOrthopedicsStatus();
        }

        public override bool Register()
        {
            if (!CheckChronicRegister()) return false;
            CheckCovertType();
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

        private void UpdateOrthopedicsStatus()
        {
            if (!string.IsNullOrEmpty(Current.SourceId))
                PrescriptionDb.UpdateOrthopedicsStatus(Current.SourceId);
        }

        private bool CheckRemarkEmpty()
        {
            if (Current.AdjustCase.ID != "2" && string.IsNullOrEmpty(Current.Remark))
            {
                var e = new CooperativeRemarkInsertWindow();
                Current.Remark = ((CooperativeRemarkInsertViesModel)e.DataContext).Remark;
                if (string.IsNullOrEmpty(Current.Remark) || Current.Remark.Length != 16)
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
            Current.PrescriptionStatus.IsVIP = (bool)!isVip.DialogResult;
            Current.PrescriptionPoint.CopaymentPointPayable = Current.PrescriptionStatus.IsVIP ? 0 : Current.PrescriptionPoint.CopaymentPoint;
        }

        private void CheckCovertType()
        {
            if (Current.AdjustCase.IsChronic())
                Current.Type = PrescriptionType.Normal;
        }

        private void CheckUpdateOrthopedicsStatus()
        {
            if (!Current.AdjustCase.IsChronic())
                UpdateOrthopedicsStatus();
        }
    }
}
