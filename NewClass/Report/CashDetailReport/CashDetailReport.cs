using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashDetailReport
{
    public class CashDetailReport: ObservableObject
    {
        public CashDetailReport(DataRow r) {
            Id = r.Field<int>("Id");
            CusName = r.Field<string>("Cus_Name");
            CopayMentPrice = r.Field<int>("CopayMentPrice");
            PaySelfPrice = r.Field<int>("PaySelfPrice");
            PaySelfPrescritionPrice = r.Field<int>("PaySelfPrecriptionPrice");
            Deposit = r.Field<int>("Deposit");
            Other = r.Field<int>("Other");
        }
        public int Id { get; set; }
        public string CusName { get; set; }
        public int CopayMentPrice { get; set; }
        public int PaySelfPrice { get; set; }
        public int PaySelfPrescritionPrice { get; set; }
        public int Deposit { get; set; }
        public int Other { get; set; }
    }
}
