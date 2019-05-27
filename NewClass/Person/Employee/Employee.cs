using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ZeroFormatter;

namespace His_Pos.NewClass.Person.Employee
{
    [ZeroFormattable]
    public class Employee:Person
    {
        public Employee(){}
        public Employee(DataRow r):base(r)
        {
            Account = r.Field<string>("Emp_Account");
            Password = r.Field<string>("Aut_Password");
            NickName = r.Field<string>("Emp_NickName");
            WorkPositionName = r.Field<string>("Emp_WorkPositionName");
            StartDate = r.Field<DateTime?>("Emp_StartDate");
            LeaveDate = r.Field<DateTime?>("Emp_LeaveDate");
            PurchaseLimit = r.Field<short>("Emp_PurchaseLimit");
            IsEnable = r.Field<bool>("Emp_IsEnable");
            AuthorityValue = r.Field<byte>("Aut_LevelID");
        }
        private string password;//密碼
        [Index(4)]
        public virtual string Password
        {
            get => password;
            set
            {
                Set(() => Password, ref password, value);
            }
        }

        private string nickName;//暱稱
        [Index(5)]
        public virtual string NickName
        {
            get => nickName;
            set
            {
                Set(() => NickName, ref nickName, value);
            }
        }  
        private int workPositionID;//職位ID
        [Index(6)]
        public virtual int WorkPositionID
        {
            get => workPositionID;
            set
            {
                Set(() => WorkPositionID, ref workPositionID, value);
            }
        }
        private string workPositionName;//職位名稱
        [Index(7)]
        public virtual string WorkPositionName
        {
            get => workPositionName;
            set
            {
                Set(() => WorkPositionName, ref workPositionName, value);
            }
        }
        private DateTime? startDate;//到職日
        [Index(8)]
        public virtual DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime? leaveDate;//離職日
        [Index(9)]
        public virtual DateTime? LeaveDate
        {
            get => leaveDate;
            set
            {
                Set(() => LeaveDate, ref leaveDate, value);
            }
        }
        private int purchaseLimit;//員購上限
        [Index(10)]
        public virtual int PurchaseLimit
        {
            get => purchaseLimit;
            set
            {
                Set(() => PurchaseLimit, ref purchaseLimit, value);
            }
        }
        private bool isEnable;//備註
        [Index(11)]
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
        #region Function
        public Employee Save()
        {
            DataTable table = EmployeeDb.Save(this);
            SaveServer();
            return new Employee(table.Rows[0]);
        }
        public void Delete()
        {
            EmployeeDb.Delete(ID); 
        }
        public static Employee Login(string Account,string Password) {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = EmployeeDb.EmployeeLogin(Account, Password);
            MainWindow.ServerConnection.CloseConnection();
            return table.Rows.Count == 0 ? null : new Employee(table.Rows[0]);
        }
        public Collection<string> GetTabAuth() { 
            DataTable table = EmployeeDb.GetTabAuth(AuthorityValue); 
            Collection<string> tabAuths = new Collection<string>();
            foreach (DataRow row in table.Rows) {
                tabAuths.Add(row["Aut_TabName"].ToString());
            }
            return tabAuths;
        }
        public string GetPassword() {
          DataTable table =  EmployeeDb.GetPassword(ID);
            return table.Rows[0]["Aut_Password"].ToString();
        }
        public void SaveServer() {
           EmployeeDb.SaveServer(this);
        }
        #endregion
    }
}
