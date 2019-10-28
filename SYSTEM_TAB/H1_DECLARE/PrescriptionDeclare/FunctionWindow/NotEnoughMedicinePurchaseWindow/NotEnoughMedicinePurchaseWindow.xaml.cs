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
using His_Pos.NewClass.Medicine.InventoryMedicineStruct;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.NotEnoughMedicinePurchaseWindow
{
    /// <summary>
    /// NotEnoughMedicinePurchaseWindow.xaml 的互動邏輯
    /// </summary>
    public partial class NotEnoughMedicinePurchaseWindow : Window
    {
        public NotEnoughMedicinePurchaseWindow()
        {
            InitializeComponent();
        }

        public NotEnoughMedicinePurchaseWindow(string wareID,string note,NotEnoughMedicines purchaseList)
        {
            InitializeComponent();
            DataContext = new NotEnoughMedicinePurchaseViewModel(wareID,note,purchaseList);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseNotEnoughMedicinePurchaseWindow"))
                {
                    Close();
                }
            });
            ShowDialog();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is NotEnoughMedicine med)) return;
            ((PrescriptionDeclareViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }

        private void DoubleTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            var t = sender as TextBox;
            if (e.Key != Key.Decimal) return;
            e.Handled = true;
            if (t != null) t.CaretIndex++;
        }

        private void InputTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InputTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
