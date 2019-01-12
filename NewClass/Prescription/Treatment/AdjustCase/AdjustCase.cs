using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    public class AdjustCase:ObservableObject
    {
        public AdjustCase() { }

        public AdjustCase(DataRow r)
        {
            Id = r[""].ToString();
            Name = r[""].ToString();
            FullName = r[""].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
