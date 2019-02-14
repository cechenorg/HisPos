using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    public class Copayments:Collection<Copayment>
    {
        public Copayments()
        {
            Init();
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
