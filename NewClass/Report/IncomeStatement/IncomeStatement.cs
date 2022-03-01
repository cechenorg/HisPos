using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class IncomeStatement : ObservableObject
    {
        public IncomeStatement(int month)
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

        private decimal prescribeIncome;

        public decimal PrescribeIncome
        {
            get => prescribeIncome;
            set
            {
                Set(() => PrescribeIncome, ref prescribeIncome, value);
            }
        }

        private decimal additionalIncome;

        public decimal AdditionalIncome
        {
            get => additionalIncome;
            set
            {
                Set(() => AdditionalIncome, ref additionalIncome, value);
            }
        }

        private decimal prescribeCost;

        public decimal PrescribeCost
        {
            get => prescribeCost;
            set
            {
                Set(() => PrescribeCost, ref prescribeCost, value);
            }
        }

        private decimal expense;

        public decimal Expense
        {
            get => expense;
            set
            {
                Set(() => Expense, ref expense, value);
            }
        }

        private decimal inventoryShortage;

        public decimal InventoryShortage
        {
            get => inventoryShortage;
            set
            {
                Set(() => InventoryShortage, ref inventoryShortage, value);
            }
        }

        private decimal inventoryOverage;

        public decimal InventoryOverage
        {
            get => inventoryOverage;
            set
            {
                Set(() => InventoryOverage, ref inventoryOverage, value);
            }
        }

        private decimal inventoryScrapped;

        public decimal InventoryScrapped
        {
            get => inventoryScrapped;
            set
            {
                Set(() => InventoryScrapped, ref inventoryScrapped, value);
            }
        }

        public decimal ChronicProfit => CountDeclareIncome() - CountDeclareCost();
        public decimal PrescribeProfit => CountAdjustIncome() - CountAdjustCost();
        public decimal HISProfit => CountHISIncome() - CountHISCost();

        private decimal CountHISIncome()
        {
            return ChronicProfit + PrescribeProfit + InventoryOverage;
        }

        private decimal CountHISCost()
        {
            return Expense + InventoryShortage + InventoryScrapped;
        }

        private decimal CountDeclareIncome()
        {
            return ChronicIncome + NormalIncome + CooperativeInstitutionsIncome.Sum(c => c.TotalIncome);
        }

        private decimal CountDeclareCost()
        {
            return ChronicCost + NormalCost;
        }

        private decimal CountAdjustIncome()
        {
            return PrescribeIncome + AdditionalIncome;
        }

        private decimal CountAdjustCost()
        {
            return PrescribeCost;
        }
    }
}