using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashReport
{
    public class CashReport: ObservableObject
    {
        public CashReport()
        {
             
        }
        public CashReport(DataRow r) {
            TypeId = r.Field<string>("TypeId");
            TypeName = r.Field<string>("TypeName");
            CopayMentPrice = r.Field<int>("CopayMentPrice");
            PaySelfPrice = r.Field<int>("PaySelfPrice");
            AllPaySelfPrice = r.Field<int>("AllPaySelfPrice");
            DepositPrice = r.Field<int>("DepositPrice");
            OtherPrice = r.Field<int>("OtherPrice");
            TotalPrice = r.Field<int>("TotalPrice");
        }
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public int CopayMentPrice { get; set; }
        public int PaySelfPrice { get; set; }
        public int AllPaySelfPrice { get; set; }
        public int DepositPrice { get; set; }
        public int OtherPrice { get; set; }
        public int TotalPrice { get; set; }
    }
}
