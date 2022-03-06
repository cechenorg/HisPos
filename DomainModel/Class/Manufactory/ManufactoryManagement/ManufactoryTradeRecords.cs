using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryTradeRecords : Collection<ManufactoryTradeRecord>
    {
        public ManufactoryTradeRecords(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ManufactoryTradeRecord(row));
            }
        }

      
    }
}