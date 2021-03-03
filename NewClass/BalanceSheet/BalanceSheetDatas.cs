using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.BalanceSheet
{
    public class BalanceSheetDatas : Collection<BalanceSheetData>
    {
        public BalanceSheetDatas(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new BalanceSheetData(row));
            }
        }
    }
}