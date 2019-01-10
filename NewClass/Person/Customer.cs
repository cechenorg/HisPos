using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using His_Pos.NewClass.Person.CustomerHistory;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person
{
    public class Customer:INotifyPropertyChanged
    {
        public Customer() {}

        public Customer(DataRow r)
        {
            Id = (int)r["CUS_ID"];
            Name = r["CUS_NAME"]?.ToString();
            Gender = (bool)r["CUS_GENDER"] ? Properties.Resources.Male : Properties.Resources.Female;
            IcNumber = r["CUS_ICNUMBER"]?.ToString();
            Tel = r["CUS_TEL"]?.ToString();
            Phone = r["CUS_PHONE"]?.ToString();
            Address = r["CUS_ADDRESS"]?.ToString();
            EmergencyContact = r["CUS_EMERGENCY_CONTACT"]?.ToString();
            EmergencyTel = r["CUS_EMERGENCY_TEL"]?.ToString();
            ContactNote = r["CUS_CONTACT_NOTE"]?.ToString();
            Note = r["CUS_NOTE"]?.ToString();
        }
        public int Id { get; private set; }
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
        private string icNumber;//身分證字號
        public string IcNumber
        {
            get => icNumber;
            set
            {
                icNumber = value;
                OnPropertyChanged(nameof(IcNumber));
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
        private string phone;//手機
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged(nameof(Phone));
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
        private string emergencyContact;//緊急連絡人
        public string EmergencyContact
        {
            get => emergencyContact;
            set
            {
                emergencyContact = value;
                OnPropertyChanged(nameof(EmergencyContact));
            }
        }
        private string emergencyTel;//緊急連絡電話
        public string EmergencyTel
        {
            get => emergencyTel;
            set
            {
                emergencyTel = value;
                OnPropertyChanged(nameof(EmergencyTel));
            }
        }
        private string contactNote;//連絡備註
        public string ContactNote
        {
            get => contactNote;
            set
            {
                contactNote = value;
                OnPropertyChanged(nameof(ContactNote));
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
        private ObservableCollection<CustomerHistoryBase> history;//處方.交易.自費調劑紀錄
        public ObservableCollection<CustomerHistoryBase> History
        {
            get => history;
            set
            {
                history = value;
                OnPropertyChanged(nameof(History));
            }
        }
        #region Function
        public void Save() {
        }
        public void Delete(){
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
