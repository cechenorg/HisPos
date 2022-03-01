using GalaSoft.MvvmLight;
using System.Data;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.SpecialTreat
{
    [ZeroFormattable]
    public class SpecialTreat : ObservableObject
    {
        public SpecialTreat()
        {
        }

        public SpecialTreat(DataRow r)
        {
            ID = r.Field<string>("SpeTre_ID");
            Name = r.Field<string>("SpeTre_Name");
            FullName = r.Field<string>("SpeTre_FullName");
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