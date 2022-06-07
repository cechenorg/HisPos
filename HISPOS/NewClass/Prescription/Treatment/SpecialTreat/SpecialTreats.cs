using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    public class SpecialTreats : Collection<SpecialTreat>
    {
        public SpecialTreats()
        {
            Init();
        }

        public SpecialTreats(IList<SpecialTreat> list)
        {
            foreach (var s in list)
                Add(s);
        }

        private void Init()
        {
            Add(new SpecialTreat());
            var table = SpecialTreatDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new SpecialTreat(row));
            }
        }
    }
}