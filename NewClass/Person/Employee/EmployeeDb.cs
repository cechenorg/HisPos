using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Person.Employee
{
    public static class EmployeeDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Employee]");
        }

        public static DataTable GetDataByID(int ID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@EmpID", ID));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeByID]", parameterList);
        }

        public static void Insert(Employee e)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Employee", SetCustomer(e)));
            if (string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName))
                MainWindow.ServerConnection.ExecuteProc("[Set].[InsertEmployee]", parameterList);
            else
            {
                MainWindow.ServerConnection.ExecuteProcBySchema(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[InsertEmployee]", parameterList);
                SyncData();
                UpdateIsLocal(e.IDNumber, true);
            }
        }

        public static void Update(Employee e)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Employee", SetCustomer(e)));
            if (string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName))
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateEmployee]", parameterList);
            else
            {
                MainWindow.ServerConnection.ExecuteProcBySchema(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[UpdateEmployee]", parameterList);
                SyncData();
            }
        }

        public static void Delete(int empId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("EmpId", empId));
            if (string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName))
                MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteEmployee]", parameterList);
            else
            {
                MainWindow.ServerConnection.ExecuteProcBySchema(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[DeleteEmployee]", parameterList);
                SyncData();
            }
        }

        public static DataTable GetEmployeeNewAccount()
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeNewAccount]", parameterList);
        }

        public static DataTable EmployeeLogin(string account, string password)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Account", account));
            parameterList.Add(new SqlParameter("Password", password));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeLogin]", parameterList);
            return table;
        }

        public static DataTable CheckIdNumber(string idNumber)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("IdNumber", idNumber));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[EmployeeCheckIdNumber]", parameterList);
            return table;
        }

        public static DataTable CheckEmployeeAccountSame(string account)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Account", account));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[CheckEmployeeAccountSame]", parameterList);
            return table;
        }

        public static DataTable GetTabAuth(int AuthValue)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("AuthValue", AuthValue));

            var table = string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName)
                 ? MainWindow.ServerConnection.ExecuteProcBySchema("HIS_POS_Server", "[Get].[TabAuth]", parameterList)
                 : MainWindow.ServerConnection.ExecuteProcBySchema(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Get].[TabAuth]", parameterList);
            return table;
        }

        public static void SyncData()
        {
            MainWindow.ServerConnection.ExecuteProc("[Set].[SyncEmployee]");
        }

        public static void UpdateIsLocal(string idNumber, bool isLocal)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "IdNumber", idNumber);
            DataBaseFunction.AddSqlParameter(parameterList, "isLocal", isLocal);
            MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateIsLocal]", parameterList);
        }

        public static DataTable GetEnableMedicalPersonnels(DateTime selectedDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Date", selectedDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[EnablePharmacists]", parameterList);
        }

        public static DataTable SetCustomers(Employees es)
        {
            DataTable employeeTable = EmployeeTable();

            foreach (Employee e in es)
            {
                DataRow newRow = employeeTable.NewRow();
                if (e.ID == 0)
                    newRow["Emp_ID"] = DBNull.Value;
                else
                    newRow["Emp_ID"] = e.ID;
                DataBaseFunction.AddColumnValue(newRow, "Emp_Account", e.Account);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Password", e.Password);
                DataBaseFunction.AddColumnValue(newRow, "Emp_AuthorityLevel", e.AuthorityValue);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Name", e.Name);
                DataBaseFunction.AddColumnValue(newRow, "Emp_NickName", e.NickName);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Gender", e.Gender);
                DataBaseFunction.AddColumnValue(newRow, "Emp_IDNumber", e.IDNumber);
                DataBaseFunction.AddColumnValue(newRow, "Emp_BirthDay", e.Birthday);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Address", e.Address);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Telephone", e.Tel);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Cellphone", e.CellPhone);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Email", e.Email);
                DataBaseFunction.AddColumnValue(newRow, "Emp_LINE", e.Line);
                DataBaseFunction.AddColumnValue(newRow, "Emp_WorkPositionID", e.WorkPosition.WorkPositionId);
                DataBaseFunction.AddColumnValue(newRow, "Emp_StartDate", e.StartDate);
                DataBaseFunction.AddColumnValue(newRow, "Emp_LeaveDate", e.LeaveDate);
                DataBaseFunction.AddColumnValue(newRow, "Emp_PurchaseLimit", e.PurchaseLimit);
                DataBaseFunction.AddColumnValue(newRow, "Emp_Note", e.Note);
                DataBaseFunction.AddColumnValue(newRow, "Emp_IsEnable", e.IsEnable);
                employeeTable.Rows.Add(newRow);
            }
            return employeeTable;
        }

        public static DataTable SetCustomer(Employee e)
        {
            DataTable employeeTable = EmployeeTable();
            DataRow newRow = employeeTable.NewRow();
            if (e.ID == 0)
                newRow["Emp_ID"] = DBNull.Value;
            else
                newRow["Emp_ID"] = e.ID;
            DataBaseFunction.AddColumnValue(newRow, "Emp_Account", e.Account);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Password", e.Password);
            DataBaseFunction.AddColumnValue(newRow, "Emp_AuthorityLevel", e.AuthorityValue);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Name", e.Name);
            DataBaseFunction.AddColumnValue(newRow, "Emp_NickName", e.NickName);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Gender", e.Gender);
            DataBaseFunction.AddColumnValue(newRow, "Emp_IDNumber", e.IDNumber);
            DataBaseFunction.AddColumnValue(newRow, "Emp_BirthDay", e.Birthday);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Address", e.Address);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Telephone", e.Tel);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Cellphone", e.CellPhone);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Email", e.Email);
            DataBaseFunction.AddColumnValue(newRow, "Emp_LINE", e.Line);
            DataBaseFunction.AddColumnValue(newRow, "Emp_WorkPositionID", e.WorkPosition.WorkPositionId);
            DataBaseFunction.AddColumnValue(newRow, "Emp_StartDate", e.StartDate);
            DataBaseFunction.AddColumnValue(newRow, "Emp_LeaveDate", e.LeaveDate);
            DataBaseFunction.AddColumnValue(newRow, "Emp_PurchaseLimit", e.PurchaseLimit);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Note", e.Note);
            DataBaseFunction.AddColumnValue(newRow, "Emp_IsEnable", e.IsEnable);
            employeeTable.Rows.Add(newRow);
            return employeeTable;
        }

        public static DataTable EmployeeTable()
        {
            DataTable employeeTable = new DataTable();
            employeeTable.Columns.Add("Emp_ID", typeof(int));
            employeeTable.Columns.Add("Emp_Account", typeof(string));
            employeeTable.Columns.Add("Emp_Password", typeof(string));
            employeeTable.Columns.Add("Emp_AuthorityLevel", typeof(int));
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



        internal static DataTable AddClockIn(string empID,int type)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Emp_ID", empID));
            parameters.Add(new SqlParameter("type", type));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertClockInLog]", parameters);
        }

        public static DataTable EmployeeClockInList(string WYear, string WMonth, int? EmpId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("WYear", WYear));
            parameterList.Add(new SqlParameter("WMonth", WMonth));
            var table = new DataTable();
            if (EmpId is null)
            {
                table = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInLogEmp]", parameterList);
            }
            else
            {
                parameterList.Add(new SqlParameter("EmpId", EmpId));
                table = MainWindow.ServerConnection.ExecuteProc("[Get].[ClockInEmployee]", parameterList);
            }
            return table;
        }


    }
}