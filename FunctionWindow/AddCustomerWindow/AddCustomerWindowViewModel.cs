using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.NewClass.Person.Customer;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow;
using His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace His_Pos.FunctionWindow.AddCustomerWindow
{
    public class AddCustomerWindowViewModel : ViewModelBase
    {
        #region Variables
        public int ID;
        public NewClass.Prescription.Prescription CurrentPrescription { get; set; }
        private Customer newCustomer;

        public Customer NewCustomer
        {
            get => newCustomer;
            set
            {
                Set(() => NewCustomer, ref newCustomer, value);
            }
        }

        public bool IsTelephone(string str_telephone)
        {
            if (string.IsNullOrEmpty(str_telephone))
                return false;
            else
                return Regex.IsMatch(str_telephone, @"\d{2,3}\d{3,4}\d{4}");
        }

        public bool IsCellphone(string str_handset)
        {
            if (string.IsNullOrEmpty(str_handset))
                return false;
            else
                return Regex.IsMatch(str_handset, @"^09[0-9]{8}$");
        }

        private bool isMale;

        public bool IsMale
        {
            get => isMale;
            set
            {
                Set(() => IsMale, ref isMale, value);
            }
        }

        private bool isFemale;

        public bool IsFemale
        {
            get => isFemale;
            set
            {
                Set(() => IsFemale, ref isFemale, value);
            }
        }

        #endregion Variables

        #region Commands

        public RelayCommand Submit { get; set; }
        public RelayCommand Cancel { get; set; }

        #endregion Commands

        public AddCustomerWindowViewModel()
        {
            NewCustomer = new Customer();
            Submit = new RelayCommand(SubmitAction);
            Cancel = new RelayCommand(CancelAction);
        }

        public AddCustomerWindowViewModel(Customer customer = null)
        {
            NewCustomer = new Customer();
            IsMale = true;
            if (customer != null)
            {
                NewCustomer.IDNumber = customer.IDNumber;
                NewCustomer.Name = customer.Name;
                NewCustomer.Birthday = customer.Birthday;
                NewCustomer.CellPhone = customer.CellPhone;
                NewCustomer.SecondPhone = customer.SecondPhone;
                NewCustomer.Tel = customer.Tel;
                if (customer.Gender == Properties.Resources.Male)
                {
                    IsMale = true;
                    NewCustomer.Gender = Properties.Resources.Male;
                }
                else if (customer.Gender == Properties.Resources.Female)
                {
                    IsFemale = true;
                    NewCustomer.Gender = Properties.Resources.Female;
                }
                else
                {
                    IsMale = true;
                    NewCustomer.Gender = Properties.Resources.Male;
                }
            }

            Submit = new RelayCommand(SubmitAction);
            Cancel = new RelayCommand(CancelAction);
        }

        private void SubmitAction()
        {
            if (!CheckFormat()) return;
            if (IsMale == true)
            {
                NewCustomer.Gender = Properties.Resources.Male;
            }
            else if (IsFemale == true)
            {
                NewCustomer.Gender = Properties.Resources.Female;
            }
            var insertResult = NewCustomer.InsertNewData();

            if (insertResult == "SUCCESS")
            {
                NewCustomerInsertSuccess();
            }
            else {
                if (insertResult == "ID_SAME") 
                {
                    MessageWindow.ShowMessage("身分證字號已存在！", MessageType.ERROR);
                    return;
                }
                if (insertResult == "PHONE_SAME") 
                {
                    ConfirmWindow confirmWindow = new ConfirmWindow("有重複電話號碼是否進入選擇?", "重複電話號碼", true);
                    if (!(bool)confirmWindow.DialogResult)
                    {
                        //不判斷電話
                        var res = NewCustomer.InsertData(); 
                        if (res)
                        {
                            NewCustomerInsertSuccess();
                        }
                        return;
                    }
                }

                CustomerSearchWindow customerSearch;
                
                Messenger.Default.Register<NotificationMessage<NewClass.Person.Customer.Customer>>(this, GetSelectedCustomer);
                customerSearch = new CustomerSearchWindow(CustomerSearchCondition.CellPhone, 0, NewCustomer.CellPhone.Trim());
                Messenger.Default.Unregister<NotificationMessage<NewClass.Person.Customer.Customer>>(this);
                
                Messenger.Default.Send(new NotificationMessage("CloseAddCustomerWindow"));

                if (ProductTransactionView.FromHISCuslblcheck == null)
                {
                    return;
                }
                else if (CurrentPrescription != null) 
                {
                    if (CurrentPrescription.Patient.CellPhone != null || CurrentPrescription.Patient.CellPhone != "")
                    {
                        ProductTransactionView.FromHISCuslblcheck.Text = CurrentPrescription.Patient.ID.ToString();
                    }
                    else if (CurrentPrescription.Patient.Tel != null || CurrentPrescription.Patient.Tel != "" || ProductTransactionView.FromHISCuslblcheck != null)
                    {
                        ProductTransactionView.FromHISCuslblcheck.Text = CurrentPrescription.Patient.ID.ToString();
                    }
                }     
                else
                {
                    return;
                }
            }
        }
        private void GetSelectedCustomer(NotificationMessage<NewClass.Person.Customer.Customer> receiveSelectedCustomer)
        {
            Messenger.Default.Unregister<NotificationMessage<NewClass.Person.Customer.Customer>>(this);
            if (receiveSelectedCustomer.Content is null)
            {
                if (!receiveSelectedCustomer.Notification.Equals("AskAddCustomerData")) return;
            }
            else
            {
                CurrentPrescription = new NewClass.Prescription.Prescription();

                CurrentPrescription.Patient = new NewClass.Person.Customer.Customer();
                CurrentPrescription.Patient = receiveSelectedCustomer.Content;

                ID = CurrentPrescription.Patient.ID;

            }
        }
        private bool CheckFormat()
        {
            var errorString = string.Empty;
            //var clearedCell = Regex.Replace(NewCustomer.CellPhone, "[^0-9]", "");
            /*if (string.IsNullOrEmpty(NewCustomer.Name))
                errorString += "姓名未填寫\r\n";
            if (NewCustomer.Birthday is null)
                errorString += "生日未填寫\r\n";
            if (!string.IsNullOrEmpty(NewCustomer.CellPhone) && clearedCell.Length != 10)
                errorString += "手機格式錯誤\r\n";*/
            if (string.IsNullOrEmpty(NewCustomer.CellPhone) && string.IsNullOrEmpty(NewCustomer.Tel))
                errorString += "手機 / 家電必擇一填寫！\r\n";
            else if (!IsCellphone(NewCustomer.CellPhone) && !string.IsNullOrEmpty(NewCustomer.CellPhone))
                errorString += "手機號碼格式錯誤！\r\n";
            else if (!IsTelephone(NewCustomer.Tel) && !string.IsNullOrEmpty(NewCustomer.Tel))
                errorString += "家電號碼格式錯誤！\r\n";
            else if (!string.IsNullOrEmpty(NewCustomer.IDNumber) && !VerifyService.VerifyIDNumber(NewCustomer.IDNumber))
                errorString += "身分證格式錯誤\r\n";

            if (!string.IsNullOrEmpty(errorString))
                MessageWindow.ShowMessage(errorString, MessageType.ERROR);
            return string.IsNullOrEmpty(errorString);
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAddCustomerWindow"));
        }

        private void NewCustomerInsertSuccess()
        {
            Messenger.Default.Send(new NotificationMessage<Customer>(NewCustomer, "GetSelectedCustomer"));
            Messenger.Default.Send(new NotificationMessage("SuccessCloseAddCustomerWindow"));
            MessageWindow.ShowMessage("新增顧客成功!", MessageType.SUCCESS);
            if (ProductTransactionView.Cuslblcheck != null)
            {
                ProductTransactionView.Cuslblcheck.Text = "1";
            }
        }

        public class GenderConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return ((string)parameter == (string)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return (bool)value ? parameter : null;
            }
        }
    }
}