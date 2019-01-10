using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
