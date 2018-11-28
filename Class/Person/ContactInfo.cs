using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.Class.Person
{
    public class ContactInfo:INotifyPropertyChanged
    {
        public ContactInfo()
        {
        }

        public ContactInfo(string address, string tel, string email)
        {
            Address = address;
            Tel = tel;
            Email = email;
        }

        public string Address { get; set; }
        private string _tel;
        public string Tel
        {
            get => _tel;
            set
            {
                _tel = value;
                OnPropertyChanged(nameof(Tel));
            }
        }
        public string Email { get; set; } 
        private string phone;
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
