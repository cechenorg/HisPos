using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person
{
    public class Employee:INotifyPropertyChanged
    {
        public Employee(){}
        public Employee(DataRow r)
        {
            Id = (int) r["EMP_ID"];
            Name = r["EMP_NAME"].ToString();
            IcNumber = r["EMP_ICNUMBER"]?.ToString();
            Birthday = (DateTime?)r["EMP_BIRTHDAY"];
        }
        public int Id { get; }
        private string name;//姓名
        public string Name
        {
            get =>name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string icNumber;//姓名
        public string IcNumber
        {
            get => icNumber;
            set
            {
                icNumber = value;
                OnPropertyChanged(nameof(IcNumber));
            }
        }
        private DateTime? birthday;//姓名
        public DateTime? Birthday
        {
            get => birthday;
            set
            {
                birthday = value;
                OnPropertyChanged(nameof(Birthday));
            }
        }
        #region Function
        public void Save()
        {
            ///存檔後更新自己
        }
        public void Delete()
        {
            
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
