using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
    public class MonthlyAccountTarget
    {
        public string PharmacyName { get; set; } // 藥局名稱

        public int MonthlyTarget { get; set; } //月目標

        public int TotalProfit { get; set; } //加總

        public double TargetRatio { get; set; } //達成率
    }
}
