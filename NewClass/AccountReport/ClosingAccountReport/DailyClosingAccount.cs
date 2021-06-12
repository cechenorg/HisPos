using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
    public class DailyClosingAccount
    {
        public DailyClosingAccount()  { }

        public string PharmacyName { get; set; }

        public int OTCSaleProfit { get; set; } //OTC銷售毛利

        public int DailyAdjustAmount { get; set; } //單日調劑人數

        public int CooperativeClinicProfit { get; set; } //合作診所

        public int PrescribeProfit { get; set; } //配藥收入

        public int ChronicAndOtherProfit { get; set; } //慢箋 + 其他\

        public int SelfProfit { get; set; } //自己的目標  銷貨毛利 + 配藥 + 慢箋 + 其他

        public int TotalProfit { get; set; } // 總計目標  SelfTarget + 合作診我
    }
}
