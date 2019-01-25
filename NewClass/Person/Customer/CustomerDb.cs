using His_Pos.Database;
using His_Pos.NewClass.Prescription;
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
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Customer]");
        }

        public static DataTable GetCustomerByCusId(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Id", cusId);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerByCusId]", parameterList); 
        }
        
        public static DataTable CheckCustomer(Customer customer)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_IDNumber", customer.IDNumber);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Name", customer.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Birthday", customer.Birthday);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Telephone", customer.Tel); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomer]", parameterList);  
        }
        public static DataTable GetDataByCondition(Customer customer) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_IDNumber", customer.IDNumber);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Name", customer.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Birthday", customer.Birthday);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Telephone", customer.Tel); 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CustomerByCondition]", parameterList); 
        }
        public static void UpdateEditTime(int cusId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_id", cusId); 
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCustomerEditTime]", parameterList);
        }
        public static void Save(Customer customer)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            DataBaseFunction.AddSqlParameter(parameterList, "Customer", SetCustomer(customer)); 
            MainWindow.ServerConnection.ExecuteProc("[Set].[SaveCustomer]", parameterList); 
        }
        public static DataTable SetCustomer(Customer c) {
            DataTable customerTable = CustomerTable();
            DataRow newRow = customerTable.NewRow();  
            DataBaseFunction.AddColumnValue(newRow, "Cus_ID",c.Id);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Name", c.Name);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Gender", c.Gender);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Birthday", c.Birthday);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Address", c.Address);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Telephone", c.Tel);
            DataBaseFunction.AddColumnValue(newRow, "Cus_CellPhone",c.CellPhone);
            DataBaseFunction.AddColumnValue(newRow, "Cus_IDNumber", c.IDNumber);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Email",c.Email);
            DataBaseFunction.AddColumnValue(newRow, "Cus_LINE", c.Line);
            DataBaseFunction.AddColumnValue(newRow, "Cus_UrgentNote",c.ContactNote);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Note", c.Note);
            customerTable.Rows.Add(newRow);
            return customerTable;
        }
        public static DataTable SetCustomersByPrescriptions(Prescriptions ps) {
            DataTable table = CustomerTable();
            foreach (var p in ps) {
                DataRow newRow = table.NewRow(); 
                DataBaseFunction.AddColumnValue(newRow, "Cus_Name", p.Patient.Name);
                DataBaseFunction.AddColumnValue(newRow, "Cus_Gender", p.Patient.Gender);
                DataBaseFunction.AddColumnValue(newRow, "Cus_Birthday", p.Patient.Birthday); 
                DataBaseFunction.AddColumnValue(newRow, "Cus_IDNumber", p.Patient.IDNumber);
                table.Rows.Add(newRow);
            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "@Customers", table);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomers]", parameterList); 
        }
        public static DataTable CustomerTable()
        {
            DataTable customerTable = new DataTable();
            customerTable.Columns.Add("Cus_ID", typeof(int));
            customerTable.Columns.Add("Cus_Name", typeof(String));
            customerTable.Columns.Add("Cus_Gender", typeof(String));
            customerTable.Columns.Add("Cus_Birthday", typeof(DateTime));
            customerTable.Columns.Add("Cus_Address", typeof(String));
            customerTable.Columns.Add("Cus_Telephone", typeof(String));
            customerTable.Columns.Add("Cus_CellPhone", typeof(String));
            customerTable.Columns.Add("Cus_IDNumber", typeof(String));
            customerTable.Columns.Add("Cus_Email", typeof(String));
            customerTable.Columns.Add("Cus_LINE", typeof(String));
            customerTable.Columns.Add("Cus_UrgentNote", typeof(String)); 
            customerTable.Columns.Add("Cus_Note", typeof(String)); 
            return customerTable;
        }
    }
}
