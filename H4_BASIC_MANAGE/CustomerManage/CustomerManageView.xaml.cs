using His_Pos.Class.Person;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace His_Pos.H4_BASIC_MANAGE.CustomerManage
{
    /// <summary>
    /// CustomerManageView.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerManageView : UserControl, INotifyPropertyChanged
    {
        private bool isFirst = true;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public ObservableCollection<Customer> customerCollection = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> CustomerCollection
        {
            get
            {
                return customerCollection;
            }
            set
            {
                customerCollection = value;
                NotifyPropertyChanged("CustomerCollection");
            }
        }
        public Customer customerDetail { get; set; }
        public Customer CustomerDetail
        {
            get
            {
                return customerDetail;
            }
            set
            {
                customerDetail = value;
                NotifyPropertyChanged("CustomerDetail");
                NotifyPropertyChanged("CustomerCollection");
            }
        }
        public CustomerManageView()
        {
            InitializeComponent();
            DataContext = this;
            GetCustomerData();
        }
        private void GetCustomerData()
        {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetCustomerData(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        }

        private void DataGridCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null) return;
            CustomerDetail = (Customer)((Customer)(sender as DataGrid).SelectedItem).Clone();
           
            InitDataChanged();
        }
        private void DataChanged()
        {
            if (isFirst) return;

            Changed.Content = "已修改";
            Changed.Foreground = Brushes.Red;

            ButtonCancel.IsEnabled = true;
            ButtonSubmit.IsEnabled = true;
        }

        private void InitDataChanged()
        {
            Changed.Content = "未修改";
            Changed.Foreground = Brushes.Black;

            ButtonCancel.IsEnabled = false;
            ButtonSubmit.IsEnabled = false;
        }
        private void Text_TextChanged(object sender, EventArgs e)
        {
            DataChanged();
        }
        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            isFirst = false;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Customer newcustomer = CustomerCollection.Where(customer => customer.Id == CustomerDetail.Id).ToList()[0];
            CustomerDetail = (Customer)newcustomer.Clone();
            InitDataChanged();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e) {
            CustomerDb.UpdateCustomerById(CustomerDetail);
            MessageWindow messageWindow = new MessageWindow("修改完成!",Class.MessageType.SUCCESS);
            messageWindow.ShowDialog();
            InitDataChanged();
        }
    }
}
