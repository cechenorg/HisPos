﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass;
using His_Pos.NewClass.Person.Employee;
using System.Data;
using DomainModel;
using DomainModel.Enum;


namespace His_Pos.SYSTEM_TAB.H5_ATTEND.ClockIn.AddClockInWindow
{
    public partial class AddClockInWindowViewModel : ViewModelBase
    {

      
        public RelayCommand ConfirmAddClockInCommand { get; set; }


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

        private IEmployeeService _employeeService = new EmployeeService(new EmployeeDb());
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
             
            Employee user = _employeeService.Login(EmployeeID, EmployeePassWord);

            
            //檢查帳密 密碼錯誤
            if (user == null)
            {
                MessageWindow.ShowMessage("密碼錯誤!", Class.MessageType.ERROR);
                return false;
            }
            Employee = user;

            return true;
        }

        #endregion ----- Define Functions -----
    }
}
