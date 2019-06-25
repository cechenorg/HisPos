﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Prescription;

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public class PrescriptionStatus
    {
        

        public PrescriptionStatus()
        {
        }

        public PrescriptionStatus(DataRow r)
        {

        }

        #region Properties
        public bool IsSendToSingde { get; set; }
        public bool IsAdjust { get; set; }
        public bool IsRead { get; set; }
        public bool IsVIP { get; set; }
        public bool IsGetCard { get; set; }
        public bool IsDeclare { get; set; }
        public bool IsDeposit { get; set; }
        public bool IsRegister { get; set; }
        public bool? IsCreateSign { get; set; }
        public bool IsSendOrder { get; set; }

        #endregion

        public void SetPrescribeStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = false;
            IsDeposit = false;
        }

        public void SetErrorAdjustStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = false;
            IsDeposit = false;
        }

        public void SetNormalAdjustStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = true;
            IsDeposit = false;
        }

        public void Init()
        {
            IsGetCard = false;
            IsRead = false;
            IsAdjust = false;
            IsSendOrder = false;
            IsSendToSingde = false;
            IsRegister = false;
            IsDeclare = true;
            IsVIP = false;
            IsCreateSign = null;
            IsDeposit = false;
        }

        public void SetDepositAdjustStatus()
        {
            IsGetCard = false;
            IsDeposit = true;
            IsDeclare = false;
            IsAdjust = true;
            IsSendOrder = false;
            IsSendToSingde = false;
            IsRegister = false;
            IsCreateSign = null;
        }

        public void SetRegisterStatus()
        {
            IsGetCard = false;
            IsAdjust = false;
            IsDeclare = false;
            IsDeposit = false;
        }

        public void UpdateStatus(int currentID)
        {
            PrescriptionDb.UpdatePrescriptionStatus(this, currentID);
        }
    }
}
