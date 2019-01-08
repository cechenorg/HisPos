using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class AdjustCase
    {
        public AdjustCase() { }

        public AdjustCase(DataRow r)
        {
            Id = r["CASE_ID"].ToString();
            Name = r["CASE_NAME"].ToString();
            FullName = r["CASE_FULLNAME"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
