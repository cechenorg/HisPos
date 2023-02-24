using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using DomainModel.Enum;

namespace His_Pos.NewClass.Person.Employee
{
    public class Employees : ObservableCollection<Employee>
    {

        private EmployeeService _employeeService = new EmployeeService(new EmployeeDb());
        public Employees()
        {

        }

        public void Init()
        {
            Clear();
            var employees = _employeeService.GetData();
          
            foreach (var emp in employees)
            {
                if(emp.Authority != Authority.Admin)
                    Add(emp);
            } 
        }

        public void ClockIn(string WYear, string WMonth,int? EmpID)
        {
            Clear();
            var employees = EmployeeDb.EmployeeClockInList(WYear, WMonth, EmpID);
            foreach (var emp in employees)
            {
                Add(emp);
            }
        }

        public void ClockInEmp(string WYear, string WMonth, string StoreNo, string EmpId, int Permit)
        {
            Clear();
            var employees = EmployeeDb.EmployeeClockInListTest(WYear, WMonth, StoreNo, EmpId, Permit);

            foreach (var emp in employees)
            {
                Add(emp);
            } 
        }


        public void GetEnablePharmacist(DateTime selectedDate)
        { 
           
            foreach (var emp in _employeeService.GetData())
            {
                if (emp.CheckLeave(selectedDate) && emp.IsLocalPharmist() && emp.IsLocal)
                    Add(emp);
                else
                {
                    if (emp.ID.Equals(ViewModelMainWindow.CurrentUser.ID) && emp.IsLocalPharmist() )
                        Add(emp);
                }
            } 
        }

        public void InitPharmacists()
        {
            Clear();
            var employees = _employeeService.GetData();
             
            foreach (var emp in employees.Where(_ => _.IsLocalPharmist()))
            {
                Add(emp);
            }
        }

     

        public Employees GetLocalPharmacist()
        {
            var localPharmacists = new Employees();
            foreach (var e in Items)
            {
                if (e.IsLocal || e.IDNumber.Equals(ViewModelMainWindow.CurrentUser.IDNumber))
                    localPharmacists.Add(e);
            }
            return localPharmacists;
        }
    }
}