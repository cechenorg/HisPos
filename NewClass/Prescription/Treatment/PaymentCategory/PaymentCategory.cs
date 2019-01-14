using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    public class PaymentCategory : ObservableObject
    {
        public PaymentCategory() { }

        public PaymentCategory(DataRow r)
        {
            Id = r["PayCat_ID"].ToString();
            Name = r["PayCat_Name"].ToString();
            FullName = r["PayCat_FullName"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
