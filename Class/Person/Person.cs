using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;

namespace His_Pos.Class.Person
{
    public class Person : INotifyPropertyChanged
    {
        public Person()
        {
        }

        public Person(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Name = dataRow["EMP_NAME"].ToString();
            IcNumber = dataRow["EMP_IDNUM"].ToString();
            Birthday = dataRow["EMP_BIRTH"].ToString();
        }

        private string id;

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string icNumber;

        public string IcNumber
        {
            get { return icNumber; }
            set
            {
                icNumber = value;
                NotifyPropertyChanged("IcNumber");
            }
        }

        private string birthday;

        public string Birthday
        {
            get { return birthday; }
            set
            {
                birthday = value;
                NotifyPropertyChanged("Birthday");
            }
        }

        public void CheckBirthDay(string customerBirthday)
        {
            Regex birth = new Regex(@"[0-9]{7}");
            if (birth.IsMatch(customerBirthday))
            {
                string year = customerBirthday.Substring(0, 3);
                string month = customerBirthday.Substring(3, 2);
                string date = customerBirthday.Substring(5, 2);
                Birthday = year + "/" + month + "/" + date;
            }
            else
            {
                //error
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}