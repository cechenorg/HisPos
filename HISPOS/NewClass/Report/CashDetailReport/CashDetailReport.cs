using System.Collections.Generic;
using GalaSoft.MvvmLight;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Report.CashDetailReport
{
    public class CashDetailReport : ObservableObject
    {
        public CashDetailReport()
        {
        }

        public CashDetailReport(DataRow r)
        {
            Id = r.Field<int>("Id");
            CusName = r.Field<string>("Cus_Name");
            CopayMentPrice = r.Field<int>("CopayMentPrice");
            PaySelfPrice = r.Field<int>("PaySelfPrice");
            PaySelfPrescritionPrice = r.Field<int>("PaySelfPrecriptionPrice");
            Deposit = r.Field<int>("Deposit");
            Other = r.Field<int>("Other");
            Ins_Name = r.Field<string>("Ins_Name");
        }

        private int id;
        private string cusName;
        private string ins_Name;
        private int copayMentPrice;
        private int paySelfPrice;
        private int paySelfPrescritionPrice;
        private int deposit;
        private int other;
        private int count;

        public int Id
        {
            get => id;
            set
            {
                Set(() => Id, ref id, value);
            }
        }

        public string CusName
        {
            get => cusName;
            set
            {
                Set(() => CusName, ref cusName, value);
            }
        }

        public string Ins_Name
        {
            get => ins_Name;
            set
            {
                Set(() => Ins_Name, ref ins_Name, value);
            }
        }

        public int CopayMentPrice
        {
            get => copayMentPrice;
            set
            {
                Set(() => CopayMentPrice, ref copayMentPrice, value);
            }
        }

        public int PaySelfPrice
        {
            get => paySelfPrice;
            set
            {
                Set(() => PaySelfPrice, ref paySelfPrice, value);
            }
        }

        public int PaySelfPrescritionPrice
        {
            get => paySelfPrescritionPrice;
            set
            {
                Set(() => PaySelfPrescritionPrice, ref paySelfPrescritionPrice, value);
            }
        }

        public int Deposit
        {
            get => deposit;
            set
            {
                Set(() => Deposit, ref deposit, value);
            }
        }

        public int Other
        {
            get => other;
            set
            {
                Set(() => Other, ref other, value);
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

        public void SumCashDetail(IEnumerable<CashDetailReport> cashDetailReport)
        {
           CusName = "總計";
           CopayMentPrice = cashDetailReport.Sum(s => s.CopayMentPrice);
           PaySelfPrice = cashDetailReport.Sum(s => s.PaySelfPrice);
           PaySelfPrescritionPrice = cashDetailReport.Sum(s => s.PaySelfPrescritionPrice);
           Deposit = cashDetailReport.Sum(s => s.Deposit);
           Other = cashDetailReport.Sum(s => s.Other);
           Count = cashDetailReport.Count();
        }
    }
}