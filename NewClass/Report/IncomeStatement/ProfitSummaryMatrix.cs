using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class ProfitSummaryMatrix : MatrixLib.Matrix.MatrixBase<string, decimal>
    {
        public ProfitSummaryMatrix(string header,IReadOnlyList<decimal> profitList)
        {
            rowHeader = header;
            rowHeaderToValueProviderMap = new Dictionary<string, CellValueProvider>();
            profits = new decimal[12];
            for (var i = 0; i < 12; i++)
            {
                profits[i] = profitList[i];
            }
            PopulateCellValueProviderMap();
        }

        #region Fields
        private readonly decimal[] profits;
        private readonly Dictionary<string, CellValueProvider> rowHeaderToValueProviderMap;
        private delegate object CellValueProvider(decimal chronicProfit);
        private readonly string rowHeader;
        #endregion
        protected override IEnumerable<decimal> GetColumnHeaderValues()
        {
            return profits;
        }

        protected override IEnumerable<string> GetRowHeaderValues()
        {
            return rowHeaderToValueProviderMap.Keys;
        }

        protected override object GetCellValue(string rowHeaderValue, decimal columnHeaderValue)
        {
            return rowHeaderToValueProviderMap[rowHeaderValue](columnHeaderValue);
        }

        void PopulateCellValueProviderMap()
        {
            rowHeaderToValueProviderMap.Add(rowHeader, profit => profit);
        }
    }
}
