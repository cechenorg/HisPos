using DomainModel.Enum;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee; 
using His_Pos.NewClass.Pharmacy;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using His_Pos.Extention;

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
                RaisePropertyChanged(nameof(DisplayEmployeeCellPhone));
                RaisePropertyChanged(nameof(DisplayEmployeeTel));
                if (ViewModelMainWindow.CurrentPharmacy.GroupPharmacyinfoList != null && SelectedEmployee != null)
                {
                    SelectedEmployee.GroupPharmacyEmployeeList.Clear();
                    var source = EmployeeService.GetGroupPharmacy(SelectedEmployee,
                        ViewModelMainWindow.CurrentPharmacy.GroupPharmacyinfoList.ToList());
                    foreach (var item in source)
                    {
                        SelectedEmployee.GroupPharmacyEmployeeList.Add(item);
                    } 
                }
                    
            }
        }

        public string DisplayEmployeeTel
        {
            get
            {
                if (_selectedEmployee is null)
                    return string.Empty;


                var tel = _selectedEmployee.Tel;
                return tel is null ? string.Empty : tel.ToPatientTel();
            }
            set
            {
                string tel = value.Replace("-", "");
                _selectedEmployee.Tel = tel;
            }
        }

        public string DisplayEmployeeCellPhone
        {
            get
            {
                if (_selectedEmployee is null)
                    return string.Empty;
                var cellphone = _selectedEmployee.CellPhone;
                return cellphone is null ? string.Empty : cellphone.ToPatientCellPhone();
            }
            set
            {
                string cellphone = value.Replace("-", "");
                _selectedEmployee.CellPhone = cellphone;
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

        private bool _isVisibleGlobalEmployee = string.IsNullOrEmpty(ViewModelMainWindow.CurrentPharmacy.GroupServerName) == false &&
                                 (ViewModelMainWindow.CurrentUser.Authority == Authority.Admin ||
                                  ViewModelMainWindow.CurrentUser.Authority == Authority.PharmacyManager ||
                                  ViewModelMainWindow.CurrentUser.Authority == Authority.AccountingStaff ||
                                  ViewModelMainWindow.CurrentUser.Authority == Authority.StoreManager ||
                                  ViewModelMainWindow.CurrentUser.Authority == Authority.MasterPharmacist);

        public bool IsVisibleGlobalEmployee
        {
            get { return _isVisibleGlobalEmployee; }
            set
            {
                Set(() => IsVisibleGlobalEmployee, ref _isVisibleGlobalEmployee, value); 
            }
        }

        private bool _isEnableEditAuthority =
            string.IsNullOrEmpty(ViewModelMainWindow.CurrentPharmacy.GroupServerName) == false &&
            (ViewModelMainWindow.CurrentUser.Authority == Authority.Admin || 
             ViewModelMainWindow.CurrentUser.Authority == Authority.PharmacyManager ||
             ViewModelMainWindow.CurrentUser.Authority == Authority.AccountingStaff);

        public bool IsEnableEditAuthority
        {
            get { return _isEnableEditAuthority; }
            set
            {
                Set(() => IsEnableEditAuthority, ref _isEnableEditAuthority, value);

                FilterEmployee();
            }
        }


        #endregion ----- Define Variables -----

        private readonly IEmployeeService _employeeService;

        public EmployeeManageViewModel(IEmployeeService employeeService)
        {
            Init();
            CancelCommand = new RelayCommand(CancelAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            NewEmployeeCommand = new RelayCommand(NewEmployeeAction);
            ChangePassWordCommand = new RelayCommand(ChangePassWordAction);
            UpdateGroupPharmacyAuthorityCommand = new RelayCommand(UpdateGroupPharmacyAuthorityAction);
            _employeeService = employeeService;
        }

        #region Action

        private void UpdateGroupPharmacyAuthorityAction()
        {
            _employeeService.Update(SelectedEmployee);
            MessageWindow.ShowMessage("權限修改成功!",Class.MessageType.SUCCESS);
            var tempID = SelectedEmployee.ID;
            ReloadData();
            SelectedEmployee = EmployeeCollection.SingleOrDefault(_ => _.ID == tempID);
        }

        private void CancelAction()
        {
            SelectedEmployee = _employeeService.GetDataByID(SelectedEmployee.ID);
        }

        private void SubmitAction()
        {
            _employeeService.Update(SelectedEmployee); 
            MessageWindow.ShowMessage("修改成功", Class.MessageType.SUCCESS);
            var tempID = SelectedEmployee.ID;
            ReloadData();
            SelectedEmployee = EmployeeCollection.SingleOrDefault(_ => _.ID == tempID);
        }

        private void DeleteAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否刪除員工? 刪除後無法恢復 請慎重確認", "員工刪除");
            if ((bool)confirmWindow.DialogResult)
            {
                _employeeService.Delete(SelectedEmployee); 
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
            ReloadData();
            LocalCheck = true;
            FilterEmployee();
            ViewModelMainWindow.CurrentPharmacy.GroupPharmacyinfoList = PharmacyDBService.GetPharmacyListByGroupServerName();
            SelectedEmployee = FilterEmployeeCollection.FirstOrDefault();
             
        }

        private void ReloadData()
        {
            EmployeeCollection = new Employees();
            EmployeeCollection.Init();

            FilterEmployeeCollection = new Employees();

            foreach (var employeedata in EmployeeCollection)
            {
                FilterEmployeeCollection.Add(employeedata);
            }
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


            if (_isVisibleGlobalEmployee)
            {
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
            }
           
            SelectedEmployee = FilterEmployeeCollection.FirstOrDefault();
        }

         
        #endregion Function
    }
}