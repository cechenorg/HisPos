using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.CashFlow {
    public class CashFlow : ObservableObject {
        public CashFlow() {
            CopaymentValue = 0;
            ClinicCopaymentValue = 0;
            PaySelfValue = 0;
            ClinicPaySelfValue = 0;
            ClinicProfitValue = 0;
            PrescriptionwithoutCooperativeValue = 0;
            PayselfMedUseValue = 0;
            MedicineUseValue = 0;
            DepositValue = 0;
        }
        public CashFlow(DataRow r) {
            Date = r.Field<DateTime>("CashFlow_Time");
            CopaymentValue = Math.Round(r.Field<decimal>("CopaymentValue"), 2);
            ClinicCopaymentValue = Math.Round(r.Field<decimal>("ClinicCopaymentValue"), 2);
            PaySelfValue = Math.Round(r.Field<decimal>("PaySelfValue"), 2);
            ClinicPaySelfValue = Math.Round(r.Field<decimal>("ClinicPaySelfValue"), 2); 
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
        private decimal clinicProfitValue;
        public decimal ClinicProfitValue //骨科毛利
        {
            get => clinicProfitValue;
            set
            {
                Set(() => ClinicProfitValue, ref clinicProfitValue, value);
            }
        }
        private decimal prescriptionwithoutCooperativeValue;
        public decimal PrescriptionwithoutCooperativeValue //排除骨科總點數(慢箋 + 其他)
        {
            get => prescriptionwithoutCooperativeValue;
            set
            {
                Set(() => PrescriptionwithoutCooperativeValue, ref prescriptionwithoutCooperativeValue, value);
            }
        }
        private decimal medicineUseValue;
        public decimal MedicineUseValue //慢箋與一般箋調劑耗用
        {
            get => medicineUseValue;
            set
            {
                Set(() => MedicineUseValue, ref medicineUseValue, value);
            }
        }
        private decimal payselfMedUseValue;
        public decimal PayselfMedUseValue //自費調劑耗用
        {
            get => payselfMedUseValue;
            set
            {
                Set(() => PayselfMedUseValue, ref payselfMedUseValue, value);
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


 
 
 


 

