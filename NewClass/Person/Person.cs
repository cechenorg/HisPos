using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person
{
    public class Person:INotifyPropertyChanged
    {
        public Person(){}

        public Person(DataRow r)
        {
            Id = (int)r[""];
            Name = r[""]?.ToString();
            IdNumber = r[""]?.ToString();
            CheckGender(r);
            Birthday = (DateTime?)r[""];
            Tel = r[""]?.ToString();
            CellPhone = r[""]?.ToString();
            Address = r[""]?.ToString();
            Email = r[""]?.ToString();
            Line = r[""]?.ToString();
            Note = r[""]?.ToString();
        }

        private int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        private string name;//姓名
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string gender;//性別
        public string Gender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        private string idNumber;//身分證字號
        public string IdNumber
        {
            get => idNumber;
            set
            {
                idNumber = value;
                OnPropertyChanged(nameof(IdNumber));
            }
        }
        private DateTime? birthday;//生日
        public DateTime? Birthday
        {
            get => birthday;
            set
            {
                birthday = value;
                OnPropertyChanged(nameof(Birthday));
            }
        }
        private string tel;//家電
        public string Tel
        {
            get => tel;
            set
            {
                tel = value;
                OnPropertyChanged(nameof(Tel));
            }
        }
        private string cellphone;//手機
        public string CellPhone
        {
            get => cellphone;
            set
            {
                cellphone = value;
                OnPropertyChanged(nameof(CellPhone));
            }
        }
        private string address;//地址
        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        private string email;//信箱
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        private string line;
        public string Line
        {
            get => line;
            set
            {
                line = value;
                OnPropertyChanged(nameof(Line));
            }
        }
        private string note;//備註
        public string Note
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged(nameof(Note));
            }
        }
        private void CheckGender(DataRow r)
        {
            if (!string.IsNullOrEmpty(r[""].ToString()))
                Gender = r[""].ToString();
            if (string.IsNullOrEmpty(IdNumber))
                Gender = Properties.Resources.Male;
            Gender = IdNumber[1].Equals('1') ? Properties.Resources.Male : Properties.Resources.Female;
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
