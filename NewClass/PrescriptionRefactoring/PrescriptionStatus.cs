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
        public bool IsPrescribe { get; set; }

        #endregion
    }
}
