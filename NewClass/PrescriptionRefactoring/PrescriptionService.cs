using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.NewClass.PrescriptionRefactoring
{
    public class PrescriptionService
    {
        //public Prescription Current { get; set; }

        public static void CheckTypeByInstitution(Prescription p)
        {
            p.CheckTypeByInstitution();
        }

        public static void CheckTypeByAdjustCase(Prescription p)
        {
            p.CheckTypeByAdjustCase();
        }
    }
}
