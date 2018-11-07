using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Person;
using JetBrains.Annotations;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// CustomerSelectWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSelectWindow : Window,INotifyPropertyChanged
    {
        public enum RadioOptions { Option1, Option2, Option3, Option4 }

        private bool conditionNotNull;
        string _selectedRadioButton;
        public string SelectedRadioButton
        {
            get
            {
                return _selectedRadioButton;
            }
            set
            {
                if (value != null)
                {
                    _selectedRadioButton = value;
                    Condition.Focus();
                    FilterCustomer(Condition.Text);
                }
            }
        }
        private ObservableCollection<Customer> _customerCollection;
        public ObservableCollection<Customer> CustomerCollection
        {
            get => _customerCollection;
            set
            {
                _customerCollection = value;
                OnPropertyChanged(nameof(CustomerCollection));
            }
        }

        private ObservableCollection<Customer> _resultCollection;
        public ObservableCollection<Customer> ResultCollection
        {
            get => _resultCollection;
            set
            {
                _resultCollection = value;
                OnPropertyChanged(nameof(ResultCollection));
            }
        }

        private Customer SelectedCustomer { get; set; }
        public CustomerSelectWindow(string condition,int option)
        {
            InitializeComponent();
            CustomerCollection = CustomerDb.GetData();
            DataContext = this;
            Condition.Text = condition;
            ResultCollection = new ObservableCollection<Customer>();
            SelectedCustomer = new Customer();
            Condition.Focus();
            if (string.IsNullOrEmpty(condition))
            {
                SelectedRadioButton = "Option1";
                conditionNotNull = false;
            }
            else
            {
                conditionNotNull = true;
                switch (option)
                {
                    case 0:
                        SelectedCustomer = CustomerCollection.SingleOrDefault(c => c.Id.Equals("0"));
                        SetSelectedCustomer();
                        break;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Customer_Select_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CusGrid.SelectedItem is null) return;
            SelectedCustomer = CusGrid.SelectedItem as Customer;
            if(SelectedCustomer != null && SelectedCustomer.IcNumber.Length == 10)
                SelectedCustomer.IcCard = new IcCard {IcNumber = SelectedCustomer.IcNumber};
            SetSelectedCustomer();
        }

        private void MatchCustomer(Customer match)
        {
            ResultCollection.Add(match);
            CusGrid.SelectedIndex = 0;
        }

        public ObservableCollection<Customer> ToObservableCollection<T>
            (IEnumerable<Customer> source)
        {
            return source == null ? new ObservableCollection<Customer>() : new ObservableCollection<Customer>(source);
        }

        private void Condition_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox txt)) return;
            if (string.IsNullOrEmpty(txt.Text))
            {
                ResultCollection = CustomerCollection;
                return;
            }
            FilterCustomer(txt.Text);
        }

        private string ToBirthStr(DateTime d)
        {
            var year = (d.Year - 1911).ToString();
            var month = d.Month.ToString().PadLeft(2, '0');
            var day = d.Day.ToString().PadLeft(2, '0');
            return year + month + day;
        }

        private void FilterCustomer(string condition)
        {
            var itemSourceList = new CollectionViewSource() { Source = CustomerCollection };
            var itemlist = itemSourceList.View;
            switch (SelectedRadioButton)
            {
                case "Option1":
                    itemSourceList.Filter += FilterByBirthDay;
                    CusGrid.ItemsSource = itemlist;
                    break;
                case "Option2":
                    itemSourceList.Filter += FilterByName;
                    CusGrid.ItemsSource = itemlist;
                    break;
                case "Option3":
                    itemSourceList.Filter += FilterByIcNumber;
                    CusGrid.ItemsSource = itemlist;
                    break;
                case "Option4":
                    itemSourceList.Filter += FilterByTel;
                    CusGrid.ItemsSource = itemlist;
                    break;
            }
            if (itemlist.Cast<Customer>().ToList().Count == 1)
            {
                SelectedCustomer = itemlist.Cast<Customer>().ToList()[0];
                if(conditionNotNull)
                    SetSelectedCustomer();
            }
            if (CusGrid.SelectedIndex == -1 && itemlist.Cast<Customer>().ToList().Count > 0)
                CusGrid.SelectedIndex = 0;
        }

        private void FilterByName(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Customer obj)) return;
            e.Accepted = string.IsNullOrEmpty(Condition.Text) || obj.Name.Contains(Condition.Text);
        }
        private void FilterByBirthDay(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Customer obj)) return;
            e.Accepted = string.IsNullOrEmpty(Condition.Text) || ToBirthStr(obj.Birthday).Contains(Condition.Text);
        }

        private void FilterByIcNumber(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Customer obj)) return;
            e.Accepted = string.IsNullOrEmpty(Condition.Text) || obj.IcNumber.Contains(Condition.Text);
        }

        private void FilterByTel(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Customer obj)) return;
            e.Accepted = string.IsNullOrEmpty(Condition.Text) || obj.ContactInfo.Tel.Contains(Condition.Text);
        }

        private void SetSelectedCustomer()
        {
            PrescriptionDec2View.Instance.CurrentPrescription.Customer = SelectedCustomer;
            PrescriptionDec2View.Instance.CurrentCustomerHistoryMaster = CustomerHistoryDb.GetDataByCUS_ID(PrescriptionDec2View.Instance.CurrentPrescription.Customer.Id);
            PrescriptionDec2View.Instance.CusHistoryMaster.ItemsSource = PrescriptionDec2View.Instance.CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection;
            PrescriptionDec2View.Instance.CusHistoryMaster.SelectedIndex = 0;
            PrescriptionDec2View.Instance.CustomerSelected = true;
            PrescriptionDec2View.Instance.NotifyPropertyChanged(nameof(PrescriptionDec2View.Instance.CurrentPrescription));
            Close();
        }
    }
}
