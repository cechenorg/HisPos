using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace His_Pos.Class.Person
{
    public class Person : INotifyPropertyChanged
    {
        public Person()
        {
            Id = "";
            IcNumber = "";
            Name = "";
            Birthday = new DateTime();
        }

        public Person(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Name = dataRow["EMP_NAME"].ToString();
            IcNumber = dataRow["EMP_IDNUM"].ToString();
            Birthday = dataRow["EMP_BIRTH"].ConvertTo<DateTime>();
        }

        private string id;

        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _icNumber;

        public string IcNumber
        {
            get => _icNumber;
            set
            {
                _icNumber = value;
                OnPropertyChanged(nameof(IcNumber));
            }
        }

        private DateTime _birthday;

        public DateTime Birthday
        {
            get => _birthday;
            set
            {
                _birthday = value;
                OnPropertyChanged(nameof(Birthday));
                OnPropertyChanged("BirthdayStr");
            }
        }

        public string BirthdayStr
        {
            get => Birthday.ToString("yyyy/MM/dd");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}