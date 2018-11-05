using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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

        string _selectedRadioButton;
        public string SelectedRadioButton
        {
            get
            {
                return _selectedRadioButton;
            }
            set
            {
                if (value != null) //要判斷一下是否為 null，否則選了A，又選B時，最後一個回傳的會是A的值，這樣就抓不到了。
                    _selectedRadioButton = value;
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
            Condition.Text = condition;
            CustomerCollection = CustomerDb.GetData();
            ResultCollection = new ObservableCollection<Customer>();
            DataContext = this;
            SelectedCustomer = new Customer();
            switch (option)
            {
                case 0:
                    SelectedCustomer = CustomerCollection.SingleOrDefault(c => c.Id.Equals("0"));
                    SetSelectedCustomer();
                    break;
                case 1:
                    SelectedRadioButton = "Option1";
                    FilterCustomer(condition);
                    break;
                case 2:
                    SelectedRadioButton = "Option2";
                    FilterCustomer(condition);
                    break;
                case 3:
                    SelectedRadioButton = "Option3";
                    FilterCustomer(condition);
                    break;
                case 4:
                    SelectedRadioButton = "Option4";
                    FilterCustomer(condition);
                    break;
                default:
                    SelectedRadioButton = "Option1";
                    ResultCollection = CustomerCollection;
                    CusGrid.SelectedIndex = 0;
                    break;
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
            ResultCollection.Clear();
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
                return;
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
            ResultCollection.Clear();
            switch (SelectedRadioButton)
            {
                case "Option1":
                    foreach (var c in CustomerCollection)
                    {
                        var birthStr = ToBirthStr(c.Birthday);
                        var search = condition.StartsWith("0")
                            ? condition.Substring(1, condition.Length - 1) : condition;
                        if (birthStr.Contains(search))
                            ResultCollection.Add(c);
                    }

                    foreach (var t in ResultCollection)
                    {
                        var birthStr = ToBirthStr(t.Birthday);
                        if (!birthStr.Equals(condition.Replace("/", "").Replace("-", ""))) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
                case "Option2":
                    foreach (var c in CustomerCollection)
                    {
                        if (c.Name.Contains(condition))
                            ResultCollection.Add(c);
                    }
                    foreach (var t in ResultCollection)
                    {
                        if (!t.Name.Equals(condition)) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
                case "Option3":
                    foreach (var c in CustomerCollection)
                    {
                        if (c.IcNumber.Contains(condition))
                            ResultCollection.Add(c);
                    }
                    foreach (var t in ResultCollection)
                    {
                        if (!t.IcNumber.Equals(condition)) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
                case "Option4":
                    foreach (var c in CustomerCollection)
                    {
                        if (c.ContactInfo.Tel.Contains(condition))
                            ResultCollection.Add(c);
                    }
                    foreach (var t in ResultCollection)
                    {
                        if (!t.ContactInfo.Tel.Equals(condition)) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
            }

            if (ResultCollection.Count == 1)
            {
                SelectedCustomer = ResultCollection[0];
                SetSelectedCustomer();
            }
            if (CusGrid.SelectedIndex == -1 && ResultCollection.Count > 0)
                CusGrid.SelectedIndex = 0;
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
