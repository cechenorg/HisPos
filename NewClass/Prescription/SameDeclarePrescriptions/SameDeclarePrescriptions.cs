using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.SameDeclarePrescriptions
{
    public class SameDeclarePrescriptions : Collection<SameDeclarePrescription>
    {
        public SameDeclarePrescriptions()
        {
        }

        public void AddItems(DataTable table)
        {
            foreach (DataRow r in table.Rows)
            {
                Add(new SameDeclarePrescription(r));
            }
        }
    }
}