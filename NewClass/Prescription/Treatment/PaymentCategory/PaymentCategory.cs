using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    [ZeroFormattable]
    public class PaymentCategory : ObservableObject
    {
        public PaymentCategory() {
            ID = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }

        public PaymentCategory(DataRow r)
        {
            ID = r.Field<string>("PayCat_ID");
            Name = r.Field<string>("PayCat_Name");
            FullName = r.Field<string>("PayCat_FullName");
        }
        [Index(0)]
        public virtual string ID { get; set; }
        [Index(1)]
        public virtual string Name { get; set; }
        private string fullName;
        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => FullName, ref fullName, value);
            }
        }
    }
}
