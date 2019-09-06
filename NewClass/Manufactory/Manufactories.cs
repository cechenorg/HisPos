using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Manufactory
{
    public class Manufactories : Collection<Manufactory>
    {
        private Manufactories(DataTable dataTable,bool isControl)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Manufactory manufactory = new Manufactory(row);
                switch (isControl) {
                    case true:
                        if(!string.IsNullOrEmpty(manufactory.ControlmedicineID))
                            Add(manufactory);
                        break;
                    case false:
                        Add(manufactory);
                        break;

                } 
            }
        }

        internal static Manufactories GetManufactories()
        {
            return new Manufactories(ManufactoryDB.GetAllManufactories(), false);
        }
        internal static Manufactories GetControlMedicineManufactories() {
            return new Manufactories(ManufactoryDB.GetAllManufactories(), true);
        }
    }
}
