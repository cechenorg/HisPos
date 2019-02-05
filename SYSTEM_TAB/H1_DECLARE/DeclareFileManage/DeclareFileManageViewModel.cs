using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage
{
    public class DeclareFileManageViewModel:TabBase
    {
        #region Variables
        public override TabBase getTab()
        {
            return this;
        }
        public MedicalPersonnels MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        #endregion
        public DeclareFileManageViewModel()
        {

        }
    }
}
