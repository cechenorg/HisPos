using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    public class PrescriptionCases:Collection<PrescriptionCase>
    {
        public PrescriptionCases()
        {
            Init();
        }

        private void Init()
        {
            var table = PrescriptionCaseDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new PrescriptionCase(row));
            }
        }
    }
}
