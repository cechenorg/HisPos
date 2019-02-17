using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using Cus = His_Pos.NewClass.Person.Customer.Customer;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomPrescriptionWindow
{
    /// <summary>
    /// CustomPrescriptionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomPrescriptionWindow : Window
    {
        private CustomPrescriptionViewModel customPrescriptionViewModel { get; set; }
        public CustomPrescriptionWindow(Cus cus,IcCard card)
        {
            InitializeComponent();
            customPrescriptionViewModel = new CustomPrescriptionViewModel(cus, card);
            DataContext = customPrescriptionViewModel;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseCustomPrescription":
                        Close();
                        Messenger.Default.Unregister(this);
                        break;
                }
            });
            if (customPrescriptionViewModel.ShowDialog)
            {
                Messenger.Default.Send(new NotificationMessage("CustomPresChecked"));
                ShowDialog();
                Messenger.Default.Unregister(this);
            }
        }

        private void Reserved_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid d && d.SelectedIndex >= 0)
            {
                Cooperative.SelectedIndex = -1;
                Messenger.Default.Send((Prescription)d.SelectedItem,"ReservedSelected");
            }
                
        }

        private void Cooperative_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid d && d.SelectedIndex >= 0)
            {
                Reserved.SelectedIndex = -1;
                Messenger.Default.Send((Prescription)d.SelectedItem, "CooperativeSelected");
            }
        }

        private void CustomPrescriptionWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Collapsed;
        }
    }
}
