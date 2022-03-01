using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    public class AdjustCases : Collection<AdjustCase>
    {
        public AdjustCases(bool init)
        {
            if (init) Init();
        }

        public AdjustCases(IList<AdjustCase> list)
        {
            foreach (var a in list)
                Add(a);
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