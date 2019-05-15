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
        public bool btnCancelEnable;
        public bool BtnCancelEnable
        {
            get { return btnCancelEnable; }
            set { Set(() => BtnCancelEnable, ref btnCancelEnable, value); }
        }
        public bool btnSubmitEnable;
        public bool BtnSubmitEnable
        {
            get { return btnSubmitEnable; }
            set { Set(() => BtnSubmitEnable, ref btnSubmitEnable, value); }
        }
        public string changeText;
        public string ChangeText
        {
            get { return changeText; }
            set { Set(() => ChangeText, ref changeText, value); }
        }
        public string changeForeground;
        public string ChangeForeground
        {
            get { return changeForeground; }
            set { Set(() => ChangeForeground, ref changeForeground, value); }
        }
        public Collection<string> positionCollection;
        public Collection<string> PositionCollection
        {
            get { return positionCollection; }
            set
            {
                Set(() => PositionCollection, ref positionCollection, value);
            }
        }
         
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
        public WorkPositions workPositions;
        public WorkPositions WorkPositions
        {
            get { return workPositions; }
            set
            {
                Set(() => WorkPositions, ref workPositions, value);
            }
        }
        public Genders genders = new Genders();
        public Genders Genders
        {
            get { return genders; }
            set
            {
                Set(() => Genders, ref genders, value);
            }
        }
        private bool isIdNumEnable = false;
        public bool IsIdNumEnable {
            get { return isIdNumEnable; }
            set { Set(()=> IsIdNumEnable,ref isIdNumEnable,value); }
        }
        #endregion

        public EmployeeManageViewModel() {

            Init();
            DataChangeCommand = new RelayCommand(DataChangeAction);
            CancelCommand = new RelayCommand(CancelAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            NewEmployeeCommand = new RelayCommand(NewEmployeeAction);
            ChangePassWordCommand = new RelayCommand(ChangePassWordAction);
        }
        #region Action
        public void ChangePassWordAction() {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(Employee);
        }
        public void NewEmployeeAction() {
            IsIdNumEnable = true;
            Employee newEmployee = new Employee();
            newEmployee.Name = "新人";
            newEmployee.Gender = "男";
            newEmployee.WorkPositionID = 2;
            newEmployee.WorkPositionName = "藥師";
            newEmployee.StartDate = DateTime.Today;
            newEmployee.Birthday = DateTime.Today;
            newEmployee.IDNumber = DateTime.Now.ToString("yyyyMMddhhss");
            MainWindow.ServerConnection.OpenConnection();
            newEmployee = newEmployee.Save();
            MainWindow.ServerConnection.CloseConnection();
            EmployeeCollection.Add(newEmployee);
            Employee = NewFunction.DeepCloneViaJson(EmployeeCollection[EmployeeCollection.Count - 1]);
        }
        public void DeleteAction()
        {
            MainWindow.ServerConnection.OpenConnection(); 
            Employee.Delete();
            EmployeeCollection.Remove( EmployeeCollection.Single(emp => emp.ID == Employee.ID) );
            Employee = NewFunction.DeepCloneViaJson( EmployeeCollection[EmployeeCollection.Count - 1]);
            MainWindow.ServerConnection.CloseConnection(); 
        }
        public void SelectionChangedAction()
        {
            if (Employee is null) return;
            IsIdNumEnable = false;
            Employee = NewFunction.DeepCloneViaJson(EmployeeCollection.Single(emp => emp.ID == Employee.ID));
            InitDataChanged();
        }
        public void DataChangeAction()
        {
            DataChanged();
        }
        public void SubmitAction() {
            IsIdNumEnable = false;
            for (int i = 0; i < EmployeeCollection.Count; i++)
            {
                if (EmployeeCollection[i].ID == Employee.ID)
                {
                    EmployeeCollection[i] = Employee;
                    Employee = NewFunction.DeepCloneViaJson(EmployeeCollection.Single(cus => cus.ID == EmployeeCollection[i].ID));
                    break;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            Employee.WorkPositionID = WorkPositions.Single(w => w.WorkPositionName == Employee.WorkPositionName).WorkPositionId;
            Employee = Employee.Save();
            ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels.Clear();
            ViewModelMainWindow.CurrentPharmacy.MedicalPersonnels = new MedicalPersonnels(MedicalPersonnelInitType.All);
            MainWindow.ServerConnection.CloseConnection();
            InitDataChanged(); 
        }
        public void CancelAction()
        {
            Employee = NewFunction.DeepCloneViaJson(EmployeeCollection.Single(emp => emp.ID == Employee.ID));
            InitDataChanged();
        }
        #endregion
        #region Function

        private void Init() {
            MainWindow.ServerConnection.OpenConnection();
            WorkPositions = new WorkPositions();
            EmployeeCollection = new Employees();
            EmployeeCollection.Init();
            MainWindow.ServerConnection.CloseConnection();

            if (EmployeeCollection.Count > 0)
                Employee = NewFunction.DeepCloneViaJson(EmployeeCollection[0]);
            InitDataChanged();
        }
        private void DataChanged()
        { 
            ChangeText = "已修改";
            ChangeForeground = "Red";
            BtnCancelEnable = true;
            BtnSubmitEnable = true;
        } 
        private void InitDataChanged()
        {
            ChangeText = "未修改";
            ChangeForeground = "Black";
            BtnCancelEnable = false;
            BtnSubmitEnable = false;
        }
         
        #endregion

    }
}
