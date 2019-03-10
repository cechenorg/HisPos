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
            CopaymentValue = (int)r.Field<decimal>("部分負擔");
            ClinicCopaymentValue = (int)r.Field<decimal>("合作部分負擔");
            PaySelfValue = (int)r.Field<decimal>("自費");
            ClinicPaySelfValue = (int)r.Field<decimal>("合作自費");
            ClinicProfitValue = (int)r.Field<decimal>("骨科毛利");
            NormalTotalPointValue = (int)r.Field<decimal>("一般總點數");
            NormalMedicineUseValue = (int)r.Field<decimal>("一般調劑耗用");
            ChronicTotalPointValue = (int)r.Field<decimal>("慢箋總點數");
            ChronicmedicineUseValue = (int)r.Field<decimal>("慢箋調劑耗用");
            PayselfAdjustValue = (int)r.Field<decimal>("自費調劑");
            PayselfMedUseValue = (int)r.Field<decimal>("自費調劑耗用");
            DepositValue = (int)r.Field<decimal>("押金");
            TotalAdjustAmount = (int)r.Field<decimal>("處方總量");
            ClinicAdjustAmount = (int)r.Field<decimal>("合作處方總量");
            GiveclinicValue = ClinicCopaymentValue + ClinicPaySelfValue;
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
        private decimal payselfAdjustValue;
        public decimal PayselfAdjustValue //自費調劑收入
        {
            get => payselfAdjustValue;
            set
            {
                Set(() => PayselfAdjustValue, ref payselfAdjustValue, value);
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
        private int clinicAdjustAmount;
        public int ClinicAdjustAmount //合作診所調劑總量
        {
            get => clinicAdjustAmount;
            set
            {
                Set(() => ClinicAdjustAmount, ref clinicAdjustAmount, value);
            }
        }
        private int totalAdjustAmount;
        public int TotalAdjustAmount //調劑總量
        {
            get => totalAdjustAmount;
            set
            {
                Set(() => TotalAdjustAmount, ref totalAdjustAmount, value);
            }
        }
        private int giveclinicValue;
        public int GiveclinicValue //給骨科的錢
        {
            get => giveclinicValue;
            set
            {
                Set(() => GiveclinicValue, ref giveclinicValue, value);
            }
        }
    }
}


 
 
 


 

