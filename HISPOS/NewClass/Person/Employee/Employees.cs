using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.ObjectModel;
using System.Data;

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
            var table = EmployeeDb.GetData();
            
            foreach (DataRow row in table.Rows)
            {
                Add(new Employee(row));
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
            var table = EmployeeDb.EmployeeClockInListTest(WYear, WMonth, StoreNo, EmpId, Permit);
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    Add(new Employee(row));
                }
            }
            else
            { 
            
            }
        }


        public void GetEnablePharmacist(DateTime selectedDate)
        {
            var table = EmployeeDb.GetData();
            var tempEmpList = new Employees();
            foreach (DataRow r in table.Rows)
            {
                tempEmpList.Add(new Employee(r));
            }
            foreach (var emp in tempEmpList)
            {
                if (emp.CheckLeave(selectedDate) && emp.IsLocalPharmist() && emp.IsLocal)
                    Add(emp);
                else
                {
                    if (emp.ID.Equals(ViewModelMainWindow.CurrentUser.ID) && emp.IsLocalPharmist() )
                        Add(emp);
                }
            }
            //var table = EmployeeDb.GetEnableMedicalPersonnels(selectedDate);
            //foreach (DataRow r in table.Rows)
            //{
            //    Add(new Employee(r));
            //}
        }

        public void InitPharmacists()
        {
            Clear();
            var table = EmployeeDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                var emp = new Employee(row);
                if (emp.IsLocalPharmist())
                    Add(emp);
            }
        }

        public void InitAllPharmacists()
        {
            Clear();
            var table = EmployeeDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                var emp = new Employee(row);
                if (emp.IsPharmist())
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