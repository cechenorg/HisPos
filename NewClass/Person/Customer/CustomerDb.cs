using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Person.Customer
{
    public static class CustomerDb
    {
        public static DataTable GetData()
        {
            var table = new DataTable();
            return table;
        }

        public static DataTable GetCustomerByCusId(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Cus_Id", cusId));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerByCusId]", parameterList);
            return table;
        }
        public static DataTable CheckCustomer(Customer customer)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Cus_IDNumber", customer.IDNumber));
            parameterList.Add(new SqlParameter("Cus_Name", customer.Name));
            parameterList.Add(customer.Birthday is null
                ? new SqlParameter("Cus_Birthday", DBNull.Value)
                : new SqlParameter("Cus_Birthday", customer.Birthday));
            parameterList.Add(string.IsNullOrEmpty(customer.Tel)
                ? new SqlParameter("Cus_Telephone", DBNull.Value)
                : new SqlParameter("Cus_Telephone", customer.Tel));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomer]", parameterList); 
            return table;
        }
    }
}
