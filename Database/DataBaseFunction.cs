using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Database {
    public static class DataBaseFunction {
        public static void AddColumnValue<T>(DataRow row,string column,T value) {
            row[column] = value;
            bool canBeNull = !value.GetType().IsValueType || (Nullable.GetUnderlyingType(value.GetType()) != null);

            if(canBeNull)
            {
                if (value == null)
                    row[column] = DBNull.Value;
                else
                    row[column] = value;
            }
            else
            {
                row[column] = value;
            }
        }
        public static void AddSqlParameter(List<SqlParameter> row, string column, Object value)
        { 
            bool canBeNull = !value.GetType().IsValueType || (Nullable.GetUnderlyingType(value.GetType()) != null);

            if (canBeNull)
            {
                if (value == null)
                    row.Add(new SqlParameter(column,DBNull.Value));
                else
                    row.Add(new SqlParameter(column, value));
            }
            else
            {
                row.Add(new SqlParameter(column, value));
            }
        }
    }
}
