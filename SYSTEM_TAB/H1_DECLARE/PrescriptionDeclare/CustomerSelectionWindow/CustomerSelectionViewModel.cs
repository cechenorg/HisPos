using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Person.Customer;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CustomerSelectionWindow
{
    public class CustomerSelectionViewModel : ViewModelBase
    {
        #region Property
        public enum RadioOptions { Option1, Option2, Option3, Option4 }
        private string selectedRadioButton;
        public string SelectedRadioButton
        {
            get => selectedRadioButton;
            set { selectedRadioButton = value; RaisePropertyChanged(() => SelectedRadioButton); }
        }

        private string searching;
        public string Searching
        {
            get => searching;
            set { searching = value; RaisePropertyChanged(() => Searching); }
        }

        private CollectionViewSource customersCollectionViewSource;
        private CollectionViewSource CustomersCollectionViewSource
        {
            get => customersCollectionViewSource;
            set { customersCollectionViewSource = value; RaisePropertyChanged(() => CustomersCollectionViewSource); }
        }

        private ICollectionView customersCollectionView;
        public ICollectionView CustomersCollectionView
        {
            get => customersCollectionView;
            set { customersCollectionView = value; RaisePropertyChanged(() => CustomersCollectionView); }
        }

        private Customers customers;
        private Customers Customers
        {
            get => customers;
            set { customers = value; RaisePropertyChanged(() => Customers); }
        }

        private Customer selectedCustomer;
        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set { selectedCustomer = value; RaisePropertyChanged(() => SelectedCustomer); }
        }
        #endregion

        #region Command
        private RelayCommand searchingTextChanged;
        public RelayCommand SearchingTextChanged
        {
            get =>
                searchingTextChanged ??
                (searchingTextChanged = new RelayCommand(ExecuteSearchingTextChanged));
            set => searchingTextChanged = value;
        }
        private void ExecuteSearchingTextChanged()
        {
            if (string.IsNullOrEmpty(Searching))
                CustomersCollectionViewSource.Filter -= FilterBySearchingText;
            else
                CustomersCollectionViewSource.Filter += FilterBySearchingText;
        }


        #endregion

        public CustomerSelectionViewModel()
        {

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
                        //itemSourceList.Filter += FilterByName;
                        break;
                    case "Option3":
                        //itemSourceList.Filter += FilterByIcNumber;
                        break;
                    case "Option4":
                        //itemSourceList.Filter += FilterByTel;
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
                    return DateTime.Compare(birth, (DateTime)c.Birthday) == 0;
                case 6:
                    birth = new DateTime(int.Parse(Searching.Substring(0, 2)) + 1911, int.Parse(Searching.Substring(2, 2)), int.Parse(Searching.Substring(4, 2)));
                    return DateTime.Compare(birth, (DateTime)c.Birthday) == 0;
                default:
                    return false;
            }
        }
    }
}
