﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryHis : CustomerHistoryDetail
    {

        public CustomerHistoryHis(DataRow dataRow)
        {
            MedicineName = dataRow["COL0"].ToString();
            Usage = dataRow["COL1"].ToString();
            Position = dataRow["COL2"].ToString();
            Dosage = dataRow["COL3"].ToString();
        }

        public string MedicineName { get; }
        public string Usage { get; }
        public string Position { get; }
        public string Dosage { get; }
    }
}