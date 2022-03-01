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
        public WorkPositions workPositions = new WorkPositions();

        public WorkPositions WorkPositions
        {
            get { return workPositions; }
            set
            {
                Set(() => WorkPositions, ref workPositions, value);
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

        public RelayCommand SubbmitCommand { get; set; }
        public RelayCommand CheckIdNumberCommand { get; set; }

        public EmployeeInsertWindowViewModel()
        {
            Employee = new Employee();
            string acc = Employee.GetEmployeeNewAccount();
            Employee.Account = acc;
            Employee.Password = acc;
            SubbmitCommand = new RelayCommand(SubbmitAction);
            CheckIdNumberCommand = new RelayCommand(CheckIdNumberAction);
        }

        private void CheckIdNumberAction()
        {
            if (Employee.CheckIdNumber())
                MessageWindow.ShowMessage("檢查通過!", NewClass.MessageType.SUCCESS);
        }

        private void SubbmitAction()
        {
            if (!Employee.CheckIdNumber())
            {
                return;
            }
            if (!Employee.CheckEmployeeAccountSame())
            {
                MessageWindow.ShowMessage("此帳號已經存在!", NewClass.MessageType.ERROR);
                return;
            }
            Employee.Insert();

            MessageWindow.ShowMessage("新增成功!", NewClass.MessageType.SUCCESS);
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseEmployeeInsertWindow"));
        }
    }
}