using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Enum;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Pharmacy;
using His_Pos.Service;

namespace His_Pos.NewClass.Person.Employee
{
    public class EmployeeService
    {

        public static Employee GetDataByID(int id)
        {
            return EmployeeDb.GetData().SingleOrDefault(_ => _.ID == id);
        }

        public static void Insert(Employee emp)
        {
            EmployeeDb.Insert(emp);
        }

        public static void Update(Employee emp)
        {

            EmployeeDb.Update(emp);
        }

        public static void Delete(Employee emp)
        {
            EmployeeDb.Delete(emp.ID);
        }

        public static string GetEmployeeNewAccount()
        {
            DataTable table = EmployeeDb.GetEmployeeNewAccount();
            return table.Rows[0].Field<string>("Account");
        }


        public static List<string> GetTabAuth(Employee employee)
        {
            DataTable table = EmployeeDb.GetTabAuth((int)employee.Authority);
            List<string> tabAuths = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                tabAuths.Add(row["Aut_TabName"].ToString());
            }
            return tabAuths;
        }

        public static ErrorMessage CheckIdNumber(Employee emp)
        {
            if (string.IsNullOrEmpty(emp.IDNumber))
                return ErrorMessage.DataEmpty;

            if (!VerifyService.VerifyIDNumber(emp.IDNumber))
            {

                return ErrorMessage.EmployeeIDNumberFormatError;
            }
            var table = EmployeeDb.CheckIdNumber(emp.IDNumber);
            if (table.Rows[0].Field<int>("Count") > 0)
            {
                return ErrorMessage.EmployeeIDNumberExist;
            }

            table = EmployeeDb.CheckEmployeeAccountSame(emp.Account);
            if (table.Rows[0].Field<int>("Count") > 0)
            {
                return ErrorMessage.EmployeeAccountExist;
            }


            return ErrorMessage.OK;
        }

        public static Employee Login(string Account, string Password)
        {
            return EmployeeDb.EmployeeLogin(Account, Password);
        }

        public static IEnumerable<GroupAuthority> GetGroupPharmacy(Employee emp, List<PharmacyInfo> groupServerList)
        {
            List<GroupAuthority> result = new List<GroupAuthority>();

            var employeeList = EmployeeDb.GetGroupPharmacyDataByID(groupServerList.Select(_ => _.PHAMAS_VerifyKey).ToList(), emp.ID);
             
            for (int i = 0; i < groupServerList.Count; i++)
            {
                GroupAuthority tempData = new GroupAuthority()
                {
                    PharmacyName = groupServerList[i].PHAMAS_NAME,
                    PharmacyVerifyKey = groupServerList[i].PHAMAS_VerifyKey,
                    EmployeeAuthority = employeeList[i].Authority
                };
                tempData.IsDirty = false;
                result.Add(tempData);
            }
             
            return result;
        }
    }
}
