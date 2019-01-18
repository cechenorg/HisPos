﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product
{
    public class ProductDB
    {
        internal static DataTable GetProductStructsBySearchString(string searchString)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", searchString));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductStructBySearchString]", parameters);
        }
    }
}