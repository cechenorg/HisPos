using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Person.Employee
{ 
    public static class EmployeeDb
    {
        public static  DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Employee]"); 
        }
        public static DataTable Save(Employee e)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>(); 
            parameterList.Add(new SqlParameter("Employee", SetCustomer(e))); 
            return MainWindow.ServerConnection.ExecuteProc("[Set].[SaveEmployee]", parameterList); 
        }
        public static void Delete(int empId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("EmpId", empId));
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteEmployee]", parameterList); 
        }
        public static DataTable EmployeeLogin(string account,string password) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Account", account));
            parameterList.Add(new SqlParameter("Password", password));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeLogin]",parameterList);  
            return table;
        }
        public static DataTable GetTabAuth(int AuthValue) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("AuthValue", AuthValue)); 
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[TabAuth]", parameterList);
            return table;
        }
        public static DataTable GetPassword(int empId) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("EmpId", empId));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeePassword]", parameterList); 
        }
        public static void ChangePassword(int empid, string password)
        { 
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "EmpId", empid); 
            DataBaseFunction.AddSqlParameter(parameterList, "Password", password);
            MainWindow.ServerConnection.ExecuteProc("[Set].[ChangeEmployeePassword]", parameterList);
        }
        public static DataTable SetCustomer(Employee e)
        {
            DataTable employeeTable = EmployeeTable();
            DataRow newRow = employeeTable.NewRow();
            if (e.ID == 0)
                newRow["Emp_ID"] = DBNull.Value;
            else
                newRow["Emp_ID"] = e.ID;
            DataBaseFunction.AddColumnValue(newRow,"Emp_Name",e.Name);
            DataBaseFunction.AddColumnValue(newRow,"Emp_NickName", e.NickName);
            DataBaseFunction.AddColumnValue(newRow,"Emp_Gender", e.Gender);
            DataBaseFunction.AddColumnValue(newRow,"Emp_IDNumber", e.IDNumber);
            DataBaseFunction.AddColumnValue(newRow,"Emp_BirthDay", e.Birthday);
            DataBaseFunction.AddColumnValue(newRow,"Emp_Address", e.Address);
            DataBaseFunction.AddColumnValue(newRow,"Emp_Telephone", e.Tel);
            DataBaseFunction.AddColumnValue(newRow,"Emp_Cellphone", e.CellPhone);
            DataBaseFunction.AddColumnValue(newRow,"Emp_Email", e.Email);
            DataBaseFunction.AddColumnValue(newRow,"Emp_LINE", e.Line);
            DataBaseFunction.AddColumnValue(newRow,"Emp_WorkPositionID", e.WorkPositionID);
            DataBaseFunction.AddColumnValue(newRow,"Emp_StartDate", e.StartDate);
            DataBaseFunction.AddColumnValue(newRow,"Emp_LeaveDate", e.LeaveDate);
            DataBaseFunction.AddColumnValue(newRow,"Emp_PurchaseLimit", e.PurchaseLimit);
            DataBaseFunction.AddColumnValue(newRow,"Emp_Note", e.Note);
            DataBaseFunction.AddColumnValue(newRow, "Emp_IsEnable", e.IsEnable); 
            employeeTable.Rows.Add(newRow);
            return employeeTable;
        }
        public static DataTable EmployeeTable() {
            DataTable employeeTable = new DataTable();
            employeeTable.Columns.Add("Emp_ID", typeof(int));
            employeeTable.Columns.Add("Emp_Name", typeof(String));
            employeeTable.Columns.Add("Emp_NickName", typeof(String));
            employeeTable.Columns.Add("Emp_Gender", typeof(String));
            employeeTable.Columns.Add("Emp_IDNumber", typeof(String));
            employeeTable.Columns.Add("Emp_BirthDay", typeof(DateTime));
            employeeTable.Columns.Add("Emp_Address", typeof(String));
            employeeTable.Columns.Add("Emp_Telephone", typeof(String));
            employeeTable.Columns.Add("Emp_Cellphone", typeof(String));
            employeeTable.Columns.Add("Emp_Email", typeof(String));
            employeeTable.Columns.Add("Emp_LINE", typeof(String));
            employeeTable.Columns.Add("Emp_WorkPositionID", typeof(int));
            employeeTable.Columns.Add("Emp_StartDate", typeof(DateTime));
            employeeTable.Columns.Add("Emp_LeaveDate", typeof(DateTime));
            employeeTable.Columns.Add("Emp_PurchaseLimit", typeof(int));
            employeeTable.Columns.Add("Emp_Note", typeof(String));
            employeeTable.Columns.Add("Emp_IsEnable", typeof(bool));
            return employeeTable; 
        }
    }
}
