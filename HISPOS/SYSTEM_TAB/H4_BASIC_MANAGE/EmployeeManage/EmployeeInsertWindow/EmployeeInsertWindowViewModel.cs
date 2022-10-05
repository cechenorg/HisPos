using DomainModel;
using DomainModel.Enum;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee; 

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage.EmployeeInsertWindow
{
    public class EmployeeInsertWindowViewModel : ViewModelBase
    {
         
        public Employee employee = new Employee(){IsLocal = true};

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

        public RelayCommand SubbmitCommand { get; set; }
        public RelayCommand CheckIdNumberCommand { get; set; }

        private readonly IEmployeeService _employeeService;
        public EmployeeInsertWindowViewModel(IEmployeeService employeeService)
        {

            _employeeService = employeeService;
            string acc = EmployeeService.GetEmployeeNewAccount();
            Employee.Account = acc;
            Employee.Password = acc;
            SubbmitCommand = new RelayCommand(SubbmitAction);
            CheckIdNumberCommand = new RelayCommand(CheckIdNumberAction);
        }

        private void CheckIdNumberAction()
        {
            CheckIdNumber();
        }

        private void SubbmitAction()
        {
            if(CheckIdNumber() == false)
                return;

            _employeeService.Insert(Employee);
             
            MessageWindow.ShowMessage("新增成功!", Class.MessageType.SUCCESS);
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseEmployeeInsertWindow"));
        }

        private bool CheckIdNumber()
        {
            var idcheck_ErrorMsg = EmployeeService.VerifyIDNumber(Employee);
            var empExist_ErrorMsg = EmployeeService.CheckEmpIsExist(Employee);

            if (idcheck_ErrorMsg == ErrorMessage.OK && empExist_ErrorMsg == ErrorMessage.OK)
            {
                MessageWindow.ShowMessage("檢查通過!", Class.MessageType.SUCCESS);
                return true;
            } 
            else
            {
                if(idcheck_ErrorMsg != ErrorMessage.OK)
                    MessageWindow.ShowMessage(idcheck_ErrorMsg.GetDescriptionText(), Class.MessageType.ERROR);

                if (empExist_ErrorMsg != ErrorMessage.OK)
                    MessageWindow.ShowMessage(empExist_ErrorMsg.GetDescriptionText(), Class.MessageType.ERROR);
                return false;
            }
        }
    }
}