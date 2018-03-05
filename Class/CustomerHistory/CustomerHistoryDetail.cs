using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryDetail
    {
        public CustomerHistoryDetail(string customerHistoryDetailCol0, string customerHistoryDetailCol1, string customerHistoryDetailCol2, string customerHistoryDetailCol3)
        {
            CustomerHistoryDetailCol0 = customerHistoryDetailCol0;
            CustomerHistoryDetailCol1 = customerHistoryDetailCol1;
            CustomerHistoryDetailCol2 = customerHistoryDetailCol2;
            CustomerHistoryDetailCol3 = customerHistoryDetailCol3;
        }
        
        public string CustomerHistoryDetailCol0 { get; }
        public string CustomerHistoryDetailCol1 { get; }
        public string CustomerHistoryDetailCol2 { get; }
        public string CustomerHistoryDetailCol3 { get; }
    }
}
