using GalaSoft.MvvmLight;
using System.Data;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.PaymentCategory
{
    [ZeroFormattable]
    public class PaymentCategory : ObservableObject
    {
        public PaymentCategory()
        {
        }

        public PaymentCategory(DataRow r)
        {
            ID = r.Field<string>("PayCat_ID");
            Name = r.Field<string>("PayCat_Name");
            FullName = r.Field<string>("PayCat_FullName");
        }

        [Index(0)]
        public virtual string ID { get; set; } = string.Empty;

        [Index(1)]
        public virtual string Name { get; set; } = string.Empty;

        private string fullName = string.Empty;

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