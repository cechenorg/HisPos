using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.WorkPosition;
using His_Pos.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using His_Pos.NewClass.Person.MedicalPerson;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage
{
   public class EmployeeManageViewModel : TabBase
   {
        public override TabBase getTab() { 
        return this;
        }
        #region -----Define Command----- 
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand DataChangeCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand NewEmployeeCommand { get; set; }
        public RelayCommand ChangePassWordCommand { get; set; }
        #endregion
        #region ----- Define Variables -----
         
        public Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set
            {
                Set(() => Employee, ref employee, value);
            }
        }
        public Employees employeeCollection;
        public Employees EmployeeCollection
        {
            get { return employeeCollection; }
            set
            {
                Set(() => EmployeeCollection, ref employeeCollection, value);
            }
        }
        public WorkPositions workPositions = new WorkPositions();
        public WorkPositions WorkPositions
        {
            get { return workPositions; }
            set
            {
                Set(() => WorkPositions, ref workPositions, value);
            }
        }
        
        #endregion

        public EmployeeManageViewModel() {

            Init();
           //DataChangeCommand = new RelayCommand(DataChangeAction);
           //CancelCommand = new RelayCommand(CancelAction);
           //SubmitCommand = new RelayCommand(SubmitAction);
           //SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
           //DeleteCommand = new RelayCommand(DeleteAction);
            NewEmployeeCommand = new RelayCommand(NewEmployeeAction);
            ChangePassWordCommand = new RelayCommand(ChangePassWordAction);
        }
        #region Action
        public void ChangePassWordAction() {
           
        }
        public void NewEmployeeAction() {
            
            Employee newEmployee = new Employee();
            newEmployee.Name = "新人";
            newEmployee.Gender = "男";
            newEmployee.WorkPositionID = 2;
            newEmployee.WorkPositionName = "藥師";
            newEmployee.StartDate = DateTime.Today;
            newEmployee.Birthday = DateTime.Today;
            newEmployee.IDNumber = DateTime.Now.ToString("yyyyMMddhhss");
            MainWindow.ServerConnection.OpenConnection();
            //newEmployee = newEmployee.Save();
            MainWindow.ServerConnection.CloseConnection();
            EmployeeCollection.Add(newEmployee);
            Employee = NewFunction.DeepCloneViaJson(EmployeeCollection[EmployeeCollection.Count - 1]);
        }
         
        #endregion
        #region Function 
        private void Init() {
            MainWindow.ServerConnection.OpenConnection();
            WorkPositions = new WorkPositions();
            EmployeeCollection = new Employees();
            EmployeeCollection.Init();
            MainWindow.ServerConnection.CloseConnection();
             
        } 
        #endregion

    }
}
