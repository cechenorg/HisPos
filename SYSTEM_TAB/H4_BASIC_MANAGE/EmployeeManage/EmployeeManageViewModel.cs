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
using His_Pos.FunctionWindow;
using System.Windows.Data;
using System.ComponentModel;

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
        private CollectionViewSource employeeCollectionViewSource;
        private CollectionViewSource EmployeeCollectionViewSource
        {
            get => employeeCollectionViewSource;
            set
            {
                Set(() => EmployeeCollectionViewSource, ref employeeCollectionViewSource, value); 
            }
        }

        private ICollectionView employeeCollectionView;
        public ICollectionView EmployeeCollectionView
        {
            get => employeeCollectionView;
            private set
            {
                Set(() => EmployeeCollectionView, ref employeeCollectionView, value);
            }
        }
        public Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set
            {
                Set(() => Employee, ref employee, value);
                IsEnable = Employee.ID == ViewModelMainWindow.CurrentUser.ID; 
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
        private bool localCheck = true;
        public bool LocalCheck
        {
            get { return localCheck; }
            set
            {
                Set(() => LocalCheck, ref localCheck, value);
                EmployeeCollectionViewSource.Filter += Filter;
            }
        }
        private bool globalCheck = false;
        public bool GlobalCheck
        {
            get { return globalCheck; }
            set
            {
                Set(() => GlobalCheck, ref globalCheck, value);
                EmployeeCollectionViewSource.Filter += Filter;
            }
        }
        private bool isEnable = true;
        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                Set(() => IsEnable, ref isEnable, value); 
            }
        }
        #endregion

        public EmployeeManageViewModel() {

            Init();
            CancelCommand = new RelayCommand(CancelAction);
            SubmitCommand = new RelayCommand(SubmitAction); 
            DeleteCommand = new RelayCommand(DeleteAction);
            NewEmployeeCommand = new RelayCommand(NewEmployeeAction);
            ChangePassWordCommand = new RelayCommand(ChangePassWordAction);
        }
        #region Action
  
        private void CancelAction() {
            Employee = Employee.GetDataByID(Employee.ID);
        }
        private void SubmitAction() {
            Employee.Update();
            MessageWindow.ShowMessage("修改成功",Class.MessageType.SUCCESS);
        }
        private void DeleteAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否刪除員工? 刪除後無法恢復 請慎重確認","員工刪除");
            if ((bool)confirmWindow.DialogResult) {
                Employee.Delete();
                MessageWindow.ShowMessage("刪除成功!",Class.MessageType.SUCCESS);
                Init();
            }
               
        }
        public void ChangePassWordAction() {
            EmployeeChangePasswordWindow.EmployeeChangePasswordWindow employeeChangePasswordWindow = new EmployeeChangePasswordWindow.EmployeeChangePasswordWindow(Employee); 
        }
        public void NewEmployeeAction() {
            EmployeeInsertWindow.EmployeeInsertWindow employeeInsertWindow = new EmployeeInsertWindow.EmployeeInsertWindow();
            Init();
        }
         
        #endregion
        #region Function 
        private void Init() {
            MainWindow.ServerConnection.OpenConnection();
            WorkPositions = new WorkPositions();
            EmployeeCollection = new Employees();
            EmployeeCollection.Init();
            MainWindow.ServerConnection.CloseConnection();

            EmployeeCollectionViewSource = new CollectionViewSource { Source = EmployeeCollection };
            EmployeeCollectionView = EmployeeCollectionViewSource.View;
            EmployeeCollectionViewSource.Filter += Filter;
        }
        private void Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is Employee))
                e.Accepted = false;

            e.Accepted = false;

            if (GlobalCheck)
                e.Accepted = true;
            else if(LocalCheck && ((Employee)e.Item).IsLocal)
                e.Accepted = true; 
        }
        #endregion

    }
}
