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

    public interface IEmployeeService
    {
        void Insert(Employee emp);
        Employee GetDataByID(int id);
        IEnumerable<Employee> GetData();
        void Update(Employee emp);
        void Delete(Employee emp);
        Employee Login(string Account, string Password);

    }
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeDB _employeeDb;

        public EmployeeService(IEmployeeDB employeeDb)
        {
            _employeeDb = employeeDb;
        }

        public Employee GetDataByID(int id)
        {
            return _employeeDb.GetData().SingleOrDefault(_ => _.ID == id);
        }

        public IEnumerable<Employee> GetData()
        {
            return _employeeDb.GetData();
        }

        public void Insert(Employee emp)
        {
            _employeeDb.Insert(emp);
        }

        public void Update(Employee emp)
        {

            _employeeDb.Update(emp);
        }
   
        public void Delete(Employee emp)
        {
            _employeeDb.Delete(emp.ID);
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

        public static ErrorMessage VerifyIDNumber(Employee emp)
        {
            if (string.IsNullOrEmpty(emp.IDNumber))
                return ErrorMessage.DataEmpty;

            if (!VerifyService.VerifyIDNumber(emp.IDNumber))
            {

                return ErrorMessage.EmployeeIDNumberFormatError;
            }
           
            return ErrorMessage.OK;
        }

        public static ErrorMessage CheckEmpIsExist(Employee emp)
        {
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

        public Employee Login(string Account, string Password)
        {
            return _employeeDb.EmployeeLogin(Account, Password);
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
