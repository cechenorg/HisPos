using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    public class Division
    {
        public Division() {}

        public Division(DataRow r)
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
