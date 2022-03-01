using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Employee;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage.EmployeeChangePasswordWindow
{
    /// <summary>
    /// EmployeeChangePasswordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EmployeeChangePasswordWindow : Window
    {
        public EmployeeChangePasswordWindow(Employee e)
        {
            InitializeComponent();
            EmployeeChangePasswordWindowViewModel employeeChangePasswordWindowViewModel = new EmployeeChangePasswordWindowViewModel(e);
            DataContext = employeeChangePasswordWindowViewModel;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseEmployeeChangePasswordWindow")
                    Close();
            });
            ShowDialog();
        }
    }
}