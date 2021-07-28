using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.ClockIn;
using System.Data;
using System.Windows.Threading;


namespace His_Pos.SYSTEM_TAB.H5_ATTEND.ClockInSearch
{
    public class ClockInSearchViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;

        }
        public ClockInSearchViewModel()
        {
            RegisterCommands();
            GetDate();
        }

        public string account = "";
        public string Account
        {
            get { return account; }
            set { Set(() => Account, ref account, value); }
        }

        public string searchMonth = System.DateTime.Now.Month.ToString();
        public string SearchMonth
        {
            get { return searchMonth; }
            set { Set(() => SearchMonth, ref searchMonth, value);

                GetDate();
            }            
        }

        public int hourCount;
        public int HourCount
        {
            get { return hourCount; }
            set
            {
                Set(() => HourCount, ref hourCount, value);
            }
        }

        public int minCount;
        public int MinCount
        {
            get { return minCount; }
            set
            {
                Set(() => MinCount, ref minCount, value);
            }
        }

        public Employee singinemployee;
        public Employee SingInEmployee
        {
            get { return singinemployee; }
            set
            {
                Set(() => SingInEmployee, ref singinemployee, value);
            }
        }

        public Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set
            {
                Set(() => Employee, ref employee, value);


                GetDate();
            //if (Employee != null) {


            //    MainWindow.ServerConnection.OpenConnection();
            //    ClockInLogs = new ClockInLog(ClockInDb.ClockInLogByDate(System.DateTime.Now.Year.ToString(), SearchMonth , Employee.ID.ToString()));
            //    MainWindow.ServerConnection.CloseConnection();

            //    if (ClockInLogs is null) return;
            //    int iMin = 0;
            //    foreach (var s in ClockInLogs)
            //    {
            //        iMin += (int)s.WMin;
            //    }
            //    HourCount = iMin / 60;
            //    MinCount = iMin % 60;

            //}
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

        public ClockInLog clockInLogs;
        public ClockInLog ClockInLogs
        {
            get { return clockInLogs; }
            set
            {
                Set(() => ClockInLogs, ref clockInLogs, value);
            }
        }



        #region ----- Define Commands -----

        public RelayCommand<object> ConfirmEmpCommand { get; set; }
        public RelayCommand<object> SearchCommand { get; set; }
        public RelayCommand<object> DataChangeCommand { get; set; }
        

        #endregion ----- Define Commands -----

        #region ----- Define Actions -----

        private void ConfirmEmpAction(object sender)
        {

            SingInEmployee = new Employee();
            SingInEmployee.Account = Account;
            SingInEmployee.Password = (sender as System.Windows.Controls.PasswordBox)?.Password;

            if (string.IsNullOrEmpty(SingInEmployee.Password) && !string.IsNullOrEmpty(SingInEmployee.Account))
            {
                return;
            }
            if (string.IsNullOrEmpty(SingInEmployee.Password) && string.IsNullOrEmpty(SingInEmployee.Account))
            {
                MessageWindow.ShowMessage("請輸入帳號密碼!", Class.MessageType.ERROR);
                return;
            }
            else
                if (!CheckPassWord()) return;  //檢查帳密




            if (SingInEmployee.ID == 1 || SingInEmployee.WorkPosition.WorkPositionId == 4)
            {
                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockIn(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString(),null);
                MainWindow.ServerConnection.CloseConnection();

            }
            else
            {
                MainWindow.ServerConnection.OpenConnection();
                EmployeeCollection = new Employees();
                EmployeeCollection.ClockIn(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString(), SingInEmployee.ID);
                MainWindow.ServerConnection.CloseConnection();

            }

            (sender as System.Windows.Controls.PasswordBox)?.Clear();
            this.Account = "";


        }
        private void SearchAction(object sender)
        {



        }
        private void DataChangeAction(object sender)
        {
            
            MessageWindow.ShowMessage(SearchMonth, Class.MessageType.ERROR);


        }


        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            ConfirmEmpCommand = new RelayCommand<object>(ConfirmEmpAction);
            SearchCommand = new RelayCommand<object>(SearchAction);
            DataChangeCommand = new RelayCommand<object>(DataChangeAction);
        }

        private bool CheckPassWord()
        {

            //1.如果全部都沒有,查無帳號,請確認帳號
            if (SingInEmployee.CheckEmployeeAccountSame())
            {
                MessageWindow.ShowMessage("此帳號不存在!", Class.MessageType.ERROR);
                return false;
            }

            MainWindow.ServerConnection.OpenConnection();
            SingInEmployee = Employee.Login(SingInEmployee.Account, SingInEmployee.Password);
            MainWindow.ServerConnection.CloseConnection();

            //檢查帳密 密碼錯誤
            if (SingInEmployee == null)
            {
                MessageWindow.ShowMessage("密碼錯誤!", Class.MessageType.ERROR);
                return false;
            }


            return true;
        }
        public void GetDate()
        {
            if (Employee != null)
            {


                MainWindow.ServerConnection.OpenConnection();
                ClockInLogs = new ClockInLog(ClockInDb.ClockInLogByDate(System.DateTime.Now.Year.ToString(), SearchMonth, Employee.ID.ToString()));
                MainWindow.ServerConnection.CloseConnection();

                if (ClockInLogs is null) return;
                int iMin = 0;
                foreach (var s in ClockInLogs)
                {
                    iMin += (int)s.WMin;
                }
                HourCount = iMin / 60;
                MinCount = iMin % 60;

            }

        }


        #endregion ----- Define Functions -----





    }
}
