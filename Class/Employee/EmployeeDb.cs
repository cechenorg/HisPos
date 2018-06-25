using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Employee
{
   public class EmployeeDb
    {
        public static DataTable GetEmployeeData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            return dd.ExecuteProc("[HIS_POS_DB].[EmployeeManageView].[GetEmployeeData]");
        }
        internal static void SaveEmployeeData(Employee employee)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID",employee.Id));
            parameters.Add(new SqlParameter("EMP_NAME",employee.Name));
            parameters.Add(new SqlParameter("EMP_QNAME", employee.Qname));
            parameters.Add(new SqlParameter("EMP_GENDER", employee.Gender));
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
            
            dd.ExecuteProc("[HIS_POS_DB].[EmployeeManageView].[SaveEmployeeData]", parameters);
        }
    }
}
