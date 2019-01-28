using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
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
        public string Id { get; }
        public string Name { get; }
        private string fullName;
        public string FullName
        {
            get => fullName;
            set
            {
                Set(() => Id, ref fullName, value);
            }
        }
    }
}
