using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    [ZeroFormattable]
    public class Division : ObservableObject
    {
        public Division() {
            ID = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }

        public Division(DataRow r)
        {
            ID = r.Field<string>("Div_ID");
            Name = r.Field<string>("Div_Name");
            FullName = r.Field<string>("Div_FullName");
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
