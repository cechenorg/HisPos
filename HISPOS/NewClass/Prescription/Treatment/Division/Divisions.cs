using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public class Divisions : Collection<Division>
    {
        public Divisions()
        {
            Init();
        }

        public Divisions(IList<Division> list)
        {
            foreach (var d in list)
                Add(d);
        }

        private void Init()
        {
            Add(new Division());
            var table = DivisionDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Division(row));
            }
        }
    }
}