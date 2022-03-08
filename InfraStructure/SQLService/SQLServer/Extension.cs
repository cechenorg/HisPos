using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace InfraStructure.SQLService.SQLServer
{
    public static class Extension
    {
        public static void AddColumnValue(DataRow row, string column, Object value)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                row[column] = DBNull.Value;
            else
                row[column] = value;
        }

        public static void AddSqlParameter(List<SqlParameter> row, string column, Object value)
        {
            if (value == null)
                row.Add(new SqlParameter(column, DBNull.Value));
            else
            {
                bool canBeNull = !value.GetType().IsValueType || (Nullable.GetUnderlyingType(value.GetType()) != null);

                if (canBeNull)
                {
                    if (value == null)
                        row.Add(new SqlParameter(column, DBNull.Value));
                    else
                        row.Add(new SqlParameter(column, value));
                }
                else
                {
                    row.Add(new SqlParameter(column, value));
                }
            }
        }

        public static DataTable ExecuteToDataTable(this SqlConnection conn,string sql, object param = null)
        {
            var dataTable = new DataTable();
            dataTable.Load(conn.ExecuteReader(sql, param));
            return dataTable;
        }
    }
}
