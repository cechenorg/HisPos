using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    [ZeroFormattable]
    public class Division : ObservableObject
    {
        public Division() {
        }

        public Division(DataRow r)
        {
            ID = r.Field<string>("Div_ID");
            Name = r.Field<string>("Div_Name");
            FullName = r.Field<string>("Div_FullName");
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
