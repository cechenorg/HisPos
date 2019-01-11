﻿using System;
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
            Password = r[""]?.ToString();
            NickName = r[""]?.ToString();
            WorkPositionId = (int)r[""];
            StartDate = (DateTime?)r[""];
            LeaveDate = (DateTime?)r[""];
            PurchaseLimit = (int)r[""];
            IsEnable = (bool)r[""];
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
        private int workPositionId;//職位ID
        public int WorkPositionId
        {
            get => workPositionId;
            set
            {
                workPositionId = value;
                OnPropertyChanged(nameof(WorkPositionId));
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
        public void Save()
        {
            ///存檔後更新自己
        }
        public void Delete()
        {
            
        }
        public bool Login(string Account,string Password) {
            return true;
        }
        public Collection<string> GetTabAuth() {
            Collection<string> tabAuths = new Collection<string>();
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
