using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
    public class MonthlyAccountTarget
    {
        public MonthlyAccountTarget(DataRow r )
        {
            VerifyKey = r.Field<string>("Pharmacy_VerifyKey");
            Month = r.Field<DateTime>("TargetMonth");
            MonthlyTarget = r.Field<int>("TargetValue"); 
        }

        public string VerifyKey { get; set; }
        public string PharmacyName { get; set; } // 藥局名稱

        public int MonthlyProfit { get; set; } //月業績

        public int MonthlyTarget { get; set; } //月目標

        public DateTime Month { get; set; } //加總

        public double TargetRatio { get; set; } //達成率
    }
}
