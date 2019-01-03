using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass
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
