using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person
{
    public class Employee:Person,INotifyPropertyChanged
    {
        public Employee(){}
        public Employee(DataRow r):base(r)
        {
            NickName = r[""]?.ToString();
            WorkPositionId = (int)r[""];
            StartDate = (DateTime?)r[""];
            LeaveDate = (DateTime?)r[""];
            PurchaseLimit = (int)r[""];
            IsEnable = (bool)r[""];
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
