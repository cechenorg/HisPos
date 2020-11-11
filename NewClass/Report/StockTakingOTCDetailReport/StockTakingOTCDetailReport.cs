﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.StockTakingDetailReport
{
    public class StockTakingOTCDetailReport : ObservableObject
    {
        public StockTakingOTCDetailReport()
        {
        }

        public StockTakingOTCDetailReport(DataRow r) {
            InvRecSourceID = r.Field<string>("StoTakDet_MasterID");
            Price = Math.Round(r.Field<decimal>("Price"), 2);
            Type= r.Field<string>("TypeID");
        }

        private string invRecSourceID;
        private string type;
        private decimal price;
        private int count;
        public string InvRecSourceID
        {
            get => invRecSourceID;
            set
            {
                Set(() => InvRecSourceID, ref invRecSourceID, value);
            }
        }
        public string Type
        {
            get => type;
            set
            {
                Set(() => Type, ref type, value);
            }
        }
        public decimal Price
        {
            get => price;
            set
            {
                Set(() => Price, ref price, value);
            }
        }

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }


    }
}
