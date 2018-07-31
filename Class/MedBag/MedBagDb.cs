using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.MedBag
{
    public class MedBagDb
    {
        internal static ObservableCollection<MedBag> ObservableGetMedBagData()
        {
            ObservableCollection<MedBag> medBags = new ObservableCollection<MedBag>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("");
            foreach (DataRow row in table.Rows)
            {
                medBags.Add(new MedBag(row));
            }
            return medBags;
        }
        internal static ObservableCollection<MedBagLocation.MedBagLocation> ObservableGetLocationData()
        {
            ObservableCollection<MedBagLocation.MedBagLocation> medBagLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("");
            foreach (DataRow row in table.Rows)
            {
                medBagLocations.Add(new MedBagLocation.MedBagLocation(row));
            }
            return medBagLocations;
        }
    }
}
