using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Person
{
    public static class PersonDb
    {
        internal static ObservableCollection<Person> GetUserCollection()
        {
            ObservableCollection<Person> collection = new ObservableCollection<Person>();

            var dd = new DbConnection(Settings.Default.SQL_local);

            var table = dd.ExecuteProc("[HIS_POS_DB].[ProductPurchaseView].[GetEmp]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add( new Person(row));
            }

            return collection;
        }

        internal static User CheckUserPassword(string id, string password)
        {
            User user = new User();

            var dd = new DbConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", id));
            parameters.Add(new SqlParameter("PASSWORD", password));

            var table = dd.ExecuteProc("[HIS_POS_DB].[LoginView].[CheckUserPassword]", parameters);

            foreach (DataRow row in table.Rows)
            {
                user = new User(row);
            }

            return user;
        }
    }
}
