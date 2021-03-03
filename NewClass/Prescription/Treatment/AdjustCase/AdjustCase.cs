using GalaSoft.MvvmLight;
using System.Data;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    [ZeroFormattable]
    public class AdjustCase : ObservableObject
    {
        public AdjustCase()
        {
        }

        public AdjustCase(DataRow r)
        {
            ID = r.Field<string>("Adj_ID");
            Name = r.Field<string>("Adj_Name");
            FullName = r.Field<string>("Adj_FullName");
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

        public bool CheckIsPrescribe()
        {
            return ID.Equals("0");
        }

        public bool CheckIsSimpleForm()
        {
            return ID.Equals("3");
        }

        public bool CheckIsQuitSmoking()
        {
            return !string.IsNullOrEmpty(ID) && ID.Equals("5");
        }

        public bool CheckIsHomeCare()
        {
            return !string.IsNullOrEmpty(ID) && ID.Equals("D");
        }

        public bool IsChronic()
        {
            return ID.Equals("2");
        }

        public bool IsNormal()
        {
            return ID.Equals("1") || ID.Equals("3");
        }
    }
}