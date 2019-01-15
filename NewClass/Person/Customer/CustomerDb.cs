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
        public static DataTable GetDataByCondition(Customer customer) {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            if (string.IsNullOrEmpty(customer.IDNumber))
                parameterList.Add(new SqlParameter("Cus_IDNumber", DBNull.Value));
            else
                parameterList.Add(new SqlParameter("Cus_IDNumber", customer.IDNumber));

            if (string.IsNullOrEmpty(customer.Name))
                parameterList.Add(new SqlParameter("Cus_Name", DBNull.Value));
            else
                parameterList.Add(new SqlParameter("Cus_Name", customer.Name));

            if (customer.Birthday is null)
                parameterList.Add(new SqlParameter("Cus_Birthday", DBNull.Value));
            else
                parameterList.Add(new SqlParameter("Cus_Birthday", customer.Birthday));

            if (string.IsNullOrEmpty(customer.Tel))
                parameterList.Add(new SqlParameter("Cus_Telephone", DBNull.Value));
            else
                parameterList.Add(new SqlParameter("Cus_Telephone", customer.Tel));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerByCondition]", parameterList);
            return table;
        }
        public static void UpdateEditTime(int cusId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Cus_id", cusId));  
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCustomerEditTime]", parameterList);
        } 
    }
}
