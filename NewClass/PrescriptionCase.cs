using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass
{
    public class PrescriptionCase
    {
        public PrescriptionCase() { }
        public PrescriptionCase(DataRow r)
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
