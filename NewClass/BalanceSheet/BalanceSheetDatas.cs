using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
