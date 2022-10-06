using Dapper;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DomainModel;
using DomainModel.Enum;
using His_Pos.ChromeTabViewModel;
using His_Pos.Service;

namespace His_Pos.NewClass.Person.Employee
{
    public interface IEmployeeDB
    {
         void Insert(Employee e);
         IEnumerable<Employee> GetData();
         void Update(Employee e);
         void Delete(int empId);
    }

    public class EmployeeDb : IEmployeeDB
    {
        public IEnumerable<Employee> GetData()
        {
            List<Employee> result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<Employee>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[Employee]", 
                    commandType: CommandType.StoredProcedure).ToList();
            });

            return result; 
        }
         
        public static List<Employee> GetGroupPharmacyDataByID(List<string> groupserverNameList,int ID)
        {

            List<Employee> result = new List<Employee>();

            foreach(var groupserverName in groupserverNameList)
            { 
                Employee employee = null;
                 
                SQLServerConnection.DapperQuery((conn) =>
                {
                    employee = conn.Query<Employee>($"{groupserverName}.[Get].[Employee]", 
                        commandType: CommandType.StoredProcedure).SingleOrDefault(_ => _.ID == ID);
                });

                result.Add(employee);
            }

            return result;
        }

        public void Insert(Employee e)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Employee", SetCustomer(e)));
            if (string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName))
                MainWindow.ServerConnection.ExecuteProc("[Set].[InsertEmployee]", parameterList);
            else
            {
                MainWindow.ServerConnection.ExecuteProcBySchema(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[InsertEmployee]", parameterList);

                foreach (var pharmacyInfo in ViewModelMainWindow.CurrentPharmacy.GroupPharmacyinfoList)
                {
                    var tempEmployee = e.DeepCloneViaJson();

                    if (pharmacyInfo.PHAMAS_VerifyKey != ViewModelMainWindow.CurrentPharmacy.VerifyKey)
                    {
                        //其他藥局都須為支援藥師
                        if (tempEmployee.Authority == Authority.MasterPharmacist || tempEmployee.Authority == Authority.NormalPharmacist )
                        {
                            tempEmployee.Authority = Authority.SupportPharmacist;
                        }

                        tempEmployee.IsLocal = false;
                    }
                     
                    parameterList = new List<SqlParameter>();
                    parameterList.Add(new SqlParameter("Employee", SetCustomer(tempEmployee)));
                    MainWindow.ServerConnection.ExecuteProcBySchema(pharmacyInfo.PHAMAS_VerifyKey, "[Set].[InsertEmployee]", parameterList);
                }
                 
                UpdateIsLocal(e.IDNumber, true);
            }
        }

        public void Update(Employee e)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Employee", SetCustomer(e)));
            if (string.IsNullOrEmpty(ViewModelMainWindow.CurrentPharmacy.GroupServerName))
            {
                MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateEmployee]", parameterList); 
            }
                
            else
            {
                 MainWindow.ServerConnection.ExecuteProcBySchema(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName, "[Set].[UpdateEmployee]", parameterList);
                 parameterList = new List<SqlParameter>();
                 parameterList.Add(new SqlParameter("Employee", SetCustomer(e)));
                 MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateEmployee]", parameterList);

                foreach (var groupPharmactEmployee in e.GroupPharmacyEmployeeList.Where(_ => _.IsDirty))
                {
                    e.Authority = groupPharmactEmployee.EmployeeAuthority;
                    parameterList = new List<SqlParameter>();
                    parameterList.Add(new SqlParameter("Employee", SetCustomer(e)));
                    MainWindow.ServerConnection.ExecuteProcBySchema(groupPharmactEmployee.PharmacyVerifyKey, "[Set].[UpdateEmployee]", parameterList);
                }
                
                SyncData();
            }
        }

        public void Delete(int empId)
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

        public static Employee EmployeeLogin(string inputAccount, string password)
        {
            Employee result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
               result = conn.Query<Employee>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[Login]",
                    param: new { account = inputAccount, pw = password },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            });
            return result;
        }

        public static bool CheckEmployeeIsEnable(int empID)
        {
            bool result = false; 
             
            if(string.IsNullOrEmpty(ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName) == false)
            {
                SQLServerConnection.DapperQuery((conn) =>
                {
                    result = conn.QueryFirst<bool>($"{ChromeTabViewModel.ViewModelMainWindow.CurrentPharmacy.GroupServerName}.[Get].[CheckEmployeeEnable]",
                         param: new { EmpID = empID },
                         commandType: CommandType.StoredProcedure);
                     
                });

                //若sever的權限是false 則直接回傳false
                if (result == false)
                    return false;
            }
             
            //query各自藥局
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.QueryFirst<bool>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[CheckEmployeeEnable]",
                     param: new { EmpID = empID },
                     commandType: CommandType.StoredProcedure);

            });
             
            return result; 
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
                 ? MainWindow.ServerConnection.ExecuteProc("[Get].[TabAuth]", parameterList)
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
            DataBaseFunction.AddColumnValue(newRow, "Emp_AuthorityLevel", (int)e.Authority);
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
            DataBaseFunction.AddColumnValue(newRow, "Emp_WorkPositionID", 0);
            DataBaseFunction.AddColumnValue(newRow, "Emp_StartDate", e.StartDate);
            DataBaseFunction.AddColumnValue(newRow, "Emp_LeaveDate", e.LeaveDate);
            DataBaseFunction.AddColumnValue(newRow, "Emp_PurchaseLimit", e.PurchaseLimit);
            DataBaseFunction.AddColumnValue(newRow, "Emp_Note", e.Note);
            DataBaseFunction.AddColumnValue(newRow, "Emp_IsEnable", e.IsEnable);
            DataBaseFunction.AddColumnValue(newRow, "Emp_IsLocal", e.IsLocal);
            
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
            employeeTable.Columns.Add("Emp_IsLocal", typeof(bool));
             
            return employeeTable;
        }



        internal static DataTable AddClockIn(string empID,int type)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Emp_ID", empID));
            parameters.Add(new SqlParameter("type", type));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertClockInLog]", parameters);
        }

        public static IEnumerable<Employee> EmployeeClockInList(string WYear, string WMonth, int? EmpId)
        {
            
            List<Employee> result = null;
            if (EmpId is null)
            { 
                SQLServerConnection.DapperQuery((conn) =>
                {
                    result = conn.Query<Employee>(
                        $"{Properties.Settings.Default.SystemSerialNumber}.[Get].[ClockInLogEmp]",
                        param: new { WYear, WMonth  },
                        commandType: CommandType.StoredProcedure).ToList();
                });

            }
            else
            {
                SQLServerConnection.DapperQuery((conn) =>
                {
                    result = conn.Query<Employee>(
                        $"{Properties.Settings.Default.SystemSerialNumber}.[Get].[ClockInEmployee]",
                        param: new { WYear, WMonth, EmpId },
                        commandType: CommandType.StoredProcedure).ToList();
                }); 
            }

            return result;
        }
        public static IEnumerable<Employee> EmployeeClockInListTest(string WYear, string WMonth,string StoreNo, string EmpId, int Permit)
        {
            List<Employee> result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<Employee>(
                    $"{Properties.Settings.Default.SystemSerialNumber}.[Get].[ClockInLogEmployees]",
                    param: new { WYear, WMonth, StoreNo, Permit, Emp_ID = EmpId },
                    commandType: CommandType.StoredProcedure).ToList();
            });

            return result;
             
        }


    }
}