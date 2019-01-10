using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    public class AdjustCases : Collection<AdjustCase>
    {
        public AdjustCases()
        {
            Init();
        }

        private void Init()
        {
            var table = AdjustCaseDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new AdjustCase(row));
            }
        }
    }
}
