using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.NewClass.Report.Accounts
{
    public class AccountsAccount
    {
        public AccountsAccount(CashFlowType type,string accountName,string IDs)
        {
            Type = type;
            AccountName = accountName;
            ID = IDs;
        }
        public CashFlowType Type{ get; set; }
        public string AccountName { get; set; }
        public string ID { get; set; }
    }
}
