using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using His_Pos.Class.Person;
using His_Pos.FunctionWindow;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage
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
            CustomerDetail = NewFunction.DeepCloneViaJson ((Customer)(sender as DataGrid).SelectedItem);
            richtextboxDesc.Document.Blocks.Clear();
            richtextboxDesc.AppendText(CustomerDetail.Description);
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
            CustomerDetail = NewFunction.DeepCloneViaJson(newcustomer);
            richtextboxDesc.Document.Blocks.Clear();
            richtextboxDesc.AppendText(CustomerDetail.Description);
            InitDataChanged();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e) {
            CustomerDetail.Description = new TextRange(richtextboxDesc.Document.ContentStart, richtextboxDesc.Document.ContentEnd).Text;

            ///CustomerDb.UpdateCustomerById(CustomerDetail);
            MessageWindow.ShowMessage("修改完成!", Class.MessageType.SUCCESS);
            messageWindow.ShowDialog();
            for (int i = 0; i < CustomerCollection.Count; i++){
                if (CustomerCollection[i].Id == CustomerDetail.Id) {
                    CustomerCollection[i] = NewFunction.DeepCloneViaJson(CustomerDetail);
                }
            } 
            richtextboxDesc.Document.Blocks.Clear();
            richtextboxDesc.AppendText(CustomerDetail.Description); 
            InitDataChanged();
        }

        private void EmpId_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataGridCustomer.Items.Filter= ((o) => {
                if (((Customer)o).Id.Contains(EmpId.Text) || ((Customer)o).Name.Contains(EmpId.Text))
                    return true;
                else
                    return false;
            } );
        }
    }
}
