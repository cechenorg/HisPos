using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// AddMedicineWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddMedicineWindow : Window
    {
        public AddMedicineWindow(string search, AddProductEnum addView)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });
            DataContext = new AddProductViewModel(search, addView);
            SearchStringTextBox.Focus();
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
