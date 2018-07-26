using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class DeclareTrade
    {
        public DeclareTrade(string vCusId,string vEmpId,string vPaySelf,string vDeposit,string vReceiveMoney,string vCopayMent,string vPayWay) {
            CusId = vCusId;
            EmpId = vEmpId;
            PaySelf = vPaySelf;
            Deposit = vDeposit;
            ReceiveMoney = vReceiveMoney;
            CopayMent = vCopayMent;
            PayWay = vPayWay;
        }
        
       public string DecMasId { get; set; }
        public string CusId { get; set; }
        public string EmpId { get; set; }
        public string PaySelf { get; set; } //自費
        public string Deposit { get; set; }
        public string ReceiveMoney { get; set; }
        public string CopayMent { get; set; }
        public string PayWay { get; set; }
    }
}