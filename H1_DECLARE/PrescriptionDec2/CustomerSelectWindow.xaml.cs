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

        public Customer SelectedCustomer { get; set; }
        public CustomerSelectWindow(ObservableCollection<Customer> c)
        {
            InitializeComponent();
            DataContext = this;
            CustomerCollection = c;
            SelectedCustomer = new Customer();
            SelectedRadioButton = "Option1";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Customer_Select_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedCustomer = CustomerCollection[CusGrid.SelectedIndex];
            SelectedCustomer.IcCard = new IcCard {IcNumber = SelectedCustomer.IcNumber};
            PrescriptionDec2View.Instance.CurrentPrescription.Customer = SelectedCustomer;
            PrescriptionDec2View.Instance.NotifyPropertyChanged(nameof(PrescriptionDec2View.Instance.CurrentPrescription));
            PrescriptionDec2View.Instance.CurrentCustomerHistoryMaster = CustomerHistoryDb.GetDataByCUS_ID(PrescriptionDec2View.Instance.CurrentPrescription.Customer.Id);
            PrescriptionDec2View.Instance.CusHistoryMaster.ItemsSource = PrescriptionDec2View.Instance.CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection;
            PrescriptionDec2View.Instance.CusHistoryMaster.SelectedIndex = 0;
            Close();
        }
    }
}
