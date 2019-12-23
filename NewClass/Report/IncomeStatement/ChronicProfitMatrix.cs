using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class ChronicProfitMatrix : MatrixLib.Matrix.MatrixBase<string, decimal>
    {
        public ChronicProfitMatrix(List<decimal> profitList)
        {
            rowHeaderToValueProviderMap = new Dictionary<string, CellValueProvider>();
            chronicProfits = new decimal[12];
            for (var i = 0; i < 12; i++)
            {
                chronicProfits[i] = profitList[i];
            }
            PopulateCellValueProviderMap();
        }

        #region Fields
        private readonly decimal[] chronicProfits;
        readonly Dictionary<string, CellValueProvider> rowHeaderToValueProviderMap;
        private delegate object CellValueProvider(decimal chronicProfit);
        #endregion
        protected override IEnumerable<decimal> GetColumnHeaderValues()
        {
            return chronicProfits;
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
            rowHeaderToValueProviderMap.Add("慢箋營業毛利", chronicProfit => chronicProfit);
        }
    }
}
