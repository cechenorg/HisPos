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
            CopaymentValue = r.Field<int>("部分負擔");
            ClinicCopaymentValue = r.Field<int>("合作部分負擔");
            PaySelfValue = r.Field<int>("自費");
            ClinicPaySelfValue = r.Field<int>("合作自費");
            ClinicProfitValue = r.Field<int>("骨科毛利");
            NormalTotalPointValue = r.Field<int>("一般總點數");
            NormalMedicineUseValue = r.Field<int>("一般調劑耗用");
            ChronicTotalPointValue = r.Field<int>("慢箋總點數");
            ChronicmedicineUseValue = r.Field<int>("慢箋調劑耗用");
            PayselfAdjustValue = r.Field<int>("自費調劑");
            PayselfMedUseValue = r.Field<int>("自費調劑耗用");
            DepositValue = r.Field<int>("押金");
            ClinicDepositValue = r.Field<int>("合作押金");
            TotalAdjustAmount = r.Field<int>("處方總量");
            ClinicAdjustAmount = r.Field<int>("合作處方總量");
            GiveclinicValue = ClinicCopaymentValue + ClinicPaySelfValue + (int)ClinicDepositValue;
            TotalCash = CopaymentValue + ClinicCopaymentValue + PaySelfValue + ClinicPaySelfValue  + (int)DepositValue + (int)ClinicDepositValue + (int)PayselfAdjustValue;
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
        private decimal clinicDepositValue;
        public decimal ClinicDepositValue
        {
            get => clinicDepositValue;
            set
            {
                Set(() => ClinicDepositValue, ref clinicDepositValue, value);
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
        private int totalCash;
        public int TotalCash //總計
        {
            get => totalCash;
            set
            {
                Set(() => TotalCash, ref totalCash, value);
            }
        }
        
    }
}


 
 
 


 

