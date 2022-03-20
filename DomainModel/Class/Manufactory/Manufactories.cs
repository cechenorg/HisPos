using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Manufactory
{
    public class Manufactories : Collection<Manufactory>
    {
        public Manufactories(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new Manufactory(row));
            }
        }

        public Manufactories()
        {
        }
         
    }
}