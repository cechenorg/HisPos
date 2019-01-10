using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Copayment
{
    public class Copayment
    {
        public Copayment() { }
        public Copayment(DataRow r)
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
