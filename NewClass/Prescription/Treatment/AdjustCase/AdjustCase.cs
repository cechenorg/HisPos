using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    [ZeroFormattable]
    public class AdjustCase:ObservableObject
    {
        public AdjustCase()
        {
            Id = string.Empty;
            Name = string.Empty;
            FullName = string.Empty;
        }

        public AdjustCase(DataRow r)
        {
            Id = r.Field<string>("Adj_ID");
            Name = r.Field<string>("Adj_Name");
            FullName = r.Field<string>("Adj_FullName");
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
                Set(() => FullName, ref fullName, value);
            }
        }
    }
}
