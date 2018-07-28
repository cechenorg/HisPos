using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryHis
    {
        public CustomerHistoryHis(string col0, string col1, string col2, string col3)
        {
            Col0 = col0;
            Col1 = col1;
            Col2 = col2;
            Col3 = col3;
        }

        public string Col0 { get; }
        public string Col1 { get; }
        public string Col2 { get; }
        public string Col3 { get; }
    }
}