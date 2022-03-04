using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.SQLService.SQLServer
{
    public abstract class SQLServerServiceBase
    {
        protected string _connectionString { get; set; }

        public SQLServerServiceBase(string connectionString,string dataBaseName)
        {
            _connectionString = connectionString + $"DataBase={dataBaseName};";
        }
    }
}
