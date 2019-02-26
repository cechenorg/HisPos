using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.PrescriptionCase
{
    [ZeroFormattable]
    public class PrescriptionCase : ObservableObject
    {
        public PrescriptionCase() {
        }
        public PrescriptionCase(DataRow r)
        {
            ID = r.Field<string>("PreCase_ID");
            Name = r.Field<string>("PreCase_Name");
            FullName = r.Field<string>("PreCase_FullName");
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
