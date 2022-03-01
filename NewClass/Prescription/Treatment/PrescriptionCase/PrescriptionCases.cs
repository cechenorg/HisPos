using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    public class PrescriptionCases : Collection<PrescriptionCase>
    {
        public PrescriptionCases()
        {
            Init();
        }

        public PrescriptionCases(IList<PrescriptionCase> list)
        {
            foreach (var p in list)
                Add(p);
        }

        private void Init()
        {
            Add(new PrescriptionCase());
            var table = PrescriptionCaseDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new PrescriptionCase(row));
            }
        }
    }
}