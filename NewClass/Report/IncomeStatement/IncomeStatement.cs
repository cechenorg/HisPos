using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class IncomeStatement : ObservableObject
    {
        public IncomeStatement()
        {

        }

        private List<int> prescriptionCount;

        public List<int> PrescriptionCount
        {
            get => prescriptionCount;
            set 
            {
                Set(() => PrescriptionCount, ref prescriptionCount, value);
            }
        }

        private int chronicIncome;

        public int ChronicIncome
        {
            get => chronicIncome;
            set
            {
                Set(() => ChronicIncome, ref chronicIncome, value);
            }
        }

        private int normalIncome;

        public int NormalIncome
        {
            get => normalIncome;
            set
            {
                Set(() => NormalIncome, ref normalIncome, value);
            }
        }

        private int otherIncome;

        public int OtherIncome
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

        private int chronicCost;

        public int ChronicCost
        {
            get => chronicCost;
            set
            {
                Set(() => ChronicCost, ref chronicCost, value);
            }
        }

        private int normalCost;

        public int NormalCost
        {
            get => normalCost;
            set
            {
                Set(() => NormalCost, ref normalCost, value);
            }
        }

        private int prescribeIncome;

        public int PrescribeIncome
        {
            get => prescribeIncome;
            set
            {
                Set(() => PrescribeIncome, ref prescribeIncome, value);
            }
        }

        private int additionalIncome;

        public int AdditionalIncome
        {
            get => additionalIncome;
            set
            {
                Set(() => AdditionalIncome, ref additionalIncome, value);
            }
        }

        private int prescribeCost;

        public int PrescribeCost
        {
            get => prescribeCost;
            set
            {
                Set(() => PrescribeCost, ref prescribeCost, value);
            }
        }

        private int expense;
        public int Expense
        {
            get => expense;
            set
            {
                Set(() => Expense, ref expense, value);
            }
        }

        private int inventoryShortage;
        public int InventoryShortage
        {
            get => inventoryShortage;
            set
            {
                Set(() => InventoryShortage, ref inventoryShortage, value);
            }
        }

        private int inventoryOverage;
        public int InventoryOverage
        {
            get => inventoryOverage;
            set
            {
                Set(() => InventoryOverage, ref inventoryOverage, value);
            }
        }

        private int inventoryScrapped;
        public int InventoryScrapped
        {
            get => inventoryScrapped;
            set
            {
                Set(() => InventoryScrapped, ref inventoryScrapped, value);
            }
        }

        public int ChronicProfit => CountDeclareIncome() - CountDeclareCost();
        public int AdjustProfit => CountAdjustIncome() - CountAdjustCost();
        public int HISProfit => CountHISIncome() - CountHISCost();

        private int CountHISIncome()
        {
            return ChronicProfit + AdjustProfit + InventoryOverage;
        }

        private int CountHISCost()
        {
            return Expense + InventoryShortage + InventoryScrapped;
        }

        private int CountDeclareIncome()
        {
            return ChronicIncome + NormalIncome + CooperativeInstitutionsIncome.Sum(c => c.TotalIncome);
        }

        private int CountDeclareCost()
        {
            return ChronicCost + NormalCost;
        }

        private int CountAdjustIncome()
        {
            return PrescribeIncome + AdditionalIncome;
        }

        private int CountAdjustCost()
        {
            return PrescribeCost;
        }
    }
}
