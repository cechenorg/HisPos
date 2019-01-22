﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Database {
    public static class DataBaseFunction {
        public static void AddColumnValue(DataRow row,string column,Object value) {
            if (value is null || string.IsNullOrEmpty(value.ToString())) 
                row[column] = DBNull.Value;
            else 
                row[column] = value; 
        }
        public static void AddSqlParameter(List<SqlParameter> row, string column, Object value)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                row.Add(new SqlParameter(column, DBNull.Value));
            else
                row.Add(new SqlParameter(column, value)); 
        }
    }
}
