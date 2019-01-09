using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class Copayment
    {
        public Copayment() { }
        public Copayment(DataRow r)
        {
            Id = r["COPAYMENT_ID"].ToString();
            Name = r["COPAYMENT_NAME"].ToString();
            FullName = r["COPAYMENT_FULLNAME"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
