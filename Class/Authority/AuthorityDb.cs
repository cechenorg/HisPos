using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.H4_BASIC_MANAGE.AuthenticationManage;
using System.Collections.ObjectModel;
using static His_Pos.H4_BASIC_MANAGE.AuthenticationManage.AuthenticationManageView;
using System.Data;

namespace His_Pos.Class.Authority
{
    public static class AuthorityDb
    {
        internal static void ChangeAuthLeaveStatus(bool status)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("STATUS", status));

            dd.ExecuteProc("[HIS_POS_DB].[AuthenticationManageView].[ChangeAuthLeaveStatus]", parameters);
        }

        internal static Collection<AuthStatus> GetAuthStatus()
        {
            Collection<AuthStatus> collection = new Collection<AuthStatus>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            
            var table = dd.ExecuteProc("[HIS_POS_DB].[AuthenticationManageView].[GetAuthStatus]");

            foreach(DataRow row in table.Rows)
            {
                collection.Add( new AuthStatus(row));
            }

            return collection;
        }
    }
}
