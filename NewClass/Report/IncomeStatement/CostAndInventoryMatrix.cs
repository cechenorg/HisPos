using System.Collections.Generic;
using System.Data;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class CostAndInventoryMatrix : MatrixLib.Matrix.MatrixBase<string, CostInventory>
    {
        public CostAndInventoryMatrix()
        {
        }

        public CostAndInventoryMatrix(DataTable costAndInventoryTable)
        {
            rowHeaderToValueProviderMap = new Dictionary<string, CellValueProvider>();
            costInventories = new CostInventory[12];
            for (var i = 0; i < costInventories.Length; i++)
                costInventories[i] = new CostInventory(i + 1);
            SetExpenseAndInventory(costAndInventoryTable);
            PopulateCellValueProviderMap();
        }

        #region Fields

        private readonly CostInventory[] costInventories;
        private readonly Dictionary<string, CellValueProvider> rowHeaderToValueProviderMap;

        private delegate object CellValueProvider(CostInventory pharmacyIncome);

        #endregion Fields

        protected override IEnumerable<CostInventory> GetColumnHeaderValues()
        {
            return costInventories;
        }

        protected override IEnumerable<string> GetRowHeaderValues()
        {
            return rowHeaderToValueProviderMap.Keys;
        }

        protected override object GetCellValue(string rowHeaderValue, CostInventory columnHeaderValue)
        {
            return rowHeaderToValueProviderMap[rowHeaderValue](columnHeaderValue);
        }

        private void PopulateCellValueProviderMap()
        {
            rowHeaderToValueProviderMap.Add("費用", cost => cost.Expense);
            rowHeaderToValueProviderMap.Add("庫存盤盈", cost => cost.InventoryOverage);
            rowHeaderToValueProviderMap.Add("庫存盤虧", cost => cost.InventoryShortage);
            rowHeaderToValueProviderMap.Add("庫存報廢", cost => cost.InventoryScrapped);
        }

        private void SetExpenseAndInventory(DataTable expenseAndInventoryTable)
        {
            var rowCount = 0;
            foreach (DataRow row in expenseAndInventoryTable.Rows)
            {
                var month = 1;
                foreach (var c in costInventories)
                {
                    SetExpenseAndInventoryField(c, row, rowCount, month);
                    month++;
                }
                rowCount++;
            }
        }

        private void SetExpenseAndInventoryField(CostInventory income, DataRow row, int rowCount, int month)
        {
            switch (rowCount)
            {
                case 0:
                    income.Expense = row.Field<decimal>($"{month}");
                    break;

                case 1:
                    income.InventoryOverage = row.Field<decimal>($"{month}");
                    break;

                case 2:
                    income.InventoryShortage = row.Field<decimal>($"{month}");
                    break;

                default:
                    income.InventoryScrapped = row.Field<decimal>($"{month}");
                    break;
            }
        }
    }
}