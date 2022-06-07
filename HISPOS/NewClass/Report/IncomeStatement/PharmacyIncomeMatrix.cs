using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class PharmacyIncomeMatrix : MatrixLib.Matrix.MatrixBase<string, PharmacyIncome>
    {
        public PharmacyIncomeMatrix()
        {
        }

        public PharmacyIncomeMatrix(DataTable declareIncomeTable, DataTable pharmacyCostTable, DataTable cooperativeHeaderTable)
        {
            rowHeaderToValueProviderMap = new Dictionary<string, CellValueProvider>();
            cooperativeInstitutionIncomeHeaderTable = cooperativeHeaderTable;
            pharmacyIncomes = new PharmacyIncome[12];
            for (var i = 0; i < pharmacyIncomes.Length; i++)
                pharmacyIncomes[i] = new PharmacyIncome(i + 1);
            SetDeclareIncome(declareIncomeTable);
            SetPharmacyCost(pharmacyCostTable);
            PopulateCellValueProviderMap();
        }

        #region Fields

        private readonly PharmacyIncome[] pharmacyIncomes;
        private readonly Dictionary<string, CellValueProvider> rowHeaderToValueProviderMap;

        private delegate object CellValueProvider(PharmacyIncome pharmacyIncome);

        private DataTable cooperativeInstitutionIncomeHeaderTable { get; set; }

        #endregion Fields

        protected override IEnumerable<PharmacyIncome> GetColumnHeaderValues()
        {
            return pharmacyIncomes;
        }

        protected override IEnumerable<string> GetRowHeaderValues()
        {
            return rowHeaderToValueProviderMap.Keys;
        }

        protected override object GetCellValue(string rowHeaderValue, PharmacyIncome columnHeaderValue)
        {
            return rowHeaderToValueProviderMap[rowHeaderValue](columnHeaderValue);
        }

        private void PopulateCellValueProviderMap()
        {
            rowHeaderToValueProviderMap.Add("慢箋健保收入", income => income.ChronicIncome);
            rowHeaderToValueProviderMap.Add("一般箋健保收入", income => income.NormalIncome);
            rowHeaderToValueProviderMap.Add("其他收入", income => income.OtherIncome);

            foreach (DataRow row in cooperativeInstitutionIncomeHeaderTable.Rows)
            {
                var index = cooperativeInstitutionIncomeHeaderTable.Rows.IndexOf(row) / 3;
                var headerName = row.Field<string>("IncomeName");
                if (headerName.Contains("合作藥服"))
                {
                    rowHeaderToValueProviderMap.Add(
                        row.Field<string>("IncomeName"), income => income.CooperativeInstitutionsIncome[index].MedicalServiceIncome);
                }
                else if (headerName.Contains("合作藥品"))
                {
                    rowHeaderToValueProviderMap.Add(
                        row.Field<string>("IncomeName"), income => income.CooperativeInstitutionsIncome[index].MedicineIncome);
                }
                else if (headerName.Contains("合作其他"))
                {
                    rowHeaderToValueProviderMap.Add(
                        row.Field<string>("IncomeName"), income => income.CooperativeInstitutionsIncome[index].OtherIncome);
                }
            }

            rowHeaderToValueProviderMap.Add("慢箋銷貨成本", income => income.ChronicCost);
            rowHeaderToValueProviderMap.Add("一般銷貨成本", income => income.NormalCost);
        }

        private void SetDeclareIncome(DataTable declareIncomeTable)
        {
            var rowCount = 0;
            foreach (DataRow row in declareIncomeTable.Rows)
            {
                var month = 1;
                foreach (var income in pharmacyIncomes)
                {
                    SetDeclareIncomeField(income, row, rowCount, month);
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
                foreach (var income in pharmacyIncomes)
                {
                    SetPharmacyCostField(income, row, rowCount, month);
                    month++;
                }
                rowCount++;
            }
        }

        private void SetDeclareIncomeField(PharmacyIncome income, DataRow row, int rowCount, int month)
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

        private void SetPharmacyCostField(PharmacyIncome income, DataRow row, int rowCount, int month)
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

        private void SetCooperativeDeclareIncome(PharmacyIncome income, DataRow row, int rowCount, int month)
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

        public List<decimal> GetChronicProfits()
        {
            return pharmacyIncomes.Select(income => income.ChronicProfit).ToList();
        }
    }
}