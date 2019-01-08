using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment
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
