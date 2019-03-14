using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Customer;
using His_Pos.Service;
using System;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage {
    public class CustomerManageViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        #region -----Define Command-----  
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand DataChangeCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        #endregion
        #region ----- Define Variables -----
        private string textCusName;
        public string TextCusName
        {
            get { return textCusName; }
            set { Set(() => TextCusName, ref textCusName, value); }
        }
        private DateTime? textCusBirthDay;
        public DateTime? TextCusBirthDay
        {
            get { return textCusBirthDay; }
            set { Set(() => TextCusBirthDay, ref textCusBirthDay, value); }
        }
        public bool btnCancelEnable;
        public bool BtnCancelEnable
        {
            get { return btnCancelEnable; }
            set { Set(() => BtnCancelEnable, ref btnCancelEnable, value); }
        }
        public bool btnSubmitEnable;
        public bool BtnSubmitEnable
        {
            get { return btnSubmitEnable; }
            set { Set(() => BtnSubmitEnable, ref btnSubmitEnable, value); }
        }
        public string changeText;
        public string ChangeText
        {
            get { return changeText; }
            set { Set(() => ChangeText, ref changeText, value); }
        }
        public string changeForeground;
        public string ChangeForeground
        {
            get { return changeForeground; }
            set { Set(() => ChangeForeground, ref changeForeground, value); }
        }
        
        public Customers customerCollection = new Customers();
        public Customers CustomerCollection
        {
            get { return customerCollection; }
            set { Set(() => CustomerCollection, ref customerCollection, value); }
        }
        public Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { 
                Set(() => Customer, ref customer, value);
            }
        }
        public Genders genders = new Genders();
        public Genders Genders
        {
            get { return genders; }
            set {
                Set(() => Genders, ref genders, value);
            }
        }
        #endregion
        public CustomerManageViewModel() { 
            DataChangeCommand = new RelayCommand(DataChangeAction);
            CancelCommand = new RelayCommand(CancelAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
            SearchCommand = new RelayCommand(SearchAction);
            ClearCommand = new RelayCommand(ClearAction);
        }
        #region Action
        private void ClearAction() {
            TextCusName = string.Empty;
            TextCusBirthDay = null;
        }
        private void SearchAction() {
            if (string.IsNullOrEmpty(TextCusName) && TextCusBirthDay == null) {
                MessageWindow.ShowMessage("姓名與生日其一不可為空",Class.MessageType.WARNING);
                return;
            }

            MainWindow.ServerConnection.OpenConnection(); 
            CustomerCollection.GetDataByNameOrBirth(TextCusName,TextCusBirthDay);
            MainWindow.ServerConnection.CloseConnection();
            if (CustomerCollection.Count > 0)
                Customer = NewFunction.DeepCloneViaJson(CustomerCollection[0]);
            InitDataChanged(); 
        }
        public void SelectionChangedAction()
        {
            if (Customer is null) return;
            Customer = NewFunction.DeepCloneViaJson(CustomerCollection.Single(cus => cus.ID == Customer.ID));
            InitDataChanged(); 
        }
        public void DataChangeAction() {
            DataChanged();
        }
        public void CancelAction() {
            Customer =  NewFunction.DeepCloneViaJson(CustomerCollection.Single(cus => cus.ID == Customer.ID));
            InitDataChanged(); 
        }
        public void SubmitAction()
        { 
            for (int i = 0; i < CustomerCollection.Count;i ++)
            {  
                if (CustomerCollection[i].ID == Customer.ID) { 
                    CustomerCollection[i] = Customer;
                    Customer = NewFunction.DeepCloneViaJson(CustomerCollection.Single(cus => cus.ID == CustomerCollection[i].ID));
                    break;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            Customer.Save();
            MainWindow.ServerConnection.CloseConnection();
            InitDataChanged();
        }
        #endregion

        #region Function 
        private void DataChanged()
        {
              
            ChangeText = "已修改";
            ChangeForeground = "Red"; 
            BtnCancelEnable = true;
            BtnSubmitEnable = true;
        }

        private void InitDataChanged()
        {
            ChangeText = "未修改";
            ChangeForeground = "Black";
            BtnCancelEnable = false;
            BtnSubmitEnable = false;
        }
        #endregion
    }
}
