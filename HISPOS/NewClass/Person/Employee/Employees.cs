using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Person.Employee
{
    public class Employees : ObservableCollection<Employee>
    {
        public Employees()
        {
        }

        public void Init()
        {
            Clear();

            var employees = EmployeeDb.GetData();

            foreach (var emp in employees)
            {
                Add(emp);
            } 
        }

        public void ClockIn(string WYear, string WMonth,int? EmpID)
        {
            Clear();
            var table = EmployeeDb.EmployeeClockInList(WYear, WMonth, EmpID);
            foreach (DataRow row in table.Rows)
            {
                Add(new Employee(row));
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
           
            foreach (var emp in EmployeeDb.GetData())
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
            var employees = EmployeeDb.GetData();
             
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