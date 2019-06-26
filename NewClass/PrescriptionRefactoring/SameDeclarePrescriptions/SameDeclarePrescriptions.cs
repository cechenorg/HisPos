using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.PrescriptionRefactoring.SameDeclarePrescriptions
{
    public class SameDeclarePrescriptions : Collection<SameDeclarePrescription>
    {
        public SameDeclarePrescriptions() { }

        public void AddItems(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Add(new SameDeclarePrescription(r));
            }
        }
    }
}
