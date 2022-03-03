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

        protected Manufactories()
        {
        }

        internal static Manufactories GetManufactories()
        {
            return new Manufactories(ManufactoryDB.GetAllManufactories());
        }

        internal static Manufactories GetControlMedicineManufactories()
        {
            DataTable table = ManufactoryDB.GetAllManufactories();
            Manufactories manufactories = new Manufactories();
            foreach (DataRow row in table.Rows)
            {
                ControlManufactory manufactory = new ControlManufactory(row);
                if (!string.IsNullOrEmpty(manufactory.ControlmedicineID))
                    manufactories.Add(manufactory);
            }
            return manufactories;
        }
    }
}