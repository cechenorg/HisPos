using His_Pos.FunctionWindow;
using His_Pos.Properties;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace His_Pos.NewClass.Prescription.Service
{
    public class OrthopedicsPrescriptionService : PrescriptionService
    {
        public OrthopedicsPrescriptionService()
        {
        }

        [SuppressMessage("ReSharper", "FlagArgument")]
        public override bool CheckPrescription(bool noCard, bool errorAdjust)
        {
            if (!CheckAnonymousPatient()) return false;
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
        public override bool CheckPrescriptionBeforeOrder(bool noCard, bool errorAdjust)
        {
            if (!CheckAnonymousPatient()) return false;
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
            return CheckPrescribeRules();
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

        public override bool ErrorAdjust()
        {
            CheckCovertType();
            Current.SetErrorAdjustStatus();
            if (Current.InsertDb())
            {
                CheckUpdateOrthopedicsStatus();
                return true;
            }
            return false;
        }

        public override bool DepositAdjust()
        {
            CheckCovertType();
            Current.SetDepositAdjustStatus();
            if (Current.InsertDb())
            {
                CheckUpdateOrthopedicsStatus();
                return true;
            }
            return false;
        }

        public override bool PrescribeAdjust()
        {
            Current.SetPrescribeAdjustStatus();
            if (Current.InsertDb())
            {
                CheckUpdateOrthopedicsStatus();
                return true;
            }
            return false;
        }

        public override bool Register()
        {
            if (!CheckChronicRegister()) return false;
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
                if (string.IsNullOrEmpty(Current.Remark) || Current.Remark.Trim().Length != 16)
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

        public override bool CheckCustomerSelected()
        {
            if (!CheckAnonymousPatient()) return false;
            if (!CheckValidCustomer()) return false;
            return true;
        }
    }
}