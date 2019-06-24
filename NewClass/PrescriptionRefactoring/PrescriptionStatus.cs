using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
