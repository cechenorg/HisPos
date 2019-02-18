using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    [ZeroFormattable]
    public class Copayment : ObservableObject
    {
        public Copayment() {
            Id = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }
        public Copayment(DataRow r)
        {
            Id = r.Field<string>("Cop_ID");
            Name = r.Field<string>("Cop_Name");
            FullName = r.Field<string>("Cop_FullName");
        }
        [Index(0)]
        public virtual string Id { get; set; }
        [Index(1)]
        public virtual string Name { get; set; }
        private string fullName;
        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => Id, ref fullName, value);
            }
        }
    }
}
