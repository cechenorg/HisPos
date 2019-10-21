using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer;

namespace His_Pos.FunctionWindow.AddCustomerWindow
{
    public class AddCustomerWindowViewModel : ViewModelBase
    {
        #region Variables

        private Customer newCustomer;

        public Customer NewCustomer
        {
            get => newCustomer;
            set
            {
                Set(() => NewCustomer, ref newCustomer, value);
            }
        }

        private bool CanInsertCustomer => NewCustomer.CheckData();

        #endregion

        #region Commands
        public RelayCommand Submit { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion
        public AddCustomerWindowViewModel()
        {
            NewCustomer = new Customer();
            Submit = new RelayCommand(SubmitAction, CheckCanInsertCustomer);
            Cancel = new RelayCommand(CancelAction);
        }

        public AddCustomerWindowViewModel(Customer customer = null)
        {
            NewCustomer = new Customer();
            if (customer != null)
            {
                NewCustomer.IDNumber = customer.IDNumber;
                NewCustomer.Name = customer.Name;
                NewCustomer.Birthday = customer.Birthday;
                NewCustomer.CellPhone = customer.CellPhone;
                NewCustomer.Tel = customer.Tel;
            }
            Submit = new RelayCommand(SubmitAction, CheckCanInsertCustomer);
            Cancel = new RelayCommand(CancelAction);
        }
        private void SubmitAction()
        {
            var insertResult = NewCustomer.InsertData();
            if (insertResult)
                NewCustomerInsertSuccess();
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAddCustomerWindow"));
        }

        private bool CheckCanInsertCustomer()
        {
            return CanInsertCustomer;
        }

        private void NewCustomerInsertSuccess()
        {
            Messenger.Default.Send(new NotificationMessage<Customer>(NewCustomer, "GetSelectedCustomer"));
            Messenger.Default.Send(new NotificationMessage("CloseAddCustomerWindow"));
        }
    }
}
