using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using static His_Pos.H5_ATTEND.ClockIn.ClockInView;

namespace His_Pos.Class.Employee
{
   public class EmployeeDb
    {
        public static ObservableCollection<Employee> GetEmployeeData()
        {
            ObservableCollection<Employee> collection = new ObservableCollection<Employee>();

            var dd = new DbConnection(Settings.Default.SQL_local);
            var table = dd.ExecuteProc("[HIS_POS_DB].[EmployeeManageView].[GetEmployeeData]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add(new Employee(row));
            }
            return collection;
        }
        public static ObservableCollection<EmpClockIn> GetEmpClockIn()
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
           var table = dd.ExecuteProc("[HIS_POS_DB].[ClockInView].[GetEmpClockIn]");
            ObservableCollection<EmpClockIn> empClockIns = new ObservableCollection<EmpClockIn>();
            foreach (DataRow row in table.Rows) {
                empClockIns.Add(new EmpClockIn(row));
            }
            return empClockIns;
        }
        internal static DataTable SaveEmployeeData(Employee employee)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID",employee.Id));
            parameters.Add(new SqlParameter("EMP_NAME",employee.Name));
            parameters.Add(new SqlParameter("EMP_NICKNAME", employee.NickName));
            parameters.Add(new SqlParameter("EMP_QNAME", employee.Qname));
            parameters.Add(new SqlParameter("EMP_GENDER", employee.Gender == "男" ? "True" : "False"));
            parameters.Add(new SqlParameter("EMP_IDNUM", employee.IdNum));
            parameters.Add(new SqlParameter("EMP_DEPARTMENT", employee.Department));
            parameters.Add(new SqlParameter("EMP_POSITION", employee.Position));
            parameters.Add(new SqlParameter("EMP_BIRTH", Convert.ToDateTime(employee.Birth)));
            parameters.Add(new SqlParameter("EMP_ADDR", employee.Address));
            parameters.Add(new SqlParameter("EMP_TEL", employee.Tel));
            parameters.Add(new SqlParameter("EMP_EMAIL", employee.Email));
            parameters.Add(new SqlParameter("EMP_STARTDATE", Convert.ToDateTime(employee.StartDate)));
            parameters.Add(new SqlParameter("EMP_URGENTPERSON", employee.UrgentContactPerson));
            parameters.Add(new SqlParameter("EMP_URGENTPHONE", employee.UrgentContactPhone));
            parameters.Add(new SqlParameter("EMP_PURCHASELIMIT", Convert.ToInt32(employee.PurchaseLimit)));
            parameters.Add(new SqlParameter("@EMP_DESCRIPTION", employee.Description));
            
           return dd.ExecuteProc("[HIS_POS_DB].[EmployeeManageView].[SaveEmployeeData]", parameters);
        }
        internal static void DeleteEmployeeData(Employee employee)
        {
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", employee.Id));
            dd.ExecuteProc("[HIS_POS_DB].[EmployeeManageView].[DeleteEmployeeData]", parameters);
        }
    }
}
