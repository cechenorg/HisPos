using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    public class Copayments : Collection<Copayment>
    {
        public Copayments()
        {
            Init();
        }

        public Copayments(IList<Copayment> list)
        {
            foreach (var c in list)
                Add(c);
        }

        private void Init()
        {
            var table = CopaymentDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Copayment(row));
            }
        }
    }
}