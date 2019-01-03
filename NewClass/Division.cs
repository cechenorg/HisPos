using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass
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
