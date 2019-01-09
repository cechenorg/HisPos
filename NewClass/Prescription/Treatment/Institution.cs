using System.Data;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class Institution
    {
        public Institution(){}

        public Institution(DataRow r)
        {
            Id = r["INS_ID"].ToString();
            Name = r["INS_NAME"].ToString();
            FullName = r["INS_FULLNAME"].ToString();
        }
        public string Id { get; }//院所代碼
        public string Name { get; }//院所名稱
        public string FullName { get; }
    }
}
