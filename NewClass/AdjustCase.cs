using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace His_Pos.NewClass
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
