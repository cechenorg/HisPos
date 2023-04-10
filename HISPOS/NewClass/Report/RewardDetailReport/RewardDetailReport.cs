using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Report.RewardDetailReport
{
    public class RewardDetailReport : ObservableObject
    {
        public RewardDetailReport(DataRow r)
        {
            RewardAmount = r.Table.Columns.Contains("RewardAmount") ? r.Field<double>("RewardAmount") : 0;
            Emp_Name = r.Table.Columns.Contains("Emp_Name") ? r.Field<string>("Emp_Name") : string.Empty;
            TraDet_RewardPersonnel = r.Table.Columns.Contains("TraDet_RewardPersonnel") ? r.Field<string>("TraDet_RewardPersonnel") : string.Empty;
            RewardAmount = Math.Ceiling(RewardAmount);
            Free = r.Table.Columns.Contains("Free") ? r.Field<double>("Free") : 0;
        }

        public double RewardAmount { get; set; }
        public string Emp_Name { get; set; }
        public string TraDet_RewardPersonnel { get; set; }
        public double Free { get; set; }
    }
}