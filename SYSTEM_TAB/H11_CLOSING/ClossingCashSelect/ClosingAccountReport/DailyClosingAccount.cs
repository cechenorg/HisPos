using System;
using System.Data;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
    public class DailyClosingAccount
    {
        public DailyClosingAccount(DataRow r)
        {
            ClosingDate = r.Field<DateTime>("ClosingDate");
            PharmacyVerifyKey = r.Field<string>("Pharmacy_VerifyKey");
            OTCSaleProfit = r.Field<int>("OTCSaleProfit");
            DailyAdjustAmount = r.Field<int>("DailyAdjustAmount");
            CooperativeClinicProfit = r.Field<int>("CooperativeClinicProfit");
            PrescribeProfit = r.Field<int>("PrescribeProfit");
            ChronicAndOtherProfit = r.Field<int>("ChronicAndOtherProfit");
            SelfProfit = r.Field<int>("SelfProfit");
            TotalProfit = r.Field<int>("TotalProfit");
        }

        public DailyClosingAccount()
        {
        }

        public int OrderNumber { get; set; }

        public DateTime ClosingDate { get; set; }

        public string PharmacyVerifyKey { get; set; }

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