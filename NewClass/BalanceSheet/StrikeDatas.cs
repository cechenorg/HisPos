using System.Collections.ObjectModel;
using System.Data;

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