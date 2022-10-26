using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport
{
    public class MonthlyAccountTarget:ObservableObject
    {
        public MonthlyAccountTarget() { }
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
        public int PrescriptionCountTarget { get; set; } //慢箋張數
        public int DrugProfitTarget { get; set; } //配藥+慢箋毛利
        public int OtcProfitTarget { get; set; } //OTC毛利
        public int OtcTurnoverTarget { get; set; } //OTC營業額

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
