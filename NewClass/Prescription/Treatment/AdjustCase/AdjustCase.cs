using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.Treatment.AdjustCase
{
    public class AdjustCase:ObservableObject
    {
        public AdjustCase() { }

        public AdjustCase(DataRow r)
        {
            Id = r["Adj_ID"].ToString();
            Name = r["Adj_Name"].ToString();
            FullName = r["Adj_FullName"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
