using Dapper;
using His_Pos.Database;
using His_Pos.NewClass.Prescription.ImportDeclareXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace His_Pos.NewClass.Person.Customer
{
    public static class CustomerDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Customer]");
        }

        public static Customer GetCustomerByCusId(int cusId)
        {
            Customer result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.QueryFirstOrDefault<Customer>(
                    $"{Properties.Settings.Default.SystemSerialNumber}.[Get].[CustomerByCusId]",
                    param: new
                    { Cus_Id = cusId, },
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }

        public static DataTable CheckCustomer(Customer customer)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_IDNumber",
                string.IsNullOrEmpty(customer.IDNumber) ? null : customer.IDNumber);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Name", customer.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Birthday", customer.Birthday);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Telephone", customer.Tel);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomer]", parameterList);
        }

        public static void UpdateEditTime(int cusId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_id", cusId);
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateCustomerEditTime]", parameterList);
        }

        public static bool Save(Customer customer)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Customer", SetCustomer(customer));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[SaveCustomer]", parameterList);

            if (result.Rows.Count > 0)
            {
                return result.Rows[0][0].ToString() == "1";
            }

            return false;
        }

        public static DataTable SetCustomer(Customer c)
        {
            DataTable customerTable = CustomerTable();
            DataRow newRow = customerTable.NewRow();
            if (c != null)
            {
                if (!string.IsNullOrEmpty(c.IDNumber))
                    c.CheckGender();
            }

            DataBaseFunction.AddColumnValue(newRow, "Cus_ID", c.ID);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Name", c.Name);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Gender", c.Gender);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Birthday", c.Birthday);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Address", c.Address);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Telephone", c.Tel);
            DataBaseFunction.AddColumnValue(newRow, "Cus_CellPhone", c.CellPhone);
            DataBaseFunction.AddColumnValue(newRow, "Cus_IDNumber", c.IDNumber);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Email", c.Email);
            DataBaseFunction.AddColumnValue(newRow, "Cus_LINE", c.Line);
            DataBaseFunction.AddColumnValue(newRow, "Cus_UrgentNote", c.ContactNote);
            DataBaseFunction.AddColumnValue(newRow, "Cus_Note", c.Note);
            DataBaseFunction.AddColumnValue(newRow, "Cus_SecondPhone", c.SecondPhone);
            DataBaseFunction.AddColumnValue(newRow, "Cus_IsEnable", c.IsEnable);

            customerTable.Rows.Add(newRow);
            return customerTable;
        }

        public static DataTable SetCustomersByPrescriptions(List<ImportDeclareXml.Ddata> Ddatas)
        {
            DataTable table = CustomerTable();
            foreach (var d in Ddatas)
            {
                DataRow newRow = table.NewRow();
                DataBaseFunction.AddColumnValue(newRow, "Cus_Name", d.D20);
                DataBaseFunction.AddColumnValue(newRow, "Cus_Gender", d.D3.Substring(1, 1) == "2" ? "男" : "女");
                DataBaseFunction.AddColumnValue(newRow, "Cus_Birthday",
                    Convert.ToDateTime((Convert.ToInt32(d.D6.Substring(0, 3)) + 1911).ToString() + "/" +
                                       d.D6.Substring(3, 2) + "/" + d.D6.Substring(5, 2)));
                DataBaseFunction.AddColumnValue(newRow, "Cus_IDNumber", d.D3);
                table.Rows.Add(newRow);
            }

            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Customers", table);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomers]", parameterList);
        }

        public static IEnumerable<Customer> GetDataByNameOrBirth(string name, DateTime? date, string idNumber, string phoneNumber)
        {
            List<Customer> result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<Customer>(
                    $"{Properties.Settings.Default.SystemSerialNumber}.[Get].[CustomerByNameOrBirth]",
                    param: new
                    {
                        CusName = name,
                        CusBirth = date,
                        IdNumber = idNumber,
                        PhoneNumber = phoneNumber
                    },
                    commandType: CommandType.StoredProcedure).ToList();
            });

            return result;

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
            customerTable.Columns.Add("Cus_SecondPhone", typeof(String));
            customerTable.Columns.Add("Cus_IsEnable", typeof(bool));


            return customerTable;
        }

        public static DataTable GetCustomerCountByCustomer(Customer c)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_IDNumber", c.IDNumber);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Name", c.Name);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Birthday", c.Birthday);
            DataBaseFunction.AddSqlParameter(parameterList, "Cus_Telephone", c.Tel);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomerExist]", parameterList);
        }

        public static DataTable InsertNewCustomerData(Customer c, int isCheckTel)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Customer", SetCustomer(c));
            DataBaseFunction.AddSqlParameter(parameterList, "IsCheckTel", isCheckTel);
            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertNewCustomer]", parameterList);
        }


        public static IEnumerable<Customer> SearchCustomers(string idNumber, string name, string cellPhone, string tel,
            DateTime? birth)
        {
            List<Customer> result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<Customer>(
                    $"{Properties.Settings.Default.SystemSerialNumber}.[Get].[SearchCustomer]",
                    param: new
                    {
                        Cus_IDNumber = idNumber,
                        Cus_Name = name,
                        Cus_Birthday = birth,
                        Cus_CellPhone = cellPhone,
                        Cus_Telephone = tel
                    },
                    commandType: CommandType.StoredProcedure).ToList();
            });

            return result;
        }

        public static Customer CheckCustomerByCard(string idNumber)
        {
            Customer result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.QueryFirstOrDefault<Customer>(
                    $"{Properties.Settings.Default.SystemSerialNumber}.[Get].[CheckCustomerByCard]",
                    param: new
                    {
                        Cus_IDNumber = idNumber,
                    },
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }


        public static DataTable CheckCustomerByPhone(string cell, string tel)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "cell", cell);
            DataBaseFunction.AddSqlParameter(parameterList, "tel", tel);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomerByPhone]", parameterList);
        }

        public static DataTable UpdateCustomerByPhone(int id_OG, int id, string cell, string tel)
        {
            var parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "id_OG", id_OG);
            DataBaseFunction.AddSqlParameter(parameterList, "id", id);
            DataBaseFunction.AddSqlParameter(parameterList, "cell", cell);
            DataBaseFunction.AddSqlParameter(parameterList, "tel", tel);
            return MainWindow.ServerConnection.ExecuteProc("[set].[UpdateCustomerByCard]", parameterList);
        }

        public static DataTable GetTodayEdited()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[TodayEditedCustomers]");
        }
    }
}