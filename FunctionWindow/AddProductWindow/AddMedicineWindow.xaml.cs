using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Windows;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// AddMedicineWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddMedicineWindow : Window
    {
        public AddMedicineWindow(string search, AddProductEnum addView, string wareHouseID)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });
            DataContext = new AddProductViewModel(search, addView, wareHouseID);
            SearchStringTextBox.Focus();
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close
            this.Hide();      // Programmatically hides the window
        }
    }
}