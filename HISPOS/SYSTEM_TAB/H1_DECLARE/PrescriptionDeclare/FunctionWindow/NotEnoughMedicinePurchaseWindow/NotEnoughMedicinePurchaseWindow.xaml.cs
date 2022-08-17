using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Medicine.NotEnoughMedicine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public NotEnoughMedicinePurchaseWindow(string note, string cusName, NotEnoughMedicines purchaseList)
        {
            InitializeComponent();
            DataContext = new NotEnoughMedicinePurchaseViewModel(note, cusName, purchaseList);
            ShowDialog();
        }

        private void ShowMedicineDetail(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGridCell cell) || !(cell.DataContext is NotEnoughMedicine med)) return;
            ((NotEnoughMedicinePurchaseViewModel)DataContext).ShowMedicineDetail.Execute(med.ID);
        }
    }
}