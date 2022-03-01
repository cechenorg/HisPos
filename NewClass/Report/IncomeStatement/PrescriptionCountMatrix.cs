using System.Collections.Generic;
using System.Data;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class PrescriptionCountMatrix : MatrixLib.Matrix.MatrixBase<string, PrescriptionCount>
    {
        public PrescriptionCountMatrix()
        {
        }

        public PrescriptionCountMatrix(DataTable cooperativeTable, DataTable prescriptionCountTable)
        {
            rowHeaderToValueProviderMap = new Dictionary<string, CellValueProvider>();
            cooperativeInstitutionNameTable = cooperativeTable;
            prescriptionCounts = new PrescriptionCount[12];
            for (var i = 0; i < prescriptionCounts.Length; i++)
                prescriptionCounts[i] = new PrescriptionCount(i + 1);
            SetPrescriptionCount(prescriptionCountTable);
            PopulateCellValueProviderMap();
        }

        #region Fields

        private readonly PrescriptionCount[] prescriptionCounts;
        private readonly Dictionary<string, CellValueProvider> rowHeaderToValueProviderMap;

        private delegate object CellValueProvider(PrescriptionCount prescriptionCount);

        private DataTable cooperativeInstitutionNameTable { get; set; }

        #endregion Fields

        protected override IEnumerable<PrescriptionCount> GetColumnHeaderValues()
        {
            return prescriptionCounts;
        }

        protected override IEnumerable<string> GetRowHeaderValues()
        {
            return rowHeaderToValueProviderMap.Keys;
        }

        protected override object GetCellValue(string rowHeaderValue, PrescriptionCount columnHeaderValue)
        {
            return rowHeaderToValueProviderMap[rowHeaderValue](columnHeaderValue);
        }

        private void PopulateCellValueProviderMap()
        {
            rowHeaderToValueProviderMap.Add("一般箋", c => c.Count[0]);
            rowHeaderToValueProviderMap.Add("慢箋", c => c.Count[1]);

            foreach (DataRow row in cooperativeInstitutionNameTable.Rows)
            {
                var index = cooperativeInstitutionNameTable.Rows.IndexOf(row) + 2;
                rowHeaderToValueProviderMap.Add(row.Field<string>("InstitutionName"), c => c.Count[index]);
            }
        }

        private void SetPrescriptionCount(DataTable prescriptionCountTable)
        {
            foreach (DataRow row in prescriptionCountTable.Rows)
            {
                var month = 1;
                foreach (var c in prescriptionCounts)
                {
                    c.Count.Add(row.Field<int>($"{month}"));
                    month++;
                }
            }
        }
    }
}