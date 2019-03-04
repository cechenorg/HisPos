using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using Customer = His_Pos.NewClass.Person.Customer.Customer;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomerSelectionWindow
{
    public class CustomerSelectionViewModel : ViewModelBase
    {
        #region Variables
        public enum RadioOptions { Option1, Option2, Option3, Option4 }
        private string selectedRadioButton;
        public string SelectedRadioButton
        {
            get => selectedRadioButton;
            set
            {
                Set(() => SelectedRadioButton, ref selectedRadioButton, value);
            }
        }

        private string searching;
        public string Searching
        {
            get => searching;
            set
            {
                Set(() => Searching, ref searching, value);
            }
        }

        private CollectionViewSource customersCollectionViewSource;
        private CollectionViewSource CustomersCollectionViewSource
        {
            get => customersCollectionViewSource;
            set
            {
                Set(() => CustomersCollectionViewSource, ref customersCollectionViewSource, value);
            }
        }

        private ICollectionView customersCollectionView;
        public ICollectionView CustomersCollectionView
        {
            get => customersCollectionView;
            set
            {
                Set(() => CustomersCollectionView, ref customersCollectionView, value);
                var c = customersCollectionView.Cast<Customer>().ToList();
                if (c.Count == 1)
                    SelectedCustomer = c[0];
            }
        }

        private Customers customers;
        private Customers Customers
        {
            get => customers;
            set
            {
                Set(() => Customers, ref customers, value);
            }
        }

        private Customer selectedCustomer;
        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                Set(() => SelectedCustomer, ref selectedCustomer, value);
            }
        }

        private bool showDialog;
        public bool ShowDialog
        {
            get => showDialog;
            set
            {
                Set(() => ShowDialog, ref showDialog, value);
            }
        }
        #endregion
        #region Commands
        public RelayCommand SearchingTextChanged { get; set; }
        private void ExecuteSearchingTextChanged()
        {
            if (string.IsNullOrEmpty(Searching))
                CustomersCollectionViewSource.Filter -= FilterBySearchingText;
            else
                CustomersCollectionViewSource.Filter += FilterBySearchingText;
        }
        public RelayCommand CustomerSelected { get; set; }
        private void ExecuteCustomerSelected()
        {
            MainWindow.ServerConnection.OpenConnection();
            SelectedCustomer.UpdateEditTime();
            MainWindow.ServerConnection.CloseConnection();
            MainWindow.ServerConnection.OpenConnection();
            SelectedCustomer.Histories = new CustomerHistories(SelectedCustomer.ID);
            MainWindow.ServerConnection.CloseConnection();
            Messenger.Default.Send(new NotificationMessage("CloseCustomerSelection"));
            Messenger.Default.Send(SelectedCustomer, "SelectedCustomer");
        }
        #endregion
        public CustomerSelectionViewModel(string condition, int option)
        {
            SearchingTextChanged = new RelayCommand(ExecuteSearchingTextChanged);
            CustomerSelected = new RelayCommand(ExecuteCustomerSelected);
            Customers = new Customers();
            MainWindow.ServerConnection.OpenConnection();
            Customers.Init();
            MainWindow.ServerConnection.CloseConnection();
            CustomersCollectionViewSource = new CollectionViewSource { Source = Customers };
            CustomersCollectionView = CustomersCollectionViewSource.View;
            Searching = condition;
            InitializeFilter(option);
            if (CustomersCollectionView.Cast<Customer>().ToList().Count == 1)
            {
                ShowDialog = false;
                SelectedCustomer = CustomersCollectionView.Cast<Customer>().ToList()[0];
                ExecuteCustomerSelected();
            }
            else
            {
                ShowDialog = true;
            }
        }

        #region FilterFunctions
        private void InitializeFilter(int option)
        {
            if (string.IsNullOrEmpty(Searching))
            {
                SelectedRadioButton = "Option1";
            }
            else
            {
                switch (option)
                {
                    case 1:
                        SelectedRadioButton = "Option1";
                        break;
                    case 2:
                        SelectedRadioButton = "Option2";
                        break;
                    case 3:
                        SelectedRadioButton = "Option3";
                        break;
                    case 4:
                        SelectedRadioButton = "Option4";
                        break;
                    default:
                        SelectedRadioButton = "Option1";
                        break;
                }
            }
            CustomersCollectionViewSource.Filter += FilterBySearchingText;
        }
        private void FilterBySearchingText(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Customer src))
                e.Accepted = false;
            else if (string.IsNullOrEmpty(Searching))
                e.Accepted = true;
            else
            {
                switch (SelectedRadioButton)
                {
                    case "Option1":
                        e.Accepted = FilterByBirthDay(src);
                        break;
                    case "Option2":
                        e.Accepted = FilterByName(src);
                        break;
                    case "Option3":
                        e.Accepted = FilterByIDNumber(src);
                        break;
                    case "Option4":
                        e.Accepted = FilterByTel(src);
                        break;
                }
            }
        }
        private bool FilterByBirthDay(Customer c)
        {
            DateTime birth;
            switch (Searching.Length)
            {
                case 7:
                    birth = new DateTime(int.Parse(Searching.Substring(0,3))+1911, int.Parse(Searching.Substring(3, 2)), int.Parse(Searching.Substring(5, 2)));
                    if (c.Birthday is null)
                        return false;
                    return DateTime.Compare(birth, (DateTime)c.Birthday) == 0;
                case 6:
                    birth = new DateTime(int.Parse(Searching.Substring(0, 2)) + 1911, int.Parse(Searching.Substring(2, 2)), int.Parse(Searching.Substring(4, 2)));
                    if (c.Birthday is null)
                        return false;
                    return DateTime.Compare(birth, (DateTime)c.Birthday) == 0;
                default:
                    return false;
            }
        }
        private bool FilterByName(Customer c)
        {
            if (!string.IsNullOrEmpty(c.Name))
                return c.Name.Contains(Searching);
            return c.Name.Contains(Searching);
        }
        private bool FilterByIDNumber(Customer c)
        {
            if (!string.IsNullOrEmpty(c.IDNumber))
                return c.IDNumber.Contains(Searching);
            return false;
        }
        private bool FilterByTel(Customer c)
        {
            if(!string.IsNullOrEmpty(c.Tel))
                return c.Tel.Contains(Searching);
            return false;
        }
        #endregion
    }
}
