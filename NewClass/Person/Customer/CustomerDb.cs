using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Person.Customer
{
    public static class CustomerDb
    {
        public static DataTable GetData(int id)
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
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Cus_IDNumber", customer.IDNumber));
            parameterList.Add(new SqlParameter("Cus_Name", customer.Name));
            parameterList.Add(new SqlParameter("Cus_Birthday", customer.Birthday));
            if(string.IsNullOrEmpty(customer.Tel))
                parameterList.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
            else
                parameterList.Add(new SqlParameter("Cus_Telephone", customer.Tel));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomer]", parameterList); 
            return table;
        }
    }
}
