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
            PayselfMedUseValue = 0;
            NormalTotalPointValue = 0;
            ChronicTotalPointValue = 0;
            ChronicmedicineUseValue = 0;
            NormalMedicineUseValue = 0;
            DepositValue = 0;
        }
        
        public DateTime Date { get; set; }
        private int copaymentValue;
        public int CopaymentValue {
            get => copaymentValue;
            set
            {
                Set(() => CopaymentValue, ref copaymentValue, value);
            }
        }
        private int clinicCopaymentValue;
        public int ClinicCopaymentValue
        {
            get => clinicCopaymentValue;
            set
            {
                Set(() => ClinicCopaymentValue, ref clinicCopaymentValue, value);
            }
        }
        private int paySelfValue;
        public int PaySelfValue
        {
            get => paySelfValue;
            set
            {
                Set(() => PaySelfValue, ref paySelfValue, value);
            }
        }
        private int clinicPaySelfValue;
        public int ClinicPaySelfValue
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
        private decimal normalTotalPointValue;
        public decimal NormalTotalPointValue //一般箋總點數
        {
            get => normalTotalPointValue;
            set
            {
                Set(() => NormalTotalPointValue, ref normalTotalPointValue, value);
            }
        }
        private decimal chronicTotalPointValue;
        public decimal ChronicTotalPointValue //慢箋總點數
        {
            get => chronicTotalPointValue;
            set
            {
                Set(() => ChronicTotalPointValue, ref chronicTotalPointValue, value);
            }
        } 
        private decimal normalmedicineUseValue;
        public decimal NormalMedicineUseValue //一般箋調劑耗用
        {
            get => normalmedicineUseValue;
            set
            {
                Set(() => NormalMedicineUseValue, ref normalmedicineUseValue, value);
            }
        }
        private decimal chronicmedicineUseValue;
        public decimal ChronicmedicineUseValue //慢箋調劑耗用
        {
            get => chronicmedicineUseValue;
            set
            {
                Set(() => ChronicmedicineUseValue, ref chronicmedicineUseValue, value);
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


 
 
 


 

