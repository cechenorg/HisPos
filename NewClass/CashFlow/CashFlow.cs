using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.CashFlow {
    public class CashFlow {
        public CashFlow() { }
        public CashFlow(DataRow r) {
            Date = r.Field<DateTime>("CashFlow_Time");
            CopaymentValue = Math.Round(r.Field<decimal>("CopaymentValue"), 2);
            ClinicCopaymentValue = Math.Round(r.Field<decimal>("ClinicCopaymentValue"), 2);
            PaySelfValue = Math.Round(r.Field<decimal>("PaySelfValue"), 2);
            ClinicPaySelfValue = Math.Round(r.Field<decimal>("ClinicPaySelfValue"), 2);
            MedServiceValue = Math.Round(r.Field<decimal>("MedServiceValue"), 2);
            ClinicMedServiceValue = Math.Round(r.Field<decimal>("ClinicMedServiceValue"), 2);
            DepositValue = Math.Round(r.Field<decimal>("DepositValue"), 2);
        }
        public DateTime Date { get; set; }
        public decimal CopaymentValue { get; set; }
        public decimal ClinicCopaymentValue { get; set; }
        public decimal PaySelfValue { get; set; }
        public decimal ClinicPaySelfValue { get; set; }
        public decimal MedServiceValue { get; set; }
        public decimal ClinicMedServiceValue { get; set; }
        public decimal DepositValue { get; set; }

       
    }
}
