﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport
{
    public class TradeProfitDetailReport : ObservableObject
    {
        public TradeProfitDetailReport()
        {
        }

        public TradeProfitDetailReport(DataRow r) {
            Id = r.Field<int>("TraMas_ID");
            Name = r.Field<string>("PosCus_Name");
            RealTotal = r.Field<int>("TraMas_RealTotal");
            ValueDifference = Math.Round(r.Field<decimal>("ValueDifference"), 2);
            Profit = r.Field<int>("Profit");
            CashAmount = r.Field<int>("CashAmount");
            CardAmount = r.Field<int>("CardAmount");
            DiscountAmt = r.Field<int>("DiscountAmt");
            TypeId= r.Field<string>("TypeId");
        }
        private int id;
        private string name;
        private int realTotal;
        private decimal valueDifference;
        private int profit;
        private int cashAmount;
        private int cardAmount;
        private int discountAmt;
        private string typeId;
        private int count;
        public string TypeId
        {
            get => typeId;
            set
            {
                Set(() => TypeId, ref typeId, value);
            }
        }


        public int Id
        {
            get => id;
            set
            {
                Set(() => Id, ref id, value);
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
        public string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }
        public int RealTotal
        {
            get => realTotal;
            set
            {
                Set(() => RealTotal, ref realTotal, value);
            }
        }
        public decimal ValueDifference
        {
            get => valueDifference;
            set
            {
                Set(() => ValueDifference, ref valueDifference, value);
            }
        }
        public int Profit
        {
            get => profit;
            set
            {
                Set(() => Profit, ref profit, value);
            }
        }
        public int CashAmount
        {
            get => cashAmount;
            set
            {
                Set(() => CashAmount, ref cashAmount, value);
            }
        }
        public int CardAmount
        {
            get => cardAmount;
            set
            {
                Set(() => CardAmount, ref cardAmount, value);
            }
        }
        public int DiscountAmt
        {
            get => discountAmt;
            set
            {
                Set(() => DiscountAmt, ref discountAmt, value);
            }
        }



    }
}
