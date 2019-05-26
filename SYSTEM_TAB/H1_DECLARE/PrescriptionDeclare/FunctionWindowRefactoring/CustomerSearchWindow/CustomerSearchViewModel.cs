using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;
using His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.CooperativeClinicControl;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerSearchWindow
{
    public enum CustomerSearchCondition
    {
        IDNumber = 0,
        Name = 1,
        Birthday = 2,
        Tel = 3
    }
    public class CustomerSearchViewModel : ViewModelBase
    {
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
                if (!customerCollectionView.IsEmpty)
                {
                    CustomerCollectionViewSource.View.MoveCurrentToFirst();
                    SelectedCustomer = (Customer)CustomerCollectionViewSource.View.CurrentItem;
                }
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
            switch (SearchCondition)
            {
                case CustomerSearchCondition.IDNumber:
                    Customers.SearchCustomers(search,null,null,null);
                    break;
                case CustomerSearchCondition.Name:
                    Customers.SearchCustomers(null, search, null, null);
                    break;
                case CustomerSearchCondition.Tel:
                    Customers.SearchCustomers(null, null, search, null);
                    break;
            }
            SearchTextChanged = new RelayCommand(ExecuteSearchTextChanged);
            CustomerSelected = new RelayCommand(ExecuteCustomerSelected);
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            switch (Customers.Count)
            {
                case 0:
                    ShowDialog = false;
                    MessageWindow.ShowMessage("查無顧客", MessageType.WARNING);
                    break;
                case 1:
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
            ExecuteSearchTextChanged();
        }

        public CustomerSearchViewModel(DateTime birth)
        {
            SearchCondition = CustomerSearchCondition.Birthday;
            Customers = new Customers();
            Customers.SearchCustomers(null, null, null, birth);
            SearchTextChanged = new RelayCommand(ExecuteSearchTextChanged);
            CustomerSelected = new RelayCommand(ExecuteCustomerSelected);
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            switch (Customers.Count)
            {
                case 0:
                    ShowDialog = false;
                    MessageWindow.ShowMessage("查無顧客", MessageType.WARNING);
                    break;
                case 1:
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
            ExecuteSearchTextChanged();
        }

        private void ExecuteCustomerSelected()
        {
            Messenger.Default.Send(SelectedCustomer, "GetSelectedCustomer");
            Messenger.Default.Send(new NotificationMessage("CloseCustomerSearchWindow"));
        }

        private void FocusUpDownAction(string direction)
        {
            if (!IsEditing && Customers.Count > 0)
            {
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
        }

        private void ExecuteSearchTextChanged()
        {
            if (IsEditing)
            {
                IsEditing = false;
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

        private void Filter(object sender, FilterEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
