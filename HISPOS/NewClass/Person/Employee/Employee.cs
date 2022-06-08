using DomainModel;
using DomainModel.Enum;
using GalaSoft.MvvmLight;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Pharmacy;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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

            Authority = TranValueToAuthority(4);
            AuthorityFullName = Authority.GetDescriptionText(); 
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
            IsLocal = r.Field<bool>("Emp_IsLocal");
            CashierID = r.Field<string>("Emp_CashierID");
            WorkPosition = new WorkPosition.WorkPosition(r);

            byte tempAutID = r.Field<byte>("Aut_LevelID");
            Authority = TranValueToAuthority(tempAutID); 
            AuthorityFullName = Authority.GetDescriptionText();
        }

        private Authority TranValueToAuthority(int autVal)
        {
            switch (autVal)
            {
                case 1:
                    return Authority.系統管理員; 
                case 2:
                    return Authority.藥局經理; 
                case 3:
                    return Authority.會計人員; 
                case 4:
                    return Authority.店長;
                case 5:
                    return Authority.店員;
                case 6:
                    return Authority.負責藥師;
                case 7:
                    return Authority.執業藥師;
                case 8:
                    return Authority.支援藥師;
            }
            return Authority.店員;
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
        public Authority Authority { get; set; }
         
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

        private ObservableCollection<GroupWorkPosition> groupPharmacyEmployeeList;//在其他加盟藥局對應的職位

        [IgnoreFormat]
        public ObservableCollection<GroupWorkPosition> GroupPharmacyEmployeeList
        {
            get => groupPharmacyEmployeeList;
            set
            {
                Set(() => GroupPharmacyEmployeeList, ref groupPharmacyEmployeeList, value);
            }
        }

        private GroupWorkPosition selectedGroupPharmacyEmployee;//在其他加盟藥局對應的職位

        [IgnoreFormat]
        public GroupWorkPosition SelectedGroupPharmacyEmployee
        {
            get => selectedGroupPharmacyEmployee;
            set
            {
                Set(() => SelectedGroupPharmacyEmployee, ref selectedGroupPharmacyEmployee, value);
            }
        }

        #region Function

        public void InitGroupPharmacyWorkPositionList(List<PharmacyInfo> groupServerList,WorkPosition.WorkPositions workPositions)
        {
            GroupPharmacyEmployeeList = new ObservableCollection<GroupWorkPosition>();

            var employeeList= EmployeeDb.GetGroupPharmacyDataByID(groupServerList.Select(_ => _.PHAMAS_VerifyKey ).ToList(), ID);

            for(int i =0; i < groupServerList.Count; i++)
            {
                GroupWorkPosition tempData = new GroupWorkPosition()
                {
                    PharmacyName = groupServerList[i].PHAMAS_NAME,
                    PharmacyVerifyKey = groupServerList[i].PHAMAS_VerifyKey
                };
                tempData.EmployeeWorkPosition = workPositions.SingleOrDefault(_ => _.WorkPositionId == employeeList[i].WorkPosition.WorkPositionId);
                GroupPharmacyEmployeeList.Add(tempData);
            }

            SelectedGroupPharmacyEmployee = GroupPharmacyEmployeeList.FirstOrDefault(); 
            
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
            return EmployeeDb.EmployeeLogin(Account, Password);
        }

        public Collection<string> GetTabAuth()
        {
            DataTable table = EmployeeDb.GetTabAuth((int)Authority);
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

    public class GroupWorkPosition : ObservableObject
    {
        public string PharmacyVerifyKey { get; set; }
        public string PharmacyName { get; set; }


        private WorkPosition.WorkPosition _employeeWorkPosition;
        public WorkPosition.WorkPosition EmployeeWorkPosition {
            get { return _employeeWorkPosition; }
            set { 
                Set(() => EmployeeWorkPosition, ref _employeeWorkPosition, value); } 
        }
    }
}