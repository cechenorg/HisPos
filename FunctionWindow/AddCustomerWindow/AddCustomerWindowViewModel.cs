using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.NewClass.Person.Customer;
using His_Pos.Service;

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
        #endregion

        #region Commands
        public RelayCommand Submit { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion
        public AddCustomerWindowViewModel()
        {
            NewCustomer = new Customer();
            Submit = new RelayCommand(SubmitAction);
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
            Submit = new RelayCommand(SubmitAction);
            Cancel = new RelayCommand(CancelAction);
        }
        private void SubmitAction()
        {
            if(!CheckFormat()) return;
            var insertResult = NewCustomer.InsertData();
            if (insertResult)
                NewCustomerInsertSuccess();
        }

        private bool CheckFormat()
        {
            var errorString = string.Empty;
            if (string.IsNullOrEmpty(NewCustomer.Name.Trim()))
                errorString += "姓名未填寫\r\n";
            if (NewCustomer.Birthday is null)
                errorString += "生日未填寫\r\n";
            if (string.IsNullOrEmpty(NewCustomer.IDNumber) || !VerifyService.VerifyIDNumber(NewCustomer.IDNumber))
                errorString += "身分證格式錯誤\r\n";
            if(!string.IsNullOrEmpty(errorString))
                MessageWindow.ShowMessage(errorString,MessageType.ERROR);
            return string.IsNullOrEmpty(errorString);
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAddCustomerWindow"));
        }

        private void NewCustomerInsertSuccess()
        {
            Messenger.Default.Send(new NotificationMessage<Customer>(NewCustomer, "GetSelectedCustomer"));
            Messenger.Default.Send(new NotificationMessage("CloseAddCustomerWindow"));
        }
    }
}
