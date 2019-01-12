using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    public class PaymentCategory : ObservableObject
    {
        public PaymentCategory() { }

        public PaymentCategory(DataRow r)
        {
            Id = r[""].ToString();
            Name = r["PAYENT_NAME"].ToString();
            FullName = r[""].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
