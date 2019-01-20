using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person.Employee
{
    public class Employee:Person,INotifyPropertyChanged
    {
        public Employee(){}
        public Employee(DataRow r):base(r)
        {
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
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string nickName;//暱稱
        public string NickName
        {
            get => nickName;
            set
            {
                nickName = value;
                OnPropertyChanged(nameof(NickName));
            }
        }  
        private int workPositionID;//職位ID
        public int WorkPositionID
        {
            get => workPositionID;
            set
            {
                workPositionID = value;
                OnPropertyChanged(nameof(WorkPositionID));
            }
        }
        private string workPositionName;//職位名稱
        public string WorkPositionName
        {
            get => workPositionName;
            set
            {
                workPositionName = value;
                OnPropertyChanged(nameof(WorkPositionName));
            }
        }
        private DateTime? startDate;//到職日
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }
        private DateTime? leaveDate;//離職日
        public DateTime? LeaveDate
        {
            get => leaveDate;
            set
            {
                leaveDate = value;
                OnPropertyChanged(nameof(LeaveDate));
            }
        }
        private int purchaseLimit;//員購上限
        public int PurchaseLimit
        {
            get => purchaseLimit;
            set
            {
                purchaseLimit = value;
                OnPropertyChanged(nameof(PurchaseLimit));
            }
        }
        private bool isEnable;//備註
        public bool IsEnable
        {
            get => isEnable;
            set
            {
                isEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }
        public int AuthorityValue { get; set; }
        #region Function
        public Employee Save()
        {
            DataTable table = EmployeeDb.Save(this);
            return new Employee(table.Rows[0]);
        }
        public void Delete()
        {
            
        }
        public static Employee Login(string Account,string Password) {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = EmployeeDb.EmployeeLogin(Account, Password);
            MainWindow.ServerConnection.CloseConnection();
            return table.Rows.Count == 0 ? null : new Employee(table.Rows[0]);
        }
        public Collection<string> GetTabAuth() {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = EmployeeDb.GetTabAuth(AuthorityValue);
            MainWindow.ServerConnection.CloseConnection(); 
            Collection<string> tabAuths = new Collection<string>();
            foreach (DataRow row in table.Rows) {
                tabAuths.Add(row["Aut_TabName"].ToString());
            }
            return tabAuths;
        }
        
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion 
    }
}
