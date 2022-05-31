using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.WorkPosition;
using His_Pos.NewClass.Pharmacy;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage
{
    public class EmployeeManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region -----Define Command-----
         
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand NewEmployeeCommand { get; set; }
        public RelayCommand ChangePassWordCommand { get; set; }

        public RelayCommand UpdateGroupPharmacyAuthorityCommand { get; set; }

        #endregion -----Define Command-----

        #region ----- Define Variables -----



        private Employee _selectedEmployee;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                Set(() => SelectedEmployee, ref _selectedEmployee, value);

                if(ViewModelMainWindow.CurrentPharmacy.GroupPharmacyinfoList != null && SelectedEmployee != null)
                    SelectedEmployee.InitGroupPharmacyWorkPositionList(ViewModelMainWindow.CurrentPharmacy.GroupPharmacyinfoList.ToList(),WorkPositions);
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

        public Employees _filterEmployeeCollection;

        public Employees FilterEmployeeCollection
        {
            get { return _filterEmployeeCollection; }
            set
            {
                Set(() => FilterEmployeeCollection, ref _filterEmployeeCollection, value);
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

        private bool localCheck;

        public bool LocalCheck
        {
            get { return localCheck; }
            set
            {
                Set(() => LocalCheck, ref localCheck, value);

                FilterEmployee();
            }
        }

        private bool globalCheck = false;

        public bool GlobalCheck
        {
            get { return globalCheck; }
            set
            {
                Set(() => GlobalCheck, ref globalCheck, value);

                FilterEmployee();
            }
        }

        private bool _isQuit = true;

        public bool IsQuit
        {
            get { return _isQuit; }
            set
            {
                Set(() => IsQuit, ref _isQuit, value);

                FilterEmployee();
            }
        }

        private bool _isGroupPharmacy = string.IsNullOrEmpty(ViewModelMainWindow.CurrentPharmacy.GroupServerName) == false;

        public bool IsGroupPharmacy
        {
            get { return _isGroupPharmacy; }
            set
            {
                Set(() => IsGroupPharmacy, ref _isGroupPharmacy, value);

                FilterEmployee();
            }
        }


        #endregion ----- Define Variables -----

        public EmployeeManageViewModel()
        {
            Init();
            CancelCommand = new RelayCommand(CancelAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            NewEmployeeCommand = new RelayCommand(NewEmployeeAction);
            ChangePassWordCommand = new RelayCommand(ChangePassWordAction);
            UpdateGroupPharmacyAuthorityCommand = new RelayCommand(UpdateGroupPharmacyAuthorityAction);
        }

        #region Action

        private void UpdateGroupPharmacyAuthorityAction()
        {
            SelectedEmployee.Update();
            MessageWindow.ShowMessage("權限修改成功!",Class.MessageType.SUCCESS);
        }

        private void CancelAction()
        {
            SelectedEmployee = SelectedEmployee.GetDataByID(SelectedEmployee.ID);
        }

        private void SubmitAction()
        {
            SelectedEmployee.Update();
            MessageWindow.ShowMessage("修改成功", Class.MessageType.SUCCESS);
        }

        private void DeleteAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否刪除員工? 刪除後無法恢復 請慎重確認", "員工刪除");
            if ((bool)confirmWindow.DialogResult)
            {
                SelectedEmployee.Delete();
                MessageWindow.ShowMessage("刪除成功!", Class.MessageType.SUCCESS);
                Init();
            }
        }

        public void ChangePassWordAction()
        {
            EmployeeChangePasswordWindow.EmployeeChangePasswordWindow employeeChangePasswordWindow = new EmployeeChangePasswordWindow.EmployeeChangePasswordWindow(SelectedEmployee);
        }

        public void NewEmployeeAction()
        {
            EmployeeInsertWindow.EmployeeInsertWindow employeeInsertWindow = new EmployeeInsertWindow.EmployeeInsertWindow();
            Init();
        }

        #endregion Action

        #region Function

        private void Init()
        {
            MainWindow.ServerConnection.OpenConnection();
            WorkPositions = new WorkPositions();
            EmployeeCollection = new Employees();
            EmployeeCollection.Init();
            MainWindow.ServerConnection.CloseConnection();
            FilterEmployeeCollection = new Employees();

            foreach (var employeedata in EmployeeCollection)
            {
                FilterEmployeeCollection.Add(employeedata);
            } 
             
            LocalCheck = true;
            FilterEmployee();
            ViewModelMainWindow.CurrentPharmacy.GroupPharmacyinfoList = PharmacyDBService.GetPharmacyListByGroupServerName();
            SelectedEmployee = FilterEmployeeCollection.FirstOrDefault();
             
        }

        private void FilterEmployee()
        {
            FilterEmployeeCollection.Clear();
            foreach (var quitEmployee in EmployeeCollection)
            {
                FilterEmployeeCollection.Add(quitEmployee);
            }

            if (IsQuit == true)
            {
                foreach (var quitEmployee in EmployeeCollection.Where(_ => _.LeaveDate != null))
                {
                    FilterEmployeeCollection.Remove(quitEmployee);
                }
            }

            if (LocalCheck)
            {
                foreach (var quitEmployee in EmployeeCollection.Where(_ => _.IsLocal == false))
                {
                    FilterEmployeeCollection.Remove(quitEmployee);
                }
            }

            if (GlobalCheck)
            {
                foreach (var quitEmployee in EmployeeCollection.Where(_ => _.IsLocal))
                {
                    FilterEmployeeCollection.Remove(quitEmployee);
                }
            }
            SelectedEmployee = FilterEmployeeCollection.FirstOrDefault();
        }

         
        #endregion Function
    }
}