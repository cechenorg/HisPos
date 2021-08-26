using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.CustomerDetailPrescription;
using His_Pos.NewClass.Prescription.CustomerDetailPrescription.CustomerDetailPrescriptionMedicine;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage
{
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
        public RelayCommand ShowMedicinesDetailCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }

    #endregion -----Define Command-----

    #region ----- Define Variables -----

    private List<string> prescriptionCaseString;

        public List<string> PrescriptionCaseString
        {
            get => prescriptionCaseString;
            set
            {
                Set(() => PrescriptionCaseString, ref prescriptionCaseString, value);
            }
        }

        private string prescriptionCaseSelectItem;

        public string PrescriptionCaseSelectItem
        {
            get => prescriptionCaseSelectItem;
            set
            {
                Set(() => PrescriptionCaseSelectItem, ref prescriptionCaseSelectItem, value);
                if (PrescriptionDetailViewSource is null) return;
                PrescriptionDetailViewSource.Filter += AdjustTypeFilter;
            }
        }

        private string textCusName;

        public string TextCusName
        {
            get { return textCusName; }
            set { Set(() => TextCusName, ref textCusName, value); }
        }

        private string searchOrderID;

        public string SearchOrderID
        {
            get { return searchOrderID; }
            set { Set(() => SearchOrderID, ref searchOrderID, value); }
        }


        private string phoneNumber;

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { Set(() => PhoneNumber, ref phoneNumber, value); }
        }

        private string idNumber;

        public string IdNumber
        {
            get { return idNumber; }
            set { Set(() => IdNumber, ref idNumber, value); }
        }

        private bool isEmployee;

        public bool IsEmployee
        {
            get 
            {
                return isEmployee;
            }
            set { Set(() => IsEmployee, ref isEmployee, value); }
        }

        private DateTime? textCusBirthDay;

        public DateTime? TextCusBirthDay
        {
            get { return textCusBirthDay; }
            set { Set(() => TextCusBirthDay, ref textCusBirthDay, value); }
        }

        public bool isDataChanged;

        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set { Set(() => IsDataChanged, ref isDataChanged, value); }
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
            set
            {
                Set(() => CustomerCollection, ref customerCollection, value);
            }
        }

        public Customer customer;

        public Customer Customer
        {
            get { return customer; }
            set
            {
                Set(() => Customer, ref customer, value);
                if (Customer is null) return;
                CustomerDetailPrescriptionCollection.GetDataByID(Customer.ID);
                CustomerDetailPrescriptionMedicines.Clear();
                if (CustomerDetailPrescriptionCollection.Count > 0)
                {
                    CustomerDetailPrescriptionSelectedItem = CustomerDetailPrescriptionCollection[0];
                }
                else
                {
                    CustomerDetailPrescriptionMedicines.Clear();
                }
                PrescriptionDetailViewSource = new CollectionViewSource { Source = CustomerDetailPrescriptionCollection };
                PrescriptionDetailView = PrescriptionDetailViewSource.View;
                PrescriptionDetailViewSource.Filter += AdjustTypeFilter;
            }
        }

        public Genders genders = new Genders();

        public Genders Genders
        {
            get { return genders; }
            set
            {
                Set(() => Genders, ref genders, value);
            }
        }

        private CollectionViewSource prescriptionDetailViewSource;

        private CollectionViewSource PrescriptionDetailViewSource
        {
            get => prescriptionDetailViewSource;
            set
            {
                Set(() => PrescriptionDetailViewSource, ref prescriptionDetailViewSource, value);
            }
        }

        private ICollectionView prescriptionDetailView;

        public ICollectionView PrescriptionDetailView
        {
            get => prescriptionDetailView;
            private set
            {
                Set(() => PrescriptionDetailView, ref prescriptionDetailView, value);
            }
        }

        private CustomerDetailPrescriptions customerDetailPrescriptionCollection = new CustomerDetailPrescriptions();

        public CustomerDetailPrescriptions CustomerDetailPrescriptionCollection
        {
            get => customerDetailPrescriptionCollection;
            set
            {
                Set(() => CustomerDetailPrescriptionCollection, ref customerDetailPrescriptionCollection, value);
            }
        }

        private CustomerDetailPrescription customerDetailPrescriptionSelectedItem = new CustomerDetailPrescription();

        public CustomerDetailPrescription CustomerDetailPrescriptionSelectedItem
        {
            get => customerDetailPrescriptionSelectedItem;
            set
            {
                Set(() => CustomerDetailPrescriptionSelectedItem, ref customerDetailPrescriptionSelectedItem, value);
            }
        }

        private CustomerDetailPrescriptionMedicines customerDetailPrescriptionMedicines = new CustomerDetailPrescriptionMedicines();

        public CustomerDetailPrescriptionMedicines CustomerDetailPrescriptionMedicines
        {
            get => customerDetailPrescriptionMedicines;
            set
            {
                Set(() => CustomerDetailPrescriptionMedicines, ref customerDetailPrescriptionMedicines, value);
            }
        }

        #endregion ----- Define Variables -----

        public CustomerManageViewModel()
        {
            TabName = MainWindow.HisFeatures[5].Functions[4];
            Icon = MainWindow.HisFeatures[5].Icon;
            Messenger.Default.Register<NotificationMessage<string>>(this, GetSelectedCustomer);
            DataChangeCommand = new RelayCommand(DataChangeAction);
            CancelCommand = new RelayCommand(CancelAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            SubmitCommand = new RelayCommand(SubmitAction);
            SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
            SearchCommand = new RelayCommand(SearchAction);
            ClearCommand = new RelayCommand(ClearAction);
            ShowMedicinesDetailCommand = new RelayCommand(ShowMedicinesDetailAction);
            PrescriptionCaseString = new List<string>() { "全部", "調劑", "登錄", "預約" };
            PrescriptionCaseSelectItem = PrescriptionCaseString[0];
            RegisterMessengers();
        }

        private void DeleteAction()
        {
            if (Customer != null)
            {
                ConfirmWindow cw = new ConfirmWindow("是否刪除顧客(無法復原)", "刪除確認");
                if (!(bool)cw.DialogResult) { return; }

                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("ID", Customer.ID));
                 MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteCustomer]", parameters);
                MainWindow.ServerConnection.CloseConnection();
               
                MessageWindow.ShowMessage("刪除成功!", Class.MessageType.SUCCESS);
                Customer = new Customer();
                CustomerCollection = new Customers();

            }
        }

        private void GetSelectedCustomer(NotificationMessage<string> notificationMessage)
        {
            if (notificationMessage.Target == this)
            {
                MainWindow.Instance.AddNewTab(TabName);
                SearchOrderID = notificationMessage.Content;
                SearchIDAction();
            }

        }

        #region Action

        private void ClearAction()
        {
            //TextCusName = string.Empty;
            //TextCusBirthDay = null;
            //IdNumber = string.Empty;
            //PhoneNumber = string.Empty;
            //Customer = new Customer();
            //CustomerCollection = new Customers();

            //edit by shani 2021/07/14
            TextCusName = null;
            TextCusBirthDay = null;
            IdNumber = null;
            PhoneNumber = null;
        }

        private void SearchAction()
        {
            if (string.IsNullOrEmpty(TextCusName) && TextCusBirthDay == null && string.IsNullOrEmpty(IdNumber) && string.IsNullOrEmpty(PhoneNumber))
            {
                MessageWindow.ShowMessage("查詢欄位不可全空", Class.MessageType.WARNING);
                return;
            }

            var name = string.IsNullOrEmpty(TextCusName) ? null : TextCusName;
            var bday = TextCusBirthDay;
            var idnum = string.IsNullOrEmpty(IdNumber) ? null : IdNumber;
            var phone = string.IsNullOrEmpty(PhoneNumber) ? null : PhoneNumber;

            MainWindow.ServerConnection.OpenConnection();
            CustomerCollection.GetDataByNameOrBirth(name, bday, idnum, phone);
            MainWindow.ServerConnection.CloseConnection();
            if (CustomerCollection.Count > 0)
                Customer = NewFunction.DeepCloneViaJson(CustomerCollection[0]);
            InitDataChanged();
        }

        private void SearchIDAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            CustomerCollection.GetDataByNameOrBirth(SearchOrderID);
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

        public void DataChangeAction()
        {
            DataChanged();
        }

        public void CancelAction()
        {
            if (Customer != null) 
            {
                Customer = NewFunction.DeepCloneViaJson(CustomerCollection.Single(cus => cus.ID == Customer.ID));
                InitDataChanged();
            }
        }

        public void SubmitAction()
        {
            if (Customer != null) 
            {
                for (int i = 0; i < CustomerCollection.Count; i++)
                {
                    if (CustomerCollection[i].ID == Customer.ID)
                    {
                        CustomerCollection[i] = Customer;
                        Customer = NewFunction.DeepCloneViaJson(CustomerCollection.Single(cus => cus.ID == CustomerCollection[i].ID));
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(Customer.IDNumber))
                {
                    if (!VerifyService.VerifyIDNumber(Customer.IDNumber))
                    {
                        MessageWindow.ShowMessage("身分證格式錯誤!", Class.MessageType.ERROR);
                        return;
                    }
                    MainWindow.ServerConnection.OpenConnection();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("Cus_IDNumber", Customer.IDNumber));
                    DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[CheckCustomerIDs]", parameters);
                    MainWindow.ServerConnection.CloseConnection();
                    if (result.Rows.Count == 1)
                    {
                        MessageWindow.ShowMessage("身分證已存在!", Class.MessageType.ERROR);
                        return;
                    }
                }
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = CustomerDb.CheckCustomerByPhone(Customer.CellPhone, Customer.Tel);
                MainWindow.ServerConnection.CloseConnection();
                if (dataTable.Rows.Count == 1)
                {
                    CustomerDb.UpdateCustomerByPhone((int)dataTable.Rows[0]["Person_Id"], Customer.ID, Customer.CellPhone, Customer.Tel);
                    MessageWindow.ShowMessage("POS顧客已併入HIS!", Class.MessageType.SUCCESS);
                }

                if (!Customer.IsEnable)
                {
                    ConfirmWindow confirmWindow = new ConfirmWindow("關閉後會刪除此位客人病患所有預約慢箋 是否關閉?", "關閉病患視窗確認");
                    if ((bool)confirmWindow.DialogResult)
                    {
                        Customer.Save();
                        MessageWindow.ShowMessage("更新成功!", Class.MessageType.SUCCESS);
                    }
                }
                else
                    Customer.Save();
                InitDataChanged();
                MessageWindow.ShowMessage("更新成功!", Class.MessageType.SUCCESS);
            }
        }

        private void ShowMedicinesDetailAction()
        {
            if (CustomerDetailPrescriptionSelectedItem is null) return;
            CustomerDetailPrescriptionMedicines.GetDataByID(CustomerDetailPrescriptionSelectedItem.ID, CustomerDetailPrescriptionSelectedItem.TypeName);
        }

        #endregion Action

        #region Function

        private void DataChanged()
        {
            IsDataChanged = true;
            BtnCancelEnable = true;
            BtnSubmitEnable = true;
        }

        private void InitDataChanged()
        {
            if (Customer != null)
            {
                MainWindow.ServerConnection.OpenConnection();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CusID", Customer.ID));
                DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[IsEmployee]", parameters);
                MainWindow.ServerConnection.CloseConnection();
                if (result.Rows[0]["Cus_CusType"].ToString() == "1")
                {
                    isEmployee = true;
                }
                else 
                {
                    isEmployee = false;
                }
            }
            IsDataChanged = false;
            BtnCancelEnable = false;
            BtnSubmitEnable = false;
        }

        private void AdjustTypeFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is null) return;
            if (!(e.Item is CustomerDetailPrescription src))
                e.Accepted = false;

            e.Accepted = false;
            CustomerDetailPrescription customerDetailPrescription = (CustomerDetailPrescription)e.Item;
            if (customerDetailPrescription.TypeName == PrescriptionCaseSelectItem)
                e.Accepted = true;
            else if (PrescriptionCaseSelectItem == "全部")
                e.Accepted = true;
        }

        #endregion Function
        private void RegisterMessengers()
        {
            Messenger.Default.Register<NotificationMessage<string>>(this, ShowOrderDetailByOrderID);
        }

        private void ShowOrderDetailByOrderID(NotificationMessage<string> notificationMessage)
        {
            if (notificationMessage.Target == this)
            {
                MainWindow.Instance.AddNewTab(TabName);

                SearchOrderID = notificationMessage.Content;

                SearchIDAction();
            }
        }
    }
}