using System;
using System.Diagnostics;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using His_Pos.Properties;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.Refactoring;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class OrthopedicsPrescriptionService : PrescriptionService
    {
        public OrthopedicsPrescriptionService()
        {

        }

        public override bool CheckPrescription(bool noCard)
        {
            CheckAnonymousPatient();
            if (!CheckMissingCooperativeContinue()) return false;
            if (!CheckValidCustomer()) return false;
            if (!CheckAdjustAndTreatDate()) return false;
            if (!CheckNhiRules(noCard)) return false;
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
            UpdateOrthopedicsStatus();
            return true;
        }

        public override void ErrorAdjust()
        {
            current.SetErrorAdjustStatus();
            current.InsertDb();
            UpdateOrthopedicsStatus();
        }

        public override void DepositAdjust()
        {
            current.SetDepositAdjustStatus();
            current.InsertDb();
            UpdateOrthopedicsStatus();
        }

        public override bool Register()
        {
            if (!current.AdjustCase.IsChronic())
            {
                MessageWindow.ShowMessage("一般箋處方不可登錄", MessageType.ERROR);
                return false;
            }

            return true;
        }

        private void UpdateOrthopedicsStatus()
        {
            if (!string.IsNullOrEmpty(current.SourceId))
                PrescriptionDb.UpdateOrthopedicsStatus(current.SourceId);
        }

        private bool CheckMissingCooperativeContinue()
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
        }
    }
}
