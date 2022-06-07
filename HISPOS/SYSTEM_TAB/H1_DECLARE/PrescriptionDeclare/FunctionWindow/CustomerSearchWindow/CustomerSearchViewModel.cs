using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.Service;
using System;
using System.ComponentModel;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSearchWindow
{
    public enum CustomerSearchCondition
    {
        IDNumber = 0,
        Name = 1,
        Birthday = 2,
        CellPhone = 3,
        Tel = 4
    }

    public class CustomerSearchViewModel : ViewModelBase
    {
        private string selectedRadioButton;

        public string SelectedRadioButton
        {
            get => selectedRadioButton;
            set
            {
                Set(() => SelectedRadioButton, ref selectedRadioButton, value);
                if (string.IsNullOrEmpty(selectedRadioButton)) return;
                switch (selectedRadioButton)
                {
                    case "Option1":
                        SearchCondition = CustomerSearchCondition.IDNumber;
                        break;

                    case "Option2":
                        SearchCondition = CustomerSearchCondition.Name;
                        break;

                    case "Option3":
                        SearchCondition = CustomerSearchCondition.Birthday;
                        break;

                    case "Option4":
                        SearchCondition = CustomerSearchCondition.CellPhone;
                        break;

                    case "Option5":
                        SearchCondition = CustomerSearchCondition.Tel;
                        break;
                }
            }
        }

        private Customers Customers { get; set; }
        private CollectionViewSource customerCollectionViewSource;

        private CollectionViewSource CustomerCollectionViewSource
        {
            get => customerCollectionViewSource;
            set
            {
                Set(() => CustomerCollectionViewSource, ref customerCollectionViewSource, value);
            }
        }

        private ICollectionView customerCollectionView;

        public ICollectionView CustomerCollectionView
        {
            get => customerCollectionView;
            private set
            {
                Set(() => CustomerCollectionView, ref customerCollectionView, value);
                if (customerCollectionView.IsEmpty) return;
                CustomerCollectionViewSource.View.MoveCurrentToFirst();
                SelectedCustomer = (Customer)CustomerCollectionViewSource.View.CurrentItem;
            }
        }

        private string search;

        public string Search
        {
            get => search;
            set { Set(() => Search, ref search, value); }
        }

        private bool isEditing = true;

        public bool IsEditing
        {
            get => isEditing;
            private set
            {
                Set(() => IsEditing, ref isEditing, value);
            }
        }

        public bool ShowDialog { get; private set; }
        private CustomerSearchCondition SearchCondition { get; set; }
        public RelayCommand SearchTextChanged { get; set; }
        public RelayCommand CustomerSelected { get; set; }
        public RelayCommand<string> FocusUpDownCommand { get; set; }
        public RelayCommand StartEditingCommand { get; set; }
        private Customer selectedCustomer;

        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set { Set(() => SelectedCustomer, ref selectedCustomer, value); }
        }

        public CustomerSearchViewModel(string search, CustomerSearchCondition condition)
        {
            Customers = new Customers();
            SearchCondition = condition;
            SearchTextChanged = new RelayCommand(ExecuteSearchTextChanged);
            CustomerSelected = new RelayCommand(ExecuteCustomerSelected);
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            StartEditingCommand = new RelayCommand(StartEditingAction);
            if (string.IsNullOrEmpty(search))
            {
                switch (SearchCondition)
                {
                    case CustomerSearchCondition.IDNumber:
                        SelectedRadioButton = "Option1";
                        break;

                    case CustomerSearchCondition.Name:
                        SelectedRadioButton = "Option2";
                        break;

                    case CustomerSearchCondition.CellPhone:
                        SelectedRadioButton = "Option4";
                        break;

                    case CustomerSearchCondition.Tel:
                        SelectedRadioButton = "Option5";
                        break;
                }
                Customers.GetTodayEdited();
            }
            else
            {
                switch (SearchCondition)
                {
                    case CustomerSearchCondition.IDNumber:
                        SelectedRadioButton = "Option1";
                        Customers.SearchCustomers(search, null, null, null, null);
                        break;

                    case CustomerSearchCondition.Name:
                        SelectedRadioButton = "Option2";
                        Customers.SearchCustomers(null, search, null, null, null);
                        break;

                    case CustomerSearchCondition.CellPhone:
                        SelectedRadioButton = "Option4";
                        Customers.SearchCustomers(null, null, search, null, null);
                        break;

                    case CustomerSearchCondition.Tel:
                        SelectedRadioButton = "Option5";
                        Customers.SearchCustomers(null, null, null, search, null);
                        break;
                }
            }
            switch (Customers.Count)
            {
                case 0:
                    ShowDialog = false;
                    MessageWindow.ShowMessage("查無顧客", MessageType.WARNING);
                    AskAddCustomerData();
                    return;

                case 1 when SearchCondition.Equals(CustomerSearchCondition.IDNumber):
                    ShowDialog = false;
                    SelectedCustomer = Customers[0];
                    ExecuteCustomerSelected();
                    break;

                default:
                    ShowDialog = true;
                    break;
            }
            CustomerCollectionViewSource = new CollectionViewSource { Source = Customers };
            CustomerCollectionView = CustomerCollectionViewSource.View;
            Search = search;
            if (!string.IsNullOrEmpty(search))
                ExecuteSearchTextChanged();
            else
                IsEditing = false;
        }

        public CustomerSearchViewModel(string search, CustomerSearchCondition condition, int phone)
        {
            Customers = new Customers();
            SearchCondition = condition;
            SearchTextChanged = new RelayCommand(ExecuteSearchTextChanged);
            CustomerSelected = new RelayCommand(ExecuteCustomerSelected);
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            StartEditingCommand = new RelayCommand(StartEditingAction);
            if (string.IsNullOrEmpty(search))
            {
                switch (SearchCondition)
                {
                    case CustomerSearchCondition.IDNumber:
                        SelectedRadioButton = "Option1";
                        break;

                    case CustomerSearchCondition.Name:
                        SelectedRadioButton = "Option2";
                        break;

                    case CustomerSearchCondition.CellPhone:
                        SelectedRadioButton = "Option4";
                        break;

                    case CustomerSearchCondition.Tel:
                        SelectedRadioButton = "Option5";
                        break;
                }
                Customers.GetTodayEdited();
            }
            else
            {
                switch (SearchCondition)
                {
                    case CustomerSearchCondition.IDNumber:
                        SelectedRadioButton = "Option1";
                        Customers.SearchCustomers(search, null, null, null, null);
                        break;

                    case CustomerSearchCondition.Name:
                        SelectedRadioButton = "Option2";
                        Customers.SearchCustomers(null, search, null, null, null);
                        break;

                    case CustomerSearchCondition.CellPhone:
                        SelectedRadioButton = "Option4";
                        Customers.SearchCustomers(null, null, search, null, null);
                        break;

                    case CustomerSearchCondition.Tel:
                        SelectedRadioButton = "Option5";
                        Customers.SearchCustomers(null, null, null, search, null);
                        break;
                }
            }
            switch (Customers.Count)
            {
                case 0:
                    ShowDialog = false;
                    //MessageWindow.ShowMessage("查無顧客", MessageType.WARNING);
                    AskAddCustomerData();
                    return;

                case 1:/* when SearchCondition.Equals(CustomerSearchCondition.IDNumber):*/
                    ShowDialog = false;
                    SelectedCustomer = Customers[0];
                    ExecuteCustomerSelected();
                    break;

                default:
                    ShowDialog = true;
                    break;
            }
            CustomerCollectionViewSource = new CollectionViewSource { Source = Customers };
            CustomerCollectionView = CustomerCollectionViewSource.View;
            Search = search;
            if (!string.IsNullOrEmpty(search))
                ExecuteSearchTextChanged();
            else
                IsEditing = false;
        }

        public CustomerSearchViewModel(DateTime? birth)
        {
            SearchCondition = CustomerSearchCondition.Birthday;
            SelectedRadioButton = "Option3";
            Customers = new Customers();
            if (birth is null)
            {
                Customers.GetTodayEdited();
            }
            else
            {
                Search = DateTimeExtensions.ConvertToTaiwanCalender((DateTime)birth);
                Customers.SearchCustomers(null, null, null, null, birth);
            }

            SearchTextChanged = new RelayCommand(ExecuteSearchTextChanged);
            CustomerSelected = new RelayCommand(ExecuteCustomerSelected);
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            switch (Customers.Count)
            {
                case 0:
                    ShowDialog = false;
                    MessageWindow.ShowMessage("查無顧客", MessageType.WARNING);
                    AskAddCustomerData();
                    break;

                default:
                    ShowDialog = true;
                    break;
            }
            CustomerCollectionViewSource = new CollectionViewSource { Source = Customers };
            CustomerCollectionView = CustomerCollectionViewSource.View;
            Search = search;
            if (birth != null)
                ExecuteSearchTextChanged();
            else
                IsEditing = false;
        }

        private void ExecuteCustomerSelected()
        {
            Messenger.Default.Send(new NotificationMessage<Customer>(SelectedCustomer, "GetSelectedCustomer"));
            Messenger.Default.Send(new NotificationMessage("CloseCustomerSearchWindow"));
        }

        private void AskAddCustomerData()
        {
            SelectedCustomer = null;
            Messenger.Default.Send(new NotificationMessage<Customer>(SelectedCustomer, "AskAddCustomerData"));
            Messenger.Default.Send(new NotificationMessage("CloseCustomerSearchWindow"));
        }

        private void FocusUpDownAction(string direction)
        {
            if (IsEditing || Customers.Count <= 0) return;

            var maxIndex = Customers.Count - 1;
            switch (direction)
            {
                case "UP":
                    if (CustomerCollectionView.CurrentPosition > 0)
                        CustomerCollectionView.MoveCurrentToPrevious();
                    break;

                case "DOWN":
                    if (CustomerCollectionView.CurrentPosition < maxIndex)
                        CustomerCollectionView.MoveCurrentToNext();
                    break;
            }
            SelectedCustomer = (Customer)CustomerCollectionView.CurrentItem;
        }

        private void ExecuteSearchTextChanged()
        {
            if (IsEditing)
            {
                IsEditing = false;
                Customers.Clear();
                switch (SearchCondition)
                {
                    case CustomerSearchCondition.IDNumber:
                        Customers.SearchCustomers(search, null, null, null, null);
                        break;

                    case CustomerSearchCondition.Name:
                        Customers.SearchCustomers(null, search, null, null, null);
                        break;

                    case CustomerSearchCondition.Birthday:
                        var searchDate = DateTimeExtensions.TWDateStringToDateOnly(Search);
                        if (searchDate is null) return;
                        Customers.SearchCustomers(null, null, null, null, searchDate);
                        break;

                    case CustomerSearchCondition.CellPhone:
                        Customers.SearchCustomers(null, null, search, null, null);
                        break;

                    case CustomerSearchCondition.Tel:
                        Customers.SearchCustomers(null, null, null, search, null);
                        break;
                }
                CustomerCollectionViewSource.Filter += Filter;
                switch (Customers.Count)
                {
                    case 0:
                        MessageWindow.ShowMessage("查無顧客", MessageType.WARNING);
                        break;

                    default:
                        CustomerCollectionView.MoveCurrentToFirst();
                        SelectedCustomer = (Customer)CustomerCollectionView.CurrentItem;
                        break;
                }
            }
            else
            {
                ExecuteCustomerSelected();
            }
        }

        private void StartEditingAction()
        {
            IsEditing = true;
        }

        #region FilterFunctions

        private void Filter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Customer src))
                e.Accepted = false;
            else if (string.IsNullOrEmpty(Search))
                e.Accepted = true;
            else
            {
                switch (SelectedRadioButton)
                {
                    case "Option1":
                        e.Accepted = FilterByIDNumber(src);
                        break;

                    case "Option2":
                        e.Accepted = FilterByName(src);
                        break;

                    case "Option3":
                        e.Accepted = FilterByBirthDay(src);
                        break;

                    case "Option4":
                        e.Accepted = FilterByCellPhone(src);
                        break;

                    case "Option5":
                        e.Accepted = FilterByTel(src);
                        break;
                }
            }
        }

        private bool FilterByBirthDay(Customer c)
        {
            var birth = DateTimeExtensions.TWDateStringToDateOnly(Search);
            if (birth is null) return false;
            if (c.Birthday is null)
                return false;
            return DateTime.Compare((DateTime)birth, (DateTime)c.Birthday) == 0;
        }

        private bool FilterByName(Customer c)
        {
            return !string.IsNullOrEmpty(c.Name) ? c.Name.Contains(Search) : c.Name.Contains(Search);
        }

        private bool FilterByIDNumber(Customer c)
        {
            return !string.IsNullOrEmpty(c.IDNumber) && c.IDNumber.Contains(Search);
        }

        private bool FilterByCellPhone(Customer c)
        {
            return (!string.IsNullOrEmpty(c.CellPhone)|| !string.IsNullOrEmpty(c.SecondPhone)) && (c.CellPhone.Contains(Search)|| c.SecondPhone.Contains(Search));
        }

        private bool FilterByTel(Customer c)
        {
            return !string.IsNullOrEmpty(c.Tel) && c.Tel.Contains(Search);
        }

        #endregion FilterFunctions
    }
}