using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    [ZeroFormattable]
    public class SpecialTreat : ObservableObject
    {
        public SpecialTreat() {
            ID = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }
        public SpecialTreat(DataRow r)
        {
            ID = r.Field<string>("SpeTre_ID");
            Name = r.Field<string>("SpeTre_Name");
            FullName = r.Field<string>("SpeTre_FullName");
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
