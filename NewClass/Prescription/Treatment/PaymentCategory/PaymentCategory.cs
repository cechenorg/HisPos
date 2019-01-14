using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    public class PaymentCategory : ObservableObject
    {
        public PaymentCategory() { }

        public PaymentCategory(DataRow r)
        {
            Id = r.Field<string>("PayCat_ID");
            Name = r.Field<string>("PayCat_Name");
            FullName = r.Field<string>("PayCat_FullName");
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
