using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass;
using His_Pos.NewClass.Person.Employee;
using System.Data;


namespace His_Pos.SYSTEM_TAB.H5_ATTEND.ClockIn.AddClockInWindow
{
    public partial class AddClockInWindowViewModel : ViewModelBase
    {

        #region ----- Define Commands -----

        public RelayCommand ConfirmAddClockInCommand { get; set; }


        #endregion ----- Define Commands -----

        #region ----- Define Variables -----
        public string EmployeeID { get; set; }
        public string EmployeePassWord { get; set; }


        public Employee employee;

        public Employee Employee
        {
            get { return employee; }
            set
            {
                Set(() => Employee, ref employee, value);
            }
        }



        #endregion ----- Define Variables -----

        public AddClockInWindowViewModel()
        {
            Employee = new Employee();
            RegisterCommands();
        }

        #region ----- Define Actions -----

        private void ConfirmAddClockInAction()
        {
            if (!CheckPassWord()) return;  //檢查帳密




            ////增加一筆打卡紀錄
            //MainWindow.ServerConnection.OpenConnection();
            //DataTable dataTable = ManufactoryDB.AddNewManufactory(ManufactoryName, ManufactoryNickName, ManufactoryTelephone, ManufactoryAddress);
            //MainWindow.ServerConnection.CloseConnection();
            ////如果全部資料當中有,該店沒有 update 該帳號人員的IsLocal
            //if (dataTable.Rows.Count > 0) //查詢當天此人打卡紀錄
            //{
            //    ManufactoryManageDetail manufactory = new ManufactoryManageDetail(dataTable.Rows[0]);
            //    Messenger.Default.Send(new NotificationMessage<ManufactoryManageDetail>(this, manufactory, nameof(AddManufactoryWindowViewModel)));
            //}
            //else
            //    MessageWindow.ShowMessage("網路異常 新增失敗!", MessageType.ERROR);
        }


        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            ConfirmAddClockInCommand = new RelayCommand(ConfirmAddClockInAction);
        }

        private bool CheckPassWord()
        {
            //Employee user = Employee.Login(EmployeeID, (sender as PasswordBox)?.Password);
            //EmployeePassWord
            Employee.Account = EmployeeID;

            MainWindow.ServerConnection.OpenConnection();
            Employee user = Employee.Login(EmployeeID, EmployeePassWord);
            MainWindow.ServerConnection.CloseConnection();

            //1.如果全部都沒有,查無帳號,請確認帳號
            if (Employee.CheckEmployeeAccountSame())
            {
                MessageWindow.ShowMessage("此帳號不存在!", NewClass.MessageType.ERROR);
                return false;
            }
            //檢查帳密 密碼錯誤
            else if (user == null)
            {
                MessageWindow.ShowMessage("密碼錯誤!", NewClass.MessageType.ERROR);
                return false;
            }
            else
            {
                Employee = Employee.Login(EmployeeID, EmployeePassWord);
            }

            return true;
        }

        #endregion ----- Define Functions -----
    }
}
