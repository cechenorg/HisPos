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
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Prescription;
using His_Pos.Service;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindowRefactoring
{
    /// <summary>
    /// PrescriptionRecordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionRecordWindow : Window
    {
        public PrescriptionRecordWindow()
        {
            InitializeComponent();
        }
        public PrescriptionRecordWindow(Prescription p, string title)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("ClosePrescriptionEditWindow"))
                    Close();
            });
            Closing += (sender, e) => Messenger.Default.Unregister(this);
            DataContext = new PrescriptionEditViewModel(p, title);
            ShowDialog();
        }
        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is Medicine med)) return;
            ((PrescriptionEditViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }

        private void InputTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            textBox.SelectAll();
            var textBoxList = new List<TextBox>();
            NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref textBoxList);
            var index = textBoxList.IndexOf((TextBox)sender);
            PrescriptionMedicines.SelectedItem = (PrescriptionMedicines.Items[index] as Medicine);
        }
        private void InputTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            e.Handled = true;
            textBox.Focus();
        }
        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                t.CaretIndex++;
            }
        }
        private void DateControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is MaskedTextBox t) t.SelectionStart = 0;
        }
    }
}
