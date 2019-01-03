using System;
using System.Data;
using His_Pos.Service;

namespace His_Pos.NewClass
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
