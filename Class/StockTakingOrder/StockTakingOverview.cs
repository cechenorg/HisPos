﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class.StockTakingOrder
{
    public class StockTakingOverview : IStockTakingRecord
    {
        public StockTakingOverview(DataRow dataRow)
        {
            StockTakingId = dataRow["PROCHE_ID"].ToString();
            StockTakingDate = dataRow["PROCHE_DATE"].ToString();
            EmpId = dataRow["EMP_NAME"].ToString();
            OldValue = dataRow["PROCHE_OLDVAL"].ToString();
            NewValue = dataRow["PROCHE_NEWVAL"].ToString();
        }

        //PROCHE_ID, EMP_NAME, PROCHE_DATE, PROCHE_OLDVAL, PROCHE_NEWVAL
        public string StockTakingId { get; set; }
        public string StockTakingDate { get; set; }
        public string EmpId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
