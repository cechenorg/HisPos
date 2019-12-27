using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.BalanceSheet
{
    public class StrikeDatas : Collection<StrikeData>
    {
        public StrikeDatas(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new StrikeData(row));
            }
        }
    }
}
