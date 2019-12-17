using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class IncomeStatementMatrix : MatrixLib.Matrix.MatrixBase<string,IncomeStatement>
    {
        public IncomeStatementMatrix()
        {

        }

        public IncomeStatementMatrix(int year)
        {
            //var t = new DataTable();
            //incomeStatement = new IncomeStatement[12];
            //for (var i = 0; i < incomeStatement.Length; i++)
            //    incomeStatement[i] = new IncomeStatement();
            //foreach (var VARIABLE in COLLECTION)
            //{
                
            //}
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

        #endregion
    }
}
