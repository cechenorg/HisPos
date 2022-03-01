using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Report.RewardDetailReport
{
    public class RewardDetailReport : ObservableObject
    {
        public RewardDetailReport(DataRow r)
        {
            RewardAmount = r.Field<double>("RewardAmount");
            Emp_Name = r.Field<string>("Emp_Name");
            TraDet_RewardPersonnel = r.Field<string>("TraDet_RewardPersonnel");
            RewardAmount = Math.Ceiling(RewardAmount);
            Free = r.Field<double>("Free");
        }

        public double RewardAmount { get; set; }
        public string Emp_Name { get; set; }
        public string TraDet_RewardPersonnel { get; set; }
        public double Free { get; set; }
    }
}