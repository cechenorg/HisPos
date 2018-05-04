using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.AbstractClass;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Person
{
    public static class PersonDb
    {
        internal static ObservableCollection<Person> GetUserCollection()
        {
            ObservableCollection<Person> collection = new ObservableCollection<Person>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetEmp]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add( new Person(row));
            }

            return collection;
        }
    }
}
