using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customer:Person,INotifyPropertyChanged
    {
        public Customer() {}

        public Customer(DataRow r) : base(r)
        {
            EmergencyContact = r[""]?.ToString();
            EmergencyTel = r[""]?.ToString();
            ContactNote = r[""]?.ToString();
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
