using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Medicine.ControlMedicineEdit;
using His_Pos.Service;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditWindow
{
    /// <summary>
    /// ControlMedicineEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ControlMedicineEditWindow : Window
    {
        public ControlMedicineEditWindow(string medID, string warID)
        {
            InitializeComponent();
            ControlMedicineEditViewModel controlMedicineEditViewModel = new ControlMedicineEditViewModel(medID, warID);
            DataContext = controlMedicineEditViewModel;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseControlMedicineEditWindow")
                    Close();
            });
            ShowDialog();
        }

        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();

            ControlMedicineGrid.SelectedItem = (textBox.DataContext as ControlMedicineEdit);
        }

        private void InputTextbox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void DateMaskedTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }
    }
}