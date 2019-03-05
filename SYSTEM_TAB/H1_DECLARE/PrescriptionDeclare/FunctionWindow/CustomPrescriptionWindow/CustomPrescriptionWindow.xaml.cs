using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.CustomerPrescription;
using Cus = His_Pos.NewClass.Person.Customer.Customer;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CustomPrescriptionWindow
{
    /// <summary>
    /// CustomPrescriptionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomPrescriptionWindow : Window
    {
        private CustomPrescriptionViewModel customPrescriptionViewModel { get; set; }
        public CustomPrescriptionWindow(int cusID,string cusIDNumber, IcCard card)
        {
            InitializeComponent();
            customPrescriptionViewModel = new CustomPrescriptionViewModel(cusID,cusIDNumber, card);
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
            }
            Messenger.Default.Unregister(this);
        }

        private void Reserved_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid d && d.SelectedIndex >= 0)
            {
                Cooperative.SelectedIndex = -1;
                Registered.SelectedIndex = -1;
                Messenger.Default.Send(new NotificationMessage<int>(((RegisterAndReservePrescription)d.SelectedItem).ID, "ReserveSelectionChanged"));
            }
        }

        private void Cooperative_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid d && d.SelectedIndex >= 0)
            {
                Reserved.SelectedIndex = -1;
                Registered.SelectedIndex = -1;
                Messenger.Default.Send(((Prescription)d.SelectedItem).Remark, "CooperativePrescriptionSelectionChanged");
            }
        }
        private void Registered_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid d && d.SelectedIndex >= 0)
            {
                Cooperative.SelectedIndex = -1;
                Reserved.SelectedIndex = -1;
                Messenger.Default.Send(new NotificationMessage<int>(((RegisterAndReservePrescription)d.SelectedItem).ID, "RegisterSelectionChanged"));
            }
        }

        private void CustomPrescriptionWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Collapsed;
        }

        private void PrescriptionSelected(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row?.Item is null) return;
            Messenger.Default.Send(new NotificationMessage("CustomPrescriptionSelected"));
        }
    }
}
