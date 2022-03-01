using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    public class PaymentCategories : Collection<PaymentCategory>
    {
        public PaymentCategories()
        {
            Init();
        }

        public PaymentCategories(IList<PaymentCategory> list)
        {
            foreach (var p in list)
                Add(p);
        }

        private void Init()
        {
            Add(new PaymentCategory());
            var table = PaymentCategoryDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new PaymentCategory(row));
            }
        }
    }
}