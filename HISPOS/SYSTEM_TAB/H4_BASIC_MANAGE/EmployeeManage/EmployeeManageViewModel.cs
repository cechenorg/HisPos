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
using His_Pos.Class;

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

        private Employees _filterEmployeeCollection;

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
            MessageWindow.ShowMessage("權限修改成功!", MessageType.SUCCESS);
            var tempID = SelectedEmployee.ID;
            ReloadData();
            SelectedEmployee = EmployeeCollection.SingleOrDefault(_ => _.ID == tempID);
        }

        private void CancelAction()
        {
            SelectedEmployee = _employeeService.GetDataByID(SelectedEmployee.ID);
        }
        private bool CheckAuth(Employee emp)
        {
            switch (ViewModelMainWindow.CurrentUser.Authority)
            {
                case Authority.Admin://所有人員的權限及員工資料皆可調整
                    return true;

                case Authority.PharmacyManager:
                    if (emp.ID != ViewModelMainWindow.CurrentUser.ID && emp.Authority == Authority.PharmacyManager)//不能修改自己以外的經理
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                case Authority.AccountingStaff:
                    if (emp.ID != ViewModelMainWindow.CurrentUser.ID && (emp.Authority == Authority.PharmacyManager || emp.Authority == Authority.AccountingStaff))//不能修改自己以外的經理&會計
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                case Authority.StoreManager:
                case Authority.MasterPharmacist:
                    List<Authority> canUpAuth = new List<Authority>() { Authority.StoreEmployee, Authority.NormalPharmacist, Authority.SupportPharmacist };
                    if (canUpAuth.Contains(emp.Authority) || emp.ID == ViewModelMainWindow.CurrentUser.ID)//店長&負責藥師只可以修改自己或578權限
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Authority.StoreEmployee:
                case Authority.NormalPharmacist:
                case Authority.SupportPharmacist:
                    return false;
                default:
                    return false;
            }
        }
        private void SubmitAction()
        {
            bool isCanEdit = IsCanEdit();
            if (!isCanEdit)
            {
                MessageWindow.ShowMessage("禁止修改權限", MessageType.WARNING);
                return;
            }

            _employeeService.Update(SelectedEmployee);
            MessageWindow.ShowMessage("修改成功", MessageType.SUCCESS);
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
                MessageWindow.ShowMessage("刪除成功!", MessageType.SUCCESS);
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

            foreach (Employee employeedata in EmployeeCollection)
            {
                bool isCanEdit = CheckAuth(employeedata);
                employeedata.IsCanEdit = isCanEdit;
                FilterEmployeeCollection.Add(employeedata);
            }
            FilterEmployee();
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
        private bool IsCanEdit()
        {
            switch (ViewModelMainWindow.CurrentUser.Authority)
            {
                case Authority.Admin:
                    return true;

                case Authority.PharmacyManager:
                    if (SelectedEmployee.Authority == Authority.PharmacyManager)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case Authority.AccountingStaff:
                    if (SelectedEmployee.Authority == Authority.PharmacyManager || SelectedEmployee.Authority == Authority.AccountingStaff)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case Authority.StoreManager:
                case Authority.MasterPharmacist:
                    List<Authority> canUpdate = new List<Authority>() { Authority.StoreEmployee, Authority.NormalPharmacist, Authority.SupportPharmacist };
                    if (canUpdate.Contains(SelectedEmployee.Authority))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        #endregion Function
    }
}