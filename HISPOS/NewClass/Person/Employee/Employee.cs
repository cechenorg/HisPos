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

            Authority = EnumFactory.TranValueToAuthority(4);
        }

        //public Employee(DataRow r) : base(r)
        //{
        //   // EmpID = r.Field<string>("ID");
        //    Account = r.Field<string>("Emp_Account");
        //    Password = r.Field<string>("Aut_Password");
        //    NickName = r.Field<string>("Emp_NickName");
        //    StartDate = r.Field<DateTime?>("Emp_StartDate");
        //    LeaveDate = r.Field<DateTime?>("Emp_LeaveDate");
        //    PurchaseLimit = r.Field<short>("Emp_PurchaseLimit");
        //    IsEnable = r.Field<bool>("Emp_IsEnable"); 
        //    IsLocal = r.Field<bool>("Emp_IsLocal");
        //    CashierID = r.Field<string>("Emp_CashierID");
        //    WorkPosition = new WorkPosition.WorkPosition(r);

        //    byte tempAutID = r.Field<byte>("Aut_LevelID");
        //    Authority = TranValueToAuthority(tempAutID); 
        //}
         
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

        private WorkPosition.WorkPosition workPosition = new WorkPosition.WorkPosition();

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
        public string AuthorityFullName { get { return Authority.GetDescriptionText(); } }

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

        private ObservableCollection<GroupAuthority> groupPharmacyEmployeeList;//在其他加盟藥局對應的職位

        [IgnoreFormat]
        public ObservableCollection<GroupAuthority> GroupPharmacyEmployeeList
        {
            get => groupPharmacyEmployeeList;
            set
            {
                Set(() => GroupPharmacyEmployeeList, ref groupPharmacyEmployeeList, value);
            }
        }

        private GroupAuthority selectedGroupPharmacyEmployee;//在其他加盟藥局對應的職位

        [IgnoreFormat]
        public GroupAuthority SelectedGroupPharmacyEmployee
        {
            get => selectedGroupPharmacyEmployee;
            set
            {
                Set(() => SelectedGroupPharmacyEmployee, ref selectedGroupPharmacyEmployee, value);
            }
        }

        #region Function

        public bool IsPharmist()
        {
            return Authority == Authority.MasterPharmacist || Authority == Authority.NormalPharmacist || Authority == Authority.SupportPharmacist;
        }

        public bool IsLocalPharmist()
        {
            return Authority == Authority.MasterPharmacist || Authority == Authority.NormalPharmacist;
        }

        public void InitGroupPharmacyWorkPositionList(List<PharmacyInfo> groupServerList)
        {
            GroupPharmacyEmployeeList = new ObservableCollection<GroupAuthority>();

            var employeeList= EmployeeDb.GetGroupPharmacyDataByID(groupServerList.Select(_ => _.PHAMAS_VerifyKey ).ToList(), ID);

            for(int i =0; i < groupServerList.Count; i++)
            {
                GroupAuthority tempData = new GroupAuthority()
                {
                    PharmacyName = groupServerList[i].PHAMAS_NAME,
                    PharmacyVerifyKey = groupServerList[i].PHAMAS_VerifyKey,
                    EmployeeAuthority = employeeList[i].Authority
                };
                tempData.IsDirty = false;
                GroupPharmacyEmployeeList.Add(tempData);
            }

            SelectedGroupPharmacyEmployee = GroupPharmacyEmployeeList.FirstOrDefault(); 
            
        }
           
        #endregion Function

        public bool CheckLeave(DateTime date)
        {
            return StartDate <= date && (LeaveDate is null || LeaveDate >= date);
        }
    }

    public class GroupAuthority : ObservableObject
    {
        public string PharmacyVerifyKey { get; set; }
        public string PharmacyName { get; set; }


        private Authority _employeeAuthority;
        public Authority EmployeeAuthority {
            get { return _employeeAuthority; }
            set
            {
                Set(() => EmployeeAuthority, ref _employeeAuthority, value);
                IsDirty = true;
            } 
        }

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            set { Set(() => IsDirty, ref _isDirty, value); }
        }
    }
}