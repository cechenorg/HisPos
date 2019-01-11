using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public class Institution
    {
        public Institution(){}

        public Institution(DataRow r)
        {
            Id = r[""].ToString();
            Name = r[""].ToString();
            FullName = r[""].ToString();
        }
        public string Id { get; }//院所代碼
        public string Name { get; }//院所名稱
        public string FullName { get; }
        public bool Common { get; set; }
        public bool IsCooperative { get; set; }
    }
}
