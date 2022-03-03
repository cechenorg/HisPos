using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class PharmacyIncome : ObservableObject
    {
        public PharmacyIncome(int month)
        {
            Month = $"{month}月";
            CooperativeInstitutionsIncome = new List<CooperativeInstitutionIncome>();
        }

        private string month;

        public string Month
        {
            get => month;
            set { Set(() => Month, ref month, value); }
        }

        private decimal chronicIncome;

        public decimal ChronicIncome
        {
            get => chronicIncome;
            set
            {
                Set(() => ChronicIncome, ref chronicIncome, value);
            }
        }

        private decimal normalIncome;

        public decimal NormalIncome
        {
            get => normalIncome;
            set
            {
                Set(() => NormalIncome, ref normalIncome, value);
            }
        }

        private decimal otherIncome;

        public decimal OtherIncome
        {
            get => otherIncome;
            set
            {
                Set(() => OtherIncome, ref otherIncome, value);
            }
        }

        private List<CooperativeInstitutionIncome> cooperativeInstitutionsIncome;

        public List<CooperativeInstitutionIncome> CooperativeInstitutionsIncome
        {
            get => cooperativeInstitutionsIncome;
            set
            {
                Set(() => CooperativeInstitutionsIncome, ref cooperativeInstitutionsIncome, value);
            }
        }

        private decimal chronicCost;

        public decimal ChronicCost
        {
            get => chronicCost;
            set
            {
                Set(() => ChronicCost, ref chronicCost, value);
            }
        }

        private decimal normalCost;

        public decimal NormalCost
        {
            get => normalCost;
            set
            {
                Set(() => NormalCost, ref normalCost, value);
            }
        }

        public decimal ChronicProfit => CountDeclareIncome() - CountDeclareCost();

        private decimal CountDeclareIncome()
        {
            return ChronicIncome + NormalIncome + OtherIncome + CooperativeInstitutionsIncome.Sum(c => c.TotalIncome);
        }

        private decimal CountDeclareCost()
        {
            return ChronicCost + NormalCost;
        }
    }
}