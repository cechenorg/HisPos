﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace His_Pos.NewClass.Person.Employee.WorkPosition
{
   public class WorkPosition
    {
        public WorkPosition(DataRow r) { 
            WorkPositionId = r.Field<int>("WorkPos_ID");
            WorkPositionName = r.Field<string>("WorkPos_Name"); 
        }
        public int WorkPositionId { get; set; }
        public string WorkPositionName { get; set; }
    }
}