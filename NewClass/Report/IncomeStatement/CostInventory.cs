using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class CostInventory : ObservableObject
    {
        public CostInventory(int month)
        {
            Month = $"{month}月";
        }

        private string month;

        public string Month
        {
            get => month;
            set { Set(() => Month, ref month, value); }
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
    }
}