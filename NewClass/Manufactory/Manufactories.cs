using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Manufactory
{
    public class Manufactories : Collection<Manufactory>
    {
        private Manufactories(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new Manufactory(row));
            }
        }

        internal static Manufactories GetManufactories()
        {
            return new Manufactories(ManufactoryDB.GetAllManufactories());
        }
    }
}
