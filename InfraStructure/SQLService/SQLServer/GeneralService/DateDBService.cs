using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace InfraStructure.SQLService.SQLServer.GeneralService
{
    public class DateDBService
    {

        //取得西元年日期 yyyyMMdd
        public string GetDateTime(SqlConnection conn,int addDay)
        {
            string sql = $@"SELECT CONVERT(NVARCHAR(8), GETDATE() + {addDay}, 112)";
            string result = conn.QueryFirst<string>(sql);

            return result;
        }
    }
}
