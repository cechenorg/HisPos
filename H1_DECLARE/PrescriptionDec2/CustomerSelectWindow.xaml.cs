using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public Customer SelectedCustomer { get; set; }
        public CustomerSelectWindow(ObservableCollection<Customer> c)
        {
            InitializeComponent();
            CustomerCollection = c;
            ResultCollection = new ObservableCollection<Customer>();
            if (c.Count == 0)
                CustomerCollection = CustomerDb.GetData();
            else
            {
                ResultCollection = c;
            }
            DataContext = this;
            SelectedCustomer = new Customer();
            SelectedRadioButton = "Option1";
            CusGrid.ItemsSource = ResultCollection;
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
            SelectedCustomer.IcCard = new IcCard {IcNumber = SelectedCustomer.IcNumber};
            PrescriptionDec2View.Instance.CurrentPrescription.Customer = SelectedCustomer;
            PrescriptionDec2View.Instance.NotifyPropertyChanged(nameof(PrescriptionDec2View.Instance.CurrentPrescription));
            PrescriptionDec2View.Instance.CurrentCustomerHistoryMaster = CustomerHistoryDb.GetDataByCUS_ID(PrescriptionDec2View.Instance.CurrentPrescription.Customer.Id);
            PrescriptionDec2View.Instance.CusHistoryMaster.ItemsSource = PrescriptionDec2View.Instance.CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection;
            PrescriptionDec2View.Instance.CusHistoryMaster.SelectedIndex = 0;
            Close();
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
            ResultCollection.Clear();
            switch (SelectedRadioButton)
            {
                case "Option1":
                    foreach (var c in CustomerCollection)
                    {
                        var birthStr = ToBirthStr(c.Birthday);
                        var search = Condition.Text.StartsWith("0")
                            ? Condition.Text.Substring(1, Condition.Text.Length - 1) : Condition.Text;
                        if (birthStr.Contains(search))
                            ResultCollection.Add(c);
                    }

                    foreach (var t in ResultCollection)
                    {
                        var birthStr = ToBirthStr(t.Birthday);
                        if (!birthStr.Equals(Condition.Text.Replace("/", "").Replace("-", ""))) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
                case "Option2":
                    foreach (var c in CustomerCollection)
                    {
                        if (c.Name.Contains(Condition.Text))
                            ResultCollection.Add(c);
                    }
                    foreach (var t in ResultCollection)
                    {
                        if (!t.Name.Equals(Condition.Text)) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
                case "Option3":
                    foreach (var c in CustomerCollection)
                    {
                        if (c.IcNumber.Contains(Condition.Text))
                            ResultCollection.Add(c);
                    }
                    foreach (var t in ResultCollection)
                    {
                        if (!t.IcNumber.Equals(Condition.Text)) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
                case "Option4":
                    foreach (var c in CustomerCollection)
                    {
                        if (c.ContactInfo.Tel.Contains(Condition.Text))
                            ResultCollection.Add(c);
                    }
                    foreach (var t in ResultCollection)
                    {
                        if (!t.ContactInfo.Tel.Equals(Condition.Text)) continue;
                        MatchCustomer(t);
                        break;
                    }
                    break;
            }
            if (CusGrid.SelectedIndex == -1 && ResultCollection.Count > 0)
                CusGrid.SelectedIndex = 0;
        }

        private string ToBirthStr(DateTime d)
        {
            var year = (d.Year - 1911).ToString();
            var month = d.Month.ToString().PadLeft(2, '0');
            var day = d.Day.ToString().PadLeft(2, '0');
            return year + month + day;
        }
    }
}
