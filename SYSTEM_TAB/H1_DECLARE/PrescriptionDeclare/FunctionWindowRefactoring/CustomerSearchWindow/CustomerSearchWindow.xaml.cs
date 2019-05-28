using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindowRefactoring.CustomerSearchWindow
{
    /// <summary>
    /// CustomerSearchWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerSearchWindow : Window
    {
        private CustomerSearchViewModel customerSearchViewModel { get; set; }
        public CustomerSearchWindow(string search,CustomerSearchCondition condition)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) => 
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSearchWindow"))
                    Close();
            });
            customerSearchViewModel = new CustomerSearchViewModel(search, condition);
            DataContext = customerSearchViewModel;
            SearchStringTextBox.Focus();
            if (customerSearchViewModel.ShowDialog)
                ShowDialog();
        }

        public CustomerSearchWindow(DateTime birth)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCustomerSearchWindow"))
                    Close();
            });
            customerSearchViewModel = new CustomerSearchViewModel(birth);
            DataContext = customerSearchViewModel;
            if (customerSearchViewModel.ShowDialog)
                ShowDialog();
        }
    }
}
