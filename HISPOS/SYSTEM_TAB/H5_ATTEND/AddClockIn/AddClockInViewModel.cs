using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.ClockIn;
using System;
using System.Data;
using System.Windows.Threading;
using DomainModel;
using DomainModel.Enum;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.AddClockIn
{
    public class AddClockInViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;

        }
        public AddClockInViewModel()
        {
            GetDate();
            RegisterCommands();
        }

        #region ----- Define Commands -----

        public RelayCommand<object> ConfirmAddClockInCommand { get; set; }


        #endregion ----- Define Commands -----

        #region ----- Define Variables -----
        public string account = "";

        public string Account
        {
            get { return account; }
            set { Set(() => Account, ref account, value); }
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

        public His_Pos.NewClass.Person.Employee.ClockIn.ClockIn clockIn;

        public ClockInLog clockInLogs;
        public His_Pos.NewClass.Person.Employee.ClockIn.ClockIn ClockIn
        {
            get { return clockIn; }
            set
            {
                Set(() => ClockIn, ref clockIn, value);
            }
        }
        public ClockInLog ClockInLogs
        {
            get { return clockInLogs; }
            set
            {
                Set(() => ClockInLogs, ref clockInLogs, value);
            }
        }

        private bool inCheck = true;
        public bool InCheck
        {
            get { return inCheck; }
            set
            {
                Set(() => InCheck, ref inCheck, value);
            }
        }

        private bool outCheck = false;
        public bool OutCheck
        {
            get { return outCheck; }
            set
            {
                Set(() => OutCheck, ref outCheck, value);
            }
        }

        #endregion ----- Define Variables -----

        #region ----- Define Actions -----

        private void ConfirmAddClockInAction(object sender)     
        {

            Employee = new Employee();
            Employee.Account = Account;
            Employee.Password = (sender as System.Windows.Controls.PasswordBox)?.Password;

            if (string.IsNullOrEmpty(Employee.Password) && !string.IsNullOrEmpty(Employee.Account))
            {
                return;
            }
            if (string.IsNullOrEmpty(Employee.Password) && string.IsNullOrEmpty(Employee.Account))
            {
                MessageWindow.ShowMessage("請輸入帳號密碼!", Class.MessageType.ERROR);
                return;
            }
            else
                if (!CheckPassWord()) return;  //檢查帳密

            int wtype=1;
            if (!inCheck)
                wtype = 2;


            ////增加一筆打卡紀錄
            MainWindow.ServerConnection.OpenConnection();
            EmployeeDb.AddClockIn(Employee.ID.ToString(), wtype);
            string year = Convert.ToString(DateTime.Now.Year);
            string month = Convert.ToString(DateTime.Now.Month);
            string day = Convert.ToString(DateTime.Now.Day);
            ClockInLogs = new ClockInLog(ClockInDb.EmployeeClockInLog(year,month,day, Employee.ID.ToString(), wtype));
            MainWindow.ServerConnection.CloseConnection();


            (sender as System.Windows.Controls.PasswordBox)?.Clear();
            this.Account = "";
            MessageWindow.ShowMessage("打卡成功!!", Class.MessageType.SUCCESS); 
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            ConfirmAddClockInCommand = new RelayCommand<object>(ConfirmAddClockInAction);
        }
        private bool CheckPassWord()
        {
            var errorMsg = EmployeeService.CheckIdNumber(Employee);

            //1.如果全部都沒有,查無帳號,請確認帳號
            if (errorMsg != ErrorMessage.OK)
            {
                MessageWindow.ShowMessage(errorMsg.GetDescriptionText(), Class.MessageType.ERROR);
                return false;
            }
            
            Employee = EmployeeService.Login(Employee.Account, Employee.Password);
           
            //檢查帳密 密碼錯誤
            if (Employee == null)
            {
                MessageWindow.ShowMessage("密碼錯誤!", Class.MessageType.ERROR);
                return false;
            }
            

            return true;
        }
        public void GetDate()
        {
            if (System.DateTime.Now.Hour > 12) {
                InCheck = false;
                OutCheck = true;
            }

            MainWindow.ServerConnection.OpenConnection();
            ClockInLogs = new ClockInLog(ClockInDb.EmployeeClockInLog(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString(), System.DateTime.Now.Day.ToString(), "",0));
            MainWindow.ServerConnection.CloseConnection();
        }
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(delegate (object f)
                {
                    ((DispatcherFrame)f).Continue = false;
                    return null;
                }
            ), frame);
            Dispatcher.PushFrame(frame);
        }

        #endregion ----- Define Functions -----

    }
}
