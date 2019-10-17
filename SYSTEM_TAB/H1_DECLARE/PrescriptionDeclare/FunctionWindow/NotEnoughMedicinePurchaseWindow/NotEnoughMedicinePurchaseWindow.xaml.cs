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

        public NotEnoughMedicinePurchaseWindow(string wareID,string note,NotEnoughMedicineStructs purchaseList)
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
            if (!(sender is DataGridCell cell) || !(cell.DataContext is NotEnoughMedicineStruct med)) return;
            ((PrescriptionDeclareViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }

        private void MedicineID_OnKeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
