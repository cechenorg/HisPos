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

        private bool CanInsertCustomer => !string.IsNullOrEmpty(NewCustomer?.Name) &&
                                          !string.IsNullOrEmpty(NewCustomer?.IDNumber) &&
                                          NewCustomer?.Birthday != null;

        #endregion

        #region Commands
        public RelayCommand Submit { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion
        public AddCustomerWindowViewModel()
        {
            Submit = new RelayCommand(SubmitAction, CheckCanInsertCustomer);
            Cancel = new RelayCommand(CancelAction);
        }

        private void SubmitAction()
        {
            NewCustomer.InsertData();
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAddCustomerWindow"));
        }

        private bool CheckCanInsertCustomer()
        {
            return CanInsertCustomer;
        }
    }
}
