using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    public class PaymentCategories:Collection<PaymentCategory>
    {
        public PaymentCategories()
        {
            Init();
        }

        private void Init()
        {
            var table = PaymentCategoryDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new PaymentCategory(row));
            }
        }
    }
}
