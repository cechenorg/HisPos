using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.NewClass.Report.CashFlow
{
    public struct CashFlowAccount
    {
        public CashFlowAccount(CashFlowType type,string accountName)
        {
            Type = type;
            AccountName = accountName;
        }
        public CashFlowType Type{ get; set; }
        public string AccountName { get; set; }
    }
}
