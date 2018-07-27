using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Pharmacy
{
    public static class PharmacyDb
    {
        internal static ObservableCollection<ManagePharmacy> GetManagePharmacy()
        {
            ObservableCollection<ManagePharmacy> collection = new ObservableCollection<ManagePharmacy>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[GetManageManufactory]");

            foreach (DataRow row in table.Rows)
            {
                string parent = row["MAN_PARENT"].ToString();

                if (parent.Equals(String.Empty))
                {
                    collection.Add(new ManagePharmacy(row));
                }
                else
                {
                    ManagePharmacy phar = collection.Where(m => m.Id.Equals(parent)).ToList()[0];

                    phar.PharmacyPrincipals.Add(new PharmacyPrincipal(row));
                }
            }

            return collection;
        }
    }
}
