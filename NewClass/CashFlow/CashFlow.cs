using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.CashFlow {
    public class CashFlow : ObservableObject {
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
        private decimal copaymentValue;
        public decimal CopaymentValue {
            get => copaymentValue;
            set
            {
                Set(() => CopaymentValue, ref copaymentValue, value);
            }
        }
        private decimal clinicCopaymentValue;
        public decimal ClinicCopaymentValue
        {
            get => clinicCopaymentValue;
            set
            {
                Set(() => ClinicCopaymentValue, ref clinicCopaymentValue, value);
            }
        }
        private decimal paySelfValue;
        public decimal PaySelfValue
        {
            get => paySelfValue;
            set
            {
                Set(() => PaySelfValue, ref paySelfValue, value);
            }
        }
        private decimal clinicPaySelfValue;
        public decimal ClinicPaySelfValue
        {
            get => clinicPaySelfValue;
            set
            {
                Set(() => ClinicPaySelfValue, ref clinicPaySelfValue, value);
            }
        }
        private decimal medServiceValue;
        public decimal MedServiceValue
        {
            get => medServiceValue;
            set
            {
                Set(() => MedServiceValue, ref medServiceValue, value);
            }
        }
        private decimal clinicMedServiceValue;
        public decimal ClinicMedServiceValue
        {
            get => clinicMedServiceValue;
            set
            {
                Set(() => ClinicMedServiceValue, ref clinicMedServiceValue, value);
            }
        }
        private decimal depositValue;
        public decimal DepositValue
        {
            get => depositValue;
            set
            {
                Set(() => DepositValue, ref depositValue, value);
            }
        }

       
    }
}
