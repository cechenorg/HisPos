using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customer:Person,INotifyPropertyChanged
    {
        public Customer() {}

        public Customer(DataRow r) : base(r)
        { 
            ContactNote = r["Cus_UrgentNote"]?.ToString();
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
        #region Function
        public void Save()
        {
        }
        public void Delete()
        {
        }
        public Customer GetCustomerByCusId(int cusId)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = CustomerDb.GetCustomerByCusId(cusId);
            Customer customer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            MainWindow.ServerConnection.CloseConnection();
            return customer;
        }
        public Customer Check() {
            DataTable table = CustomerDb.CheckCustomer(this);
            Customer newcustomer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            return newcustomer;
        }
        public void UpdateEditTime() {
            CustomerDb.UpdateEditTime(Id);
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
