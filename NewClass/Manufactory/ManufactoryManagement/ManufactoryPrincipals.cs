using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryPrincipals : ObservableCollection<ManufactoryPrincipal>, ICloneable
    {
        private ManufactoryPrincipals() { }
        public ManufactoryPrincipals(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ManufactoryPrincipal(row));
            }
        }

        internal static ManufactoryPrincipals GetManufactoryPrincipals(string manufactoryID)
        {
            return new ManufactoryPrincipals(ManufactoryDB.GetManufactoryPrincipals(manufactoryID));
        }

        public object Clone()
        {
            ManufactoryPrincipals principals = new ManufactoryPrincipals();

            foreach (var principal in Items)
                principals.Add(principal.Clone() as ManufactoryPrincipal);

            return principals;
        }

        internal void AddNewPrincipal()
        {
            Add(new ManufactoryPrincipal());
        }

        internal void ResetData(ManufactoryPrincipals principals)
        {
            Clear();

            foreach (var principal in principals)
            {
                Add(principal.Clone() as ManufactoryPrincipal);
            }
        }
    }
}
