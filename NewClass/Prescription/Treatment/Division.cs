using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class Division
    {
        public Division() {}

        public Division(DataRow r)
        {
            Id = r["DIV_ID"].ToString();
            Name = r["DIV_NAME"].ToString();
            FullName = r["DIV_FULLNAME"].ToString();
        }
        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
    }
}
