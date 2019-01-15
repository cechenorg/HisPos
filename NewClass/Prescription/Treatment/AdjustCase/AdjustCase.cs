using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    public class AdjustCase:ObservableObject
    {
        public AdjustCase() { }

        public AdjustCase(DataRow r)
        {
            Id = r.Field<string>("Adj_ID");
            Name = r.Field<string>("Adj_Name");
            FullName = r.Field<string>("Adj_FullName");
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
