﻿using System.Collections.Generic;
using System.Data;
using MySqlX.XDevAPI.Relational;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class IncomeStatementMatrix : MatrixLib.Matrix.MatrixBase<string,IncomeStatement>
    {
        public IncomeStatementMatrix()
        {

        }

        public IncomeStatementMatrix(DataSet incomeSet)
        {
            rowHeaderToValueProviderMap = new Dictionary<string, CellValueProvider>();
            var declareIncomeTable = incomeSet.Tables[1];
            var pharmacyCostTable = incomeSet.Tables[2];
            var prescribeIncomeTable = incomeSet.Tables[3];
            var prescribeOtherIncomeAndCostTable = incomeSet.Tables[4];
            var expenseAndInventoryTable = incomeSet.Tables[5];
            cooperativeInstitutionIncomeHeaderTable = incomeSet.Tables[7];
            incomeStatement = new IncomeStatement[12];
            for (var i = 0; i < incomeStatement.Length; i++)
                incomeStatement[i] = new IncomeStatement();
            //SetDeclareIncome(declareIncomeTable);
            //SetPharmacyCost(pharmacyCostTable);
            SetPrescribeIncome(prescribeIncomeTable);
            SetPrescribeOtherIncomeAndCost(prescribeOtherIncomeAndCostTable);
            SetExpenseAndInventory(expenseAndInventoryTable);
            PopulateCellValueProviderMap();
        }

        private void SetDeclareIncome(DataTable declareIncomeTable)
        {
            var rowCount = 0;
            foreach (DataRow row in declareIncomeTable.Rows)
            {
                var month = 1;
                foreach (var income in incomeStatement)
                {
                    SetDeclareIncomeField(income,row, rowCount, month);
                    month++;
                }
                rowCount++;
            }
        }

        private void SetPharmacyCost(DataTable pharmacyCostTable)
        {
            var rowCount = 0;
            foreach (DataRow row in pharmacyCostTable.Rows)
            {
                var month = 1;
                foreach (var income in incomeStatement)
                {
                    SetPharmacyCostField(income, row, rowCount, month);
                    month++;
                }
                rowCount++;
            }
        }

        private void SetPrescribeIncome(DataTable prescribeIncomeTable)
        {
            foreach (DataRow row in prescribeIncomeTable.Rows)
            {
                var month = 1;
                foreach (var income in incomeStatement)
                {
                    income.PrescribeIncome = row.Field<decimal>($"{month}");
                    month++;
                }
            }
        }

        private void SetPrescribeOtherIncomeAndCost(DataTable prescribeOtherIncomeAndCostTable)
        {
            var rowCount = 0;
            foreach (DataRow row in prescribeOtherIncomeAndCostTable.Rows)
            {
                var month = 1;
                foreach (var income in incomeStatement)
                {
                    SetPrescribeOtherIncomeAndCostField(income, row, rowCount, month);
                    month++;
                }
                rowCount++;
            }
        }

        private void SetExpenseAndInventory(DataTable expenseAndInventoryTable)
        {
            var rowCount = 0;
            foreach (DataRow row in expenseAndInventoryTable.Rows)
            {
                var month = 1;
                foreach (var income in incomeStatement)
                {
                    SetExpenseAndInventoryField(income, row, rowCount, month);
                    month++;
                }
                rowCount++;
            }
        }

        private void SetDeclareIncomeField(IncomeStatement income, DataRow row, int rowCount,int month)
        {
            switch (rowCount)
            {
                case 0:
                    income.ChronicIncome = row.Field<decimal>($"{month}");
                    break;
                case 1:
                    income.NormalIncome = row.Field<decimal>($"{month}");
                    break;
                case 2:
                    income.OtherIncome = row.Field<decimal>($"{month}");
                    break;
                default:
                    SetCooperativeDeclareIncome(income, row, rowCount, month);
                    break;
            }
        }

        private void SetPharmacyCostField(IncomeStatement income, DataRow row, int rowCount, int month)
        {
            switch (rowCount)
            {
                case 0:
                    income.ChronicCost = row.Field<decimal>($"{month}");
                    break;
                case 1:
                    income.NormalCost = row.Field<decimal>($"{month}");
                    break;
            }
        }

        private void SetCooperativeDeclareIncome(IncomeStatement income, DataRow row, int rowCount, int month)
        {
            var incomeOrder = rowCount % 3;
            var insIndex = rowCount / 3 - 1;
            switch (incomeOrder)
            {
                case 0:
                    var c = new CooperativeInstitutionIncome();
                    c.MedicalServiceIncome = row.Field<decimal>($"{month}");
                    income.CooperativeInstitutionsIncome.Add(c);
                    break;
                case 1:
                    income.CooperativeInstitutionsIncome[insIndex].MedicineIncome = row.Field<decimal>($"{month}");
                    break;
                case 2:
                    income.CooperativeInstitutionsIncome[insIndex].OtherIncome = row.Field<decimal>($"{month}");
                    break;
            }
        }

        private void SetPrescribeOtherIncomeAndCostField(IncomeStatement income, DataRow row, int rowCount, int month)
        {
            switch (rowCount)
            {
                case 0:
                    income.AdditionalIncome = row.Field<decimal>($"{month}");
                    break;
                case 1:
                    income.PrescribeCost = row.Field<decimal>($"{month}");
                    break;
            }
        }

        private void SetExpenseAndInventoryField(IncomeStatement income, DataRow row, int rowCount, int month)
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

        protected override IEnumerable<IncomeStatement> GetColumnHeaderValues()
        {
            return incomeStatement;
        }

        protected override IEnumerable<string> GetRowHeaderValues()
        {
            return rowHeaderToValueProviderMap.Keys;
        }

        protected override object GetCellValue(string rowHeaderValue, IncomeStatement columnHeaderValue)
        {
            return rowHeaderToValueProviderMap[rowHeaderValue](columnHeaderValue);
        }

        #region Fields

        readonly IncomeStatement[] incomeStatement;
        readonly Dictionary<string, CellValueProvider> rowHeaderToValueProviderMap;
        private delegate object CellValueProvider(IncomeStatement incomeStatement);
        private DataTable cooperativeInstitutionIncomeHeaderTable { get; set; }
        #endregion

        void PopulateCellValueProviderMap()
        {
            //rowHeaderToValueProviderMap.Add("慢箋健保收入",income => income.ChronicIncome);
            //rowHeaderToValueProviderMap.Add("一般箋健保收入", income => income.NormalIncome);
            //rowHeaderToValueProviderMap.Add("其他收入", income => income.OtherIncome);

            //for (var i = 0; i < cooperativeInstitutionIncomeHeaderTable.Rows.Count; i++)
            //{
            //    var index = 0;
            //    var headerName = cooperativeInstitutionIncomeHeaderTable.Rows[i].Field<string>("IncomeName");
            //    if (headerName.Contains("合作藥服"))
            //    {
            //        rowHeaderToValueProviderMap.Add(
            //            cooperativeInstitutionIncomeHeaderTable.Rows[i].Field<string>("IncomeName"), income => income.CooperativeInstitutionsIncome[index].MedicalServiceIncome);
            //    }
            //    else if (headerName.Contains("合作藥品"))
            //    {
            //        rowHeaderToValueProviderMap.Add(
            //            cooperativeInstitutionIncomeHeaderTable.Rows[i].Field<string>("IncomeName"), income => income.CooperativeInstitutionsIncome[index].MedicineIncome);
            //    }
            //    else if (headerName.Contains("合作其他"))
            //    {
            //        rowHeaderToValueProviderMap.Add(
            //            cooperativeInstitutionIncomeHeaderTable.Rows[i].Field<string>("IncomeName"), income => income.CooperativeInstitutionsIncome[index].OtherIncome);
            //    }
            //    if(i % 3 == 0)
            //        index++;
            //}

            //rowHeaderToValueProviderMap.Add("慢箋銷貨成本",income => income.ChronicCost);
            //rowHeaderToValueProviderMap.Add("一般銷貨成本", income => income.NormalCost);
            //rowHeaderToValueProviderMap.Add("慢箋營業毛利", income => income.ChronicProfit);
            rowHeaderToValueProviderMap.Add("配藥收入", income => income.PrescribeIncome);
            rowHeaderToValueProviderMap.Add("額外收入", income => income.OtherIncome);
            rowHeaderToValueProviderMap.Add("配藥成本", income => income.PrescribeCost);
            rowHeaderToValueProviderMap.Add("調劑營業毛利", income => income.AdjustProfit);
            rowHeaderToValueProviderMap.Add("費用", income => income.Expense);
            rowHeaderToValueProviderMap.Add("庫存盤盈", income => income.InventoryOverage);
            rowHeaderToValueProviderMap.Add("庫存盤虧", income => income.InventoryShortage);
            rowHeaderToValueProviderMap.Add("庫存報廢", income => income.InventoryScrapped);
            rowHeaderToValueProviderMap.Add("調劑台營業毛利", income => income.HISProfit);
        }
    }
}
