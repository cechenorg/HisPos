using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass
{
    public class PaymentCategory
    {
        public PaymentCategory() { }

        public PaymentCategory(DataRow r)
        {
            Id = r["PAYMENT_ID"].ToString();
            Name = r["PAYMENT_NAME"].ToString();
            FullName = r["PAYMENT_FULLNAME"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
