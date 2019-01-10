using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public class Divisions:Collection<Division>
    {
        public Divisions()
        {
            Init();
        }

        private void Init()
        {
            var table = DivisionDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Division(row));
            }
        }
    }
}
