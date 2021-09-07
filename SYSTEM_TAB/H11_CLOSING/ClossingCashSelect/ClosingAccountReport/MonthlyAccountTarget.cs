using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
    public class MonthlyAccountTarget : ObservableObject
    {
        public MonthlyAccountTarget()
        {
        }

        public MonthlyAccountTarget(DataRow r)
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

        private string targetRatio; //達成率

        public string TargetRatio
        {
            get => targetRatio;
            set
            {
                Set(() => TargetRatio, ref targetRatio, value);
            }
        }
    }
}