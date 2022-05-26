﻿using His_Pos.FunctionWindow;
using His_Pos.NewClass.Pharmacy;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ZeroFormatter;

namespace His_Pos.NewClass.Person.Employee
{
    [ZeroFormattable]
    public class Employee : Person
    {
        public Employee()
        {
            Gender = "男";
            WorkPosition = new WorkPosition.WorkPosition();
            WorkPosition.WorkPositionId = 2;
            StartDate = DateTime.Today;
            Birthday = DateTime.Today;
            IsEnable = true;
            AuthorityValue = 4;

            AuthorityFullName = TransAuthorityFullName(AuthorityValue);
        }

        public Employee(DataRow r) : base(r)
        {
           // EmpID = r.Field<string>("ID");
            Account = r.Field<string>("Emp_Account");
            Password = r.Field<string>("Aut_Password");
            NickName = r.Field<string>("Emp_NickName");
            StartDate = r.Field<DateTime?>("Emp_StartDate");
            LeaveDate = r.Field<DateTime?>("Emp_LeaveDate");
            PurchaseLimit = r.Field<short>("Emp_PurchaseLimit");
            IsEnable = r.Field<bool>("Emp_IsEnable");
            AuthorityValue = r.Field<byte>("Aut_LevelID");
            IsLocal = r.Field<bool>("Emp_IsLocal");
            CashierID = r.Field<string>("Emp_CashierID");
            WorkPosition = new WorkPosition.WorkPosition(r);

            AuthorityFullName = TransAuthorityFullName(AuthorityValue);
        }

        public void GetGroupPharmacyAuthority()
        {
            
        }
         
        private string TransAuthorityFullName(int AuthorityValue)
        {
            string result = string.Empty;
            
            switch (AuthorityValue)
            {
                case 1:
                    return "系統管理員";
                case 2:
                    return "店長";
                case 3:
                    return "店員";
                case 4:
                    return "藥師"; 
            }


            return result;
        }

        private string cashierID;

        [IgnoreFormat]
        public virtual string CashierID
        {
            get => cashierID;
            set
            {
                Set(() => CashierID, ref cashierID, value);
            }
        }

        private string password;//密碼

        [Index(7)]
        public virtual string Password
        {
            get => password;
            set
            {
                Set(() => Password, ref password, value);
            }
        }

        private string nickName;//暱稱

        [IgnoreFormat]
        public virtual string NickName
        {
            get => nickName;
            set
            {
                Set(() => NickName, ref nickName, value);
            }
        }

        private WorkPosition.WorkPosition workPosition;

        [IgnoreFormat]
        public virtual WorkPosition.WorkPosition WorkPosition
        {
            get => workPosition;
            set
            {
                Set(() => WorkPosition, ref workPosition, value);
            }
        }

        private DateTime? startDate;//到職日

        [IgnoreFormat]
        public virtual DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime? leaveDate;//離職日

        [IgnoreFormat]
        public virtual DateTime? LeaveDate
        {
            get => leaveDate;
            set
            {
                Set(() => LeaveDate, ref leaveDate, value);
            }
        }

        private int purchaseLimit;//員購上限

        [IgnoreFormat]
        public virtual int PurchaseLimit
        {
            get => purchaseLimit;
            set
            {
                Set(() => PurchaseLimit, ref purchaseLimit, value);
            }
        }

        private bool isEnable;//備註

        [IgnoreFormat]
        public virtual bool IsEnable
        {
            get => isEnable;
            set
            {
                Set(() => IsEnable, ref isEnable, value);
            }
        }

        [IgnoreFormat]
        public int AuthorityValue { get; set; }

        [IgnoreFormat]
        public string AuthorityFullName { get; set; }

        private string account;//帳號

        [IgnoreFormat]
        public virtual string Account
        {
            get => account;
            set
            {
                Set(() => Account, ref account, value);
            }
        }

        private bool isLocal;//是否為本店新增

        [IgnoreFormat]
        public virtual bool IsLocal
        {
            get => isLocal;
            set
            {
                Set(() => IsLocal, ref isLocal, value);
            }
        }

        private Dictionary<string, WorkPosition.WorkPosition> groupPharmacyWorkPositionList;//在其他加盟藥局對應的職位

        [IgnoreFormat]
        public Dictionary<string, WorkPosition.WorkPosition> GroupPharmacyWorkPositionList
        {
            get => groupPharmacyWorkPositionList;
            set
            {
                Set(() => GroupPharmacyWorkPositionList, ref groupPharmacyWorkPositionList, value);
            }
        }

        #region Function

        public void InitGroupPharmacyWorkPositionList(List<string> groupServerList)
        {
            GroupPharmacyWorkPositionList = new Dictionary<string, WorkPosition.WorkPosition>();

            var employeeList= EmployeeDb.GetGroupPharmacyDataByID(groupServerList, ID);

            for(int i =0; i < groupServerList.Count; i++)
            {
                GroupPharmacyWorkPositionList.Add(groupServerList[i], employeeList[i].WorkPosition);
            } 
        }

        public Employee GetDataByID(int id)
        {
            DataTable table = EmployeeDb.GetDataByID(id);
            return new Employee(table.Rows[0]);
        }

        public bool CheckIdNumber()
        {
            if (string.IsNullOrEmpty(IDNumber)) return false;

            if (!VerifyService.VerifyIDNumber(IDNumber))
            {
                MessageWindow.ShowMessage("身分證格式錯誤!", Class.MessageType.ERROR);
                return false;
            }

            var table = EmployeeDb.CheckIdNumber(IDNumber);
            if (table.Rows[0].Field<int>("Count") > 0)
            {
                MessageWindow.ShowMessage("此身分證已經存在!", Class.MessageType.ERROR);
                return false;
            }

            return true;
        }

        public bool CheckEmployeeAccountSame()
        {
            var table = EmployeeDb.CheckEmployeeAccountSame(Account);
            return table.Rows[0].Field<int>("Count") == 0 ? true : false;
        }

        public void Insert()
        {
            EmployeeDb.Insert(this);
        }

        public void Update()
        {
            EmployeeDb.Update(this);
        }

        public void Delete()
        {
            EmployeeDb.Delete(ID);
        }

        public string GetEmployeeNewAccount()
        {
            DataTable table = EmployeeDb.GetEmployeeNewAccount();
            return table.Rows[0].Field<string>("Account");
        }

        public static Employee Login(string Account, string Password)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = EmployeeDb.EmployeeLogin(Account, Password);
            MainWindow.ServerConnection.CloseConnection();
            return table.Rows.Count == 0 ? null : new Employee(table.Rows[0]);
        }

        public Collection<string> GetTabAuth()
        {
            DataTable table = EmployeeDb.GetTabAuth(AuthorityValue);
            Collection<string> tabAuths = new Collection<string>();
            foreach (DataRow row in table.Rows)
            {
                tabAuths.Add(row["Aut_TabName"].ToString());
            }
            return tabAuths;
        }

        #endregion Function

        public bool CheckLeave(DateTime date)
        {
            return StartDate <= date && (LeaveDate is null || LeaveDate >= date);
        }
    }

     
}