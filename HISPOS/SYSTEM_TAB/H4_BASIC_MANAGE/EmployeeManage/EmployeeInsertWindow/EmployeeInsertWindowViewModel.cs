using DomainModel;
using DomainModel.Enum;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.WorkPosition;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage.EmployeeInsertWindow
{
    public class EmployeeInsertWindowViewModel : ViewModelBase
    {
         
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

        public RelayCommand SubbmitCommand { get; set; }
        public RelayCommand CheckIdNumberCommand { get; set; }

        public EmployeeInsertWindowViewModel()
        {
           
            string acc = EmployeeService.GetEmployeeNewAccount();
            Employee.Account = acc;
            Employee.Password = acc;
            SubbmitCommand = new RelayCommand(SubbmitAction);
            CheckIdNumberCommand = new RelayCommand(CheckIdNumberAction);
        }

        private void CheckIdNumberAction()
        {
            var errorMsg = EmployeeService.CheckIdNumber(Employee);

            if (errorMsg == ErrorMessage.OK)
                MessageWindow.ShowMessage("檢查通過!", Class.MessageType.SUCCESS);
            else 
                MessageWindow.ShowMessage(errorMsg.GetDescriptionText(), Class.MessageType.ERROR);
           
        }

        private void SubbmitAction()
        {
            var errorMsg = EmployeeService.CheckIdNumber(Employee);
            if (errorMsg != ErrorMessage.OK)
            {
                return;
            }
            if (!Employee.CheckEmployeeAccountSame())
            {
                MessageWindow.ShowMessage("此帳號已經存在!", Class.MessageType.ERROR);
                return;
            }

            EmployeeService.Insert(Employee);
             
            MessageWindow.ShowMessage("新增成功!", Class.MessageType.SUCCESS);
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseEmployeeInsertWindow"));
        }
    }
}