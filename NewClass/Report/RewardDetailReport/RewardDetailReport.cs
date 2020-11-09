﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.RewardDetailReport
{
    public class RewardDetailReport : ObservableObject
    {
        public RewardDetailReport(DataRow r) {
            RewardAmount = r.Field<double>("RewardAmount");
            Emp_Name = r.Field<string>("Emp_Name");
            TraDet_RewardPersonnel = r.Field<string>("TraDet_RewardPersonnel");
            RewardAmount=Math.Ceiling(RewardAmount);
        }

        public double RewardAmount { get; set; }
        public string Emp_Name { get; set; }
        public string TraDet_RewardPersonnel { get; set; }
    }
}