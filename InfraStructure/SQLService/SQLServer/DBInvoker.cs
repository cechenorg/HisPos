using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.SQLService.SQLServer
{
    public class DBInvoker
    {
        public static string GetTableColumns(string[] columns)
        {
            return string.Join(",", columns);
        }

        public static string GetTableParameterColumns(string[] columns)
        {
            return "@" + string.Join(",@", columns);
        }

        public static string GetUpdateTableColumns(string[] columns)
        {
            var updateString = columns.Aggregate("", (current, column) => current + $"{column} = @{column},");

            return updateString.Trim(',');
        }
    }
}
