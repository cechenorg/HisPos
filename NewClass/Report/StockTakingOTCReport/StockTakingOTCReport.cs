﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.StockTakingOTCReport
{
    public class StockTakingOTCReport : ObservableObject
    {
        public StockTakingOTCReport()
        {
        }
        public StockTakingOTCReport(DataRow r) {
            TypeId = r.Field<string>("TypeId");
            TypeName = r.Field<string>("TypeName");
            Count = r.Field<int>("Count");
            Price= Math.Round(r.Field<decimal>("Price"),2);
        }
        private string typeId;
        public string TypeId {
            get => typeId;
            set
            {
                Set(() => TypeId, ref typeId, value);
            }
        }
        private string typeName;
        public string TypeName
        {
            get => typeName;
            set
            {
                Set(() => TypeName, ref typeName, value);
            }
        }
        private int count;
        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }
        private decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                Set(() => Price, ref price, value);
            }
        }

    }
}
