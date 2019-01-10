using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    public class SpecialTreats:Collection<SpecialTreat>
    {
        public SpecialTreats()
        {
            Init();
        }

        private void Init()
        {
            var table = SpecialTreatDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new SpecialTreat(row));
            }
        }
    }
}
